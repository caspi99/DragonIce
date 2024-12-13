using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grabbable : InteractiveObject
{
    //<-------GRABBABLE ATRIBUTES------>
    [SerializeField]
    protected List<Grip> grips_left;

    [SerializeField]
    protected List<Grip> grips_right;

    protected int grabbable_type;

    [SerializeField]
    protected int shape;

    protected int internal_id;

    protected int preference_grip_id;

    //GameObject Components

    [SerializeField]
    protected GameObject object_middle;

    protected Rigidbody rb;

    protected float inclinationPositionOffset = 3.2f;
    protected float inclinationRotationOffset = 10f;

    //Individual or collaborative bool
    protected bool collaborative = false;

    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        FixIfBugged();
    }

    //<-------GRABBABLE METHODS------>

    //<-------START METHODS------>

    protected virtual void ChangeGrabbableName()
    {
        string grabbable_name = grabbable_type.ToString() + "_" + shape.ToString();

        this.gameObject.name = grabbable_name;
    }

    //<-------METHODS TO USE INTERNALLY------>

    protected virtual List<Grip> GetSideGrips(int side)
    {
        switch (side)
        {
            case GameConstants.LEFT_SIDE:
                return grips_left;
            case GameConstants.RIGHT_SIDE:
                return grips_right;
            default:
                return new List<Grip>();
        }
    }

    protected virtual List<(int grip, Transform player)> GetGrabbedGripsStructureList(List<Transform> players, int side)
    {
        List<(int grip, Transform player)> grabbed_grips = new List<(int grip, Transform player)>();

        List<Grip> current_side_grips = GetSideGrips(side);

        for (int i = 0; i < current_side_grips.Count; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {
                bool drag_condition = current_side_grips[i].GetIfBeingHold(players[j].gameObject.name);
                if (drag_condition)
                {                    
                    grabbed_grips.Add((i, players[j]));
                }
            }
        }

        return grabbed_grips;
    }

    protected virtual Vector3 GetVectorsMean(List<Vector3> vectors)
    {
        Vector3 mean = Vector3.zero;

        for(int i = 0; i < vectors.Count; i++)
        {
            mean += vectors[i];
        }

        return mean/vectors.Count;
    }

    protected virtual void ChangeGravityStatus(bool status)
    {
        rb.useGravity = status;
    }

    protected virtual void EnableGravity() { ChangeGravityStatus(true); }
    protected virtual void DisableGravity() { ChangeGravityStatus(false); }

    //protected virtual void ResetAngularVelocity() { rb.angularVelocity = Vector3.zero; }

    //<-------MOVING METHODS------>

    protected virtual void CalculateMovement(List<(int grip, Transform player)> grabbed_grips_left, List<(int grip, Transform player)> grabbed_grips_right)
    {
        //In case the grips are grabbed from both sides
        if ((grabbed_grips_left.Count > 0) && (grabbed_grips_right.Count > 0))
        {
            //In case the object is grabbed, we deactivate gravity
            DisableGravity();

            CalculateRectangleMovementCollaborative(grabbed_grips_left, grabbed_grips_right);
        }
        //In case the grips are grabbed from left side
        else if (grabbed_grips_left.Count > 0)
        {
            //In case the object is grabbed, we deactivate gravity
            DisableGravity();

            CalculateRectangleMovementSolo(grabbed_grips_left, GameConstants.LEFT_SIDE);
        }
        //In case the grips are grabbed from right side
        else if (grabbed_grips_right.Count > 0)
        {
            //In case the object is grabbed, we deactivate gravity
            DisableGravity();

            CalculateRectangleMovementSolo(grabbed_grips_right, GameConstants.RIGHT_SIDE);
        }
        //In case there is no grip grabbed
        else
        {
            //In case the object is not grabbed, we activate gravity
            EnableGravity();
        }
    }

    protected virtual void CalculateRectangleMovementSolo(List<(int grip, Transform player)> grabbed_grips, int side)
    {
        //<---DATA COLLECTION--->

        //<--PLAYERS POSITIONS-->
        List<Vector3> players_positions = new List<Vector3>();

        for (int i = 0; i < grabbed_grips.Count; i++)
        {
            Vector3 player = grabbed_grips[i].player.position;
            players_positions.Add(player);
        }

        Vector3 players_mean = GetVectorsMean(players_positions);

        //<--GRIPS POSITIONS-->
        List<Grip> current_side_grips = GetSideGrips(side);

        List<Vector3> grips_positions = new List<Vector3>();

        for (int i = 0; i < current_side_grips.Count; i++)
        {
            Vector3 grip = current_side_grips[i].gameObject.transform.position;
            grips_positions.Add(grip);
        }

        Vector3 grips_mean = GetVectorsMean(grips_positions);

        //<---POSITION--->

        Vector3 direction_to_player = players_mean - grips_mean;
        direction_to_player.y = 0.0f;

        //<--SPEED CALCULATION-->
        float move_speed = CheckboxManagerLevel2.grabbable_move_speed_solo;

        //To check the condition of the exclusive collaborative
        if(CheckboxManagerLevel2.exclusivePlayStyleGrabbables && collaborative && (GetNumOfGrabbedGrips() < 2)) { move_speed = 0.0f; }

        //To add more force if there are 2 players
        if(grabbed_grips.Count > 1) { move_speed *= 2; }

        //To avoid bugs while standing in the same position
        if (direction_to_player.magnitude < 0.5f) { move_speed = 0.0f; }

        //Speed vector
        Vector3 speed = direction_to_player.normalized * Time.deltaTime * move_speed;

        //<--DISPLACEMENT-->

        Vector3 curr_pos = this.gameObject.transform.position;

        //<-INCLINATION FIX->
        if (CheckboxManagerLevel2.inclinate_grabbable)
        {
            if (grabbable_type == GameConstants.STONE_PHASE_1) { curr_pos.y = 2.75f; }
            //if (grabbable_type == GameConstants.TREE_PHASE_2) { curr_pos.y = 6.1f; }
            if (grabbable_type == GameConstants.TREE_PHASE_2) { curr_pos.y = inclinationPositionOffset; }
        }

        //<-CHANGING POSITION->
        if (CheckboxManagerLevel2.use_rigidbody_translation) { rb.MovePosition(curr_pos + speed); }
        else { this.gameObject.transform.position = curr_pos + speed; }


        //<---ROTATION--->

        //we create these vectors with (y = 0.0f) to avoid problems calculating the angle
        Vector3 player_pos_for_rot = new Vector3(players_mean.x, 0.0f, players_mean.z);
        Vector3 middle_pos_for_rot = new Vector3(object_middle.transform.position.x, 0.0f, object_middle.transform.position.z);

        Vector3 direction_player_center = (player_pos_for_rot - middle_pos_for_rot).normalized;

        RotateRectangle(direction_player_center, GameConstants.SOLO, side);
    }

    protected virtual void CalculateRectangleMovementCollaborative(List<(int grip, Transform player)> grabbed_grips_left, List<(int grip, Transform player)> grabbed_grips_right)
    {
        //<---DATA COLLECTION--->

        //<--PLAYERS POSITIONS LEFT-->
        List<Vector3> positions_left = new List<Vector3>();

        for (int i = 0; i < grabbed_grips_left.Count; i++)
        {
            Vector3 player = grabbed_grips_left[i].player.position;
            positions_left.Add(player);
        }

        //<--PLAYERS POSITIONS RIGHT-->
        List<Vector3> positions_right = new List<Vector3>();

        for (int i = 0; i < grabbed_grips_right.Count; i++)
        {
            Vector3 player = grabbed_grips_right[i].player.position;
            positions_right.Add(player);
        }

        //<--PLAYERS MEANS-->
        List<Vector3> means = new List<Vector3>();
        means.Add(GetVectorsMean(positions_left));
        means.Add(GetVectorsMean(positions_right));

        //<---POSITION--->

        Vector3 final_position = GetVectorsMean(means);
        if (grabbable_type == GameConstants.STONE_PHASE_1) { final_position.y = 3.0f; }
        if (grabbable_type == GameConstants.TREE_PHASE_2) { final_position.y = 6.5f; }

        //<--DISPLACEMENT-->
        //<-CHANGING POSITION->
        if (CheckboxManagerLevel2.use_rigidbody_translation) { rb.MovePosition(final_position); }
        else { this.gameObject.transform.position = final_position; }


        //<---ROTATION--->

        Vector3 direction_between_player = (means[GameConstants.LEFT_SIDE] - means[GameConstants.RIGHT_SIDE]).normalized;

        RotateRectangle(direction_between_player, GameConstants.COLLABORATIVE);
    }

    protected virtual void RotateRectangle(Vector3 direction, int type_of_rotation, int grabbed_grips = GameConstants.LEFT_SIDE)
    {
        //<---ANGLE CALCULATION--->

        Vector3 start_vector = new Vector3(-1.0f, 0.0f, 0.0f);

        float dot_vector = Vector3.Dot(start_vector, direction);
        float cos_angle = dot_vector / Vector3.Magnitude(start_vector) * Vector3.Magnitude(direction);
        float angle = Mathf.Acos(cos_angle) * Mathf.Rad2Deg;

        if(grabbed_grips == GameConstants.RIGHT_SIDE) { angle -= 180.0f; }

        if (direction.z < 0.0f) { angle = -angle; }

        //<---INCLINATION--->

        float inclination_angle = 0.0f;

        if(type_of_rotation == GameConstants.SOLO)
        {
            //Inclination of the tree
            if (CheckboxManagerLevel2.inclinate_grabbable)
            {
                if (grabbed_grips == GameConstants.LEFT_SIDE) { inclination_angle = -inclinationRotationOffset; }
                else { inclination_angle = inclinationRotationOffset; }
            }

            //Clamp of rotation to make it move with more friction
            //angle = Mathf.Clamp(angle, -5.0f, 5.0f);
        }

        //<---CHANGING ROTATION--->

        if (CheckboxManagerLevel2.use_rigidbody_translation) { rb.MoveRotation(Quaternion.Euler(new Vector3(0.0f, angle, inclination_angle))); }
        else { this.gameObject.transform.localEulerAngles = new Vector3(0.0f, angle, inclination_angle); }
        
    }

    protected virtual void FixIfBugged()
    {
        Vector3 position = this.gameObject.transform.position;
        
        if (position.y < 0.0f)
        {
            position.y = 4.0f;

            this.gameObject.transform.position = position;
        }
    }

    //<-------METHODS TO USE EXTERNALLY------>

    public virtual void MoveGrabbable(List<Transform> players)
    {
        List<(int grip, Transform player)> grabbed_grips_left = GetGrabbedGripsStructureList(players, GameConstants.LEFT_SIDE);

        List<(int grip, Transform player)> grabbed_grips_right = GetGrabbedGripsStructureList(players, GameConstants.RIGHT_SIDE);

        CalculateMovement(grabbed_grips_left, grabbed_grips_right);
    }

    public virtual bool GetIfGrabbablePlaced()
    {
        return this.gameObject.name == "disabled";
    }

    public virtual void ResetGrabbedGripsPlayers(List<Transform> players)
    {
        List<(int grip, Transform player)> grabbed_grips_left = GetGrabbedGripsStructureList(players, GameConstants.LEFT_SIDE);
        List<(int grip, Transform player)> grabbed_grips_right = GetGrabbedGripsStructureList(players, GameConstants.RIGHT_SIDE);

        for(int i = 0; i < grabbed_grips_left.Count; i++)
        {
            grabbed_grips_left[i].player.gameObject.GetComponent<PlayerLevel2>().ResetGrabbingGrip();
        }

        for (int i = 0; i < grabbed_grips_right.Count; i++)
        {
            grabbed_grips_right[i].player.gameObject.GetComponent<PlayerLevel2>().ResetGrabbingGrip();
        }
    }

    private void FindPlayerAndUntachGrip(Grip grip)
    {
        int assignedPlayer = grip.GetAssignedPlayer();

        if (assignedPlayer >= 0)
        {
            GameObject player = GameObject.Find("Player" + (assignedPlayer + 1).ToString());
            grip.UngrabMethod(player.GetComponent<PlayerLevel2>());
        }

        if (CheckboxManagerLevel2.stayToPickStone) { grip.ResetGripTimer(); }
    }

    public void ResetGripsOfThisGrabbable()
    {
        for(int i = 0; i < grips_left.Count; i++)
        {
            FindPlayerAndUntachGrip(grips_left[i]);
        }

        for (int i = 0; i < grips_right.Count; i++)
        {
            FindPlayerAndUntachGrip(grips_right[i]);
        }
    }

    public virtual int GetInternalId() { return internal_id; }
    public virtual void SetInternalId(int id) { internal_id = id; }

    protected virtual void InitGrips(int side)
    {
        List<Grip> current_side_grips = GetSideGrips(side);

        for(int i = 0; i < current_side_grips.Count; i++)
        {
            current_side_grips[i].SetAssignedPlayer(-1);
            current_side_grips[i].SetInternalId(grabbable_type, internal_id, i + side * 2);
            current_side_grips[i].InitAudio();
            current_side_grips[i].InitBoxCollider();
            current_side_grips[i].ChangeGripVisibility();
            current_side_grips[i].InitParent();
            current_side_grips[i].InitDataExtractor();
            current_side_grips[i].SetGrabbableShape(shape);
            current_side_grips[i].SetCollaborative(collaborative);
        }
    }

    public virtual void InitGrabbable(int id, bool collaborative)
    {
        ChangeGrabbableName();

        rb = this.gameObject.GetComponent<Rigidbody>();

        rb.centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);

        SetInternalId(id);
        preference_grip_id = -1;

        this.collaborative = collaborative;

        InitGrips(GameConstants.LEFT_SIDE);
        InitGrips(GameConstants.RIGHT_SIDE);
    }

    public int GetShape() { return shape; }
    public int GetGrabbableType() { return grabbable_type; }

    public int GetPreferenceGrip() { return preference_grip_id; }
    public void SetPreferenceGrip(int preference_grip_id) { this.preference_grip_id = preference_grip_id; }
    public void SetCollaborative(bool status) { this.collaborative = status; }
    public int GetNumOfGrabbedGrips()
    {
        int result = 0;
        foreach(Grip grip in grips_left)
        {
            if (grip.GetIfBeingUsed()) { result++; }
        }
        foreach (Grip grip in grips_right)
        {
            if (grip.GetIfBeingUsed()) { result++; }
        }
        return result;
    }

    /*
    protected virtual void CalculateOldRectangleMovementSolo(int grabbed_grip, Transform player, int side)
    {
        List<Grip> current_side_grips = GetSideGrips(side);

        //POSITION

        Vector3 direction_to_player = player.position - current_side_grips[grabbed_grip].gameObject.transform.position;

        Vector3 speed = direction_to_player.normalized * Time.deltaTime * 10;
        speed.y = 0.0f;

        this.gameObject.transform.position += speed;

        //ROTATION

        //we create these vectors with (y = 0.0f) to avoid problems calculating the angle
        Vector3 player_pos_for_rot = new Vector3(player.position.x, 0.0f, player.position.z);
        Vector3 middle_pos_for_rot = new Vector3(object_middle.transform.position.x, 0.0f, object_middle.transform.position.z);

        Vector3 direction_player_center = (player_pos_for_rot - middle_pos_for_rot).normalized;

        //this.gameObject.GetComponent<Rigidbody>().AddForce(10*direction_player_center);

        RotateRectangle(direction_player_center);
    }
    */

    /*
    protected virtual void CalculateOldRectangleMovementCollaborative(List<(int grip, Transform player)> grabbed_grips)
    {
        Vector3 final_position = Vector3.zero;

        for (int i = 0; i < grabbed_grips.Count; i++)
        {
            Transform player = grabbed_grips[i].player;
            final_position += player.position;
        }

        final_position = final_position / grips.Count;
        final_position.y = 1.4f;

        this.gameObject.transform.position = final_position;

        Vector3 direction_between_player = (grabbed_grips[0].player.position - grabbed_grips[grabbed_grips.Count - 1].player.position).normalized;

        RotateRectangle(direction_between_player);
    }
     */
}
