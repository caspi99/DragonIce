using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePhase2 : Grabbable
{
    //DATA EXTRACTION
    private PlayersControllerLevel2 player_controller;
    private DataExtractorCSVLevel2 data_extractor;

    // For the inclination fix of the tree
    private float baseInclinationPositionOffset = 4.65f;
    private float baseMaxInclinationPositionOffset = 6f;
    private float baseInclinationRotationOffset = 20f;
    private bool correctInclination = false;

    public override void InitGrabbable(int id, bool status)
    {
        //We change the value of the grabbable to its corresponding type
        grabbable_type = GameConstants.TREE_PHASE_2;
        player_controller = GameObject.Find("PlayersController").GetComponent<PlayersControllerLevel2>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel2>();

        base.InitGrabbable(id, status);
    }

    protected override void Update()
    {
        ChangeInclinationOfTree();
        base.Update();
    }

    //<-------METHODS TO USE INTERNALLY------>

    //protected virtual void ResetAngularVelocity() { rb.angularVelocity = Vector3.zero; }

    //<-------MOVING METHODS------>

    /*
    protected override void CalculateMovement(List<(int grip, Transform player)> grabbed_grips_left, List<(int grip, Transform player)> grabbed_grips_right)
    {
        Vector3 middle_pos_for_rot = new Vector3(object_middle.transform.position.x, 0.0f, object_middle.transform.position.z);

        //<--PLAYERS DIRECTIONS LEFT-->
        List<Vector3> directions_left = new List<Vector3>();

        for (int i = 0; i < grabbed_grips_left.Count; i++)
        {
            Vector3 player = grabbed_grips_left[i].player.position;
            player.y = 0.0f;

            directions_left.Add((middle_pos_for_rot - player).normalized);
        }

        //<--PLAYERS DIRECTIONS RIGHT-->
        List<Vector3> directions_right = new List<Vector3>();

        for (int i = 0; i < grabbed_grips_right.Count; i++)
        {
            Vector3 player = grabbed_grips_right[i].player.position;
            player.y = 0.0f;

            directions_right.Add((middle_pos_for_rot - player).normalized);
        }

        //<--PLAYERS MEANS-->
        List<Vector3> means = new List<Vector3>();

        if (directions_left.Count > 0) { means.Add(GetVectorsMean(directions_left)); }

        if (directions_right.Count > 0) { means.Add(GetVectorsMean(directions_right)); }

        if (means.Count > 0)
        {
            //<---POSITION--->

            Vector3 final_direction = GetVectorsMean(means);

            //<--DISPLACEMENT-->

            //AddForce(final_direction.normalized);

            //<---ROTATION--->

            AddTorque(final_direction.normalized);
        }
    }

    private void AddForce(Vector3 direction)
    {
        rb.AddForce(direction, ForceMode.Impulse);
    }

    private void AddTorque(Vector3 direction)
    {
        //<---ANGLE CALCULATION--->
        Vector3 start_vector = new Vector3(Mathf.Cos(this.transform.rotation.y * Mathf.Deg2Rad), 0, Mathf.Sin(this.transform.rotation.y * Mathf.Deg2Rad));

        float dot_vector = Vector3.Dot(start_vector, direction);
        float cos_angle = dot_vector / Vector3.Magnitude(start_vector) * Vector3.Magnitude(direction);
        float angle = Mathf.Acos(cos_angle) * Mathf.Rad2Deg;

        //if (grabbed_grips == GameConstants.RIGHT_SIDE) { angle -= 180.0f; }

        if (direction.z < 0.0f) { angle = -angle; }

        rb.AddTorque(0f, 1/angle*rb.mass, 0f, ForceMode.VelocityChange);
    }
    */

    public override void MoveGrabbable(List<Transform> players)
    {
        //Only activate when it has the checkbox activated
        if (CheckboxManagerLevel2.pullTreeMechanic) { base.MoveGrabbable(players); }
    }

    private void ChangeDragAndAngularDrag(float drag, float angular_drag)
    {
        rb.angularDrag = angular_drag;
        rb.drag = drag;
    }
    private void ChangeMass(float mass)
    {
        rb.mass = mass;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!CheckboxManagerLevel2.pullTreeMechanic && collision.gameObject.tag == "Player")
        {
            ChangeDragAndAngularDrag(0.1f, 0.1f);
            ChangeMass(CheckboxManagerLevel2.tree_mass_touching);
            //Debug.Log("Pushing");

            collision.gameObject.GetComponent<PlayerLevel2>().SetGrabbingGrip(grabbable_type, internal_id, - 1);

            //DATA EXTRACTION
            int assigned_player = GetPlayerId(collision.gameObject.name);
            data_extractor.WriteDataLineMovingGrabbable(assigned_player, DataExtractorCSVLevel2.ACTIVE, internal_id, shape);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Inclination fix
        if (CheckboxManagerLevel2.pullTreeMechanic && collision.gameObject.tag == "TreePhase2")
        {
            correctInclination = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!CheckboxManagerLevel2.pullTreeMechanic && collision.gameObject.tag == "Player")
        {
            ChangeDragAndAngularDrag(1f, 1f);
            ChangeMass(CheckboxManagerLevel2.tree_mass_default);
            //Debug.Log("Not pushing");

            collision.gameObject.GetComponent<PlayerLevel2>().SetGrabbingGrip(-1, -1, -1);

            //DATA EXTRACTION
            int assigned_player = GetPlayerId(collision.gameObject.name);
            data_extractor.WriteDataLineMovingGrabbable(assigned_player, DataExtractorCSVLevel2.INACTIVE, internal_id, shape);
        }

        // Inclination fix
        if (CheckboxManagerLevel2.pullTreeMechanic && collision.gameObject.tag == "TreePhase2")
        {
            correctInclination = false;
        }
    }

    //method to change the inclination of the tree
    private void ChangeInclinationOfTree()
    {
        if (correctInclination)
        {
            inclinationPositionOffset = Mathf.Min(inclinationPositionOffset + 1 * Time.deltaTime, baseMaxInclinationPositionOffset);
            inclinationRotationOffset = Mathf.Max(inclinationRotationOffset - 1 * Time.deltaTime, 0f);
        }
        else
        {
            inclinationPositionOffset = Mathf.Max(inclinationPositionOffset - 1 * Time.deltaTime, baseInclinationPositionOffset);
            inclinationRotationOffset = Mathf.Min(inclinationRotationOffset + 1 * Time.deltaTime, baseInclinationRotationOffset);
        }
    }

    //method to get the id of a player
    private int GetPlayerId(string name) { return int.Parse(name[name.Length - 1].ToString()) - 1; }
}
