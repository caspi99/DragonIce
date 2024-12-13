using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{
    public int assigned_player;     //Int with the id of the assigned player to this tree drag

    [SerializeField]
    private GameObject scythe;

    private int grabbable_phase_id;

    private int grabbable_id;

    private int internal_id;

    private int grabbable_shape;

    private SoundController sound_controller; //to call some sound methods
    private NewSoundController nwsc; //to call some sound methods
    private BoxCollider grip_collider;
    private float center_offset;
    private Grabbable parent;

    //DATA EXTRACTION
    private DataExtractorCSVLevel2 data_extractor;
    private TimeManager timeManager;

    //Timer
    private int timer;

    //Individual or collaborative bool
    private bool collaborative = false;

    // Start is called before the first frame update
    void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        timer = timeManager.CreateTimer();
        timeManager.PauseTimer(timer);
    }

    // Update is called once per frame
    void Update()
    {
        //Show/Hide the scythe
        scythe.SetActive(GetIfBeingUsed());

        //Debug.Log(this.gameObject.name);
        //Debug.Log(assigned_player);
    }

    //method to get the id of a player
    private int GetPlayerId(string name) { return int.Parse(name[name.Length - 1].ToString()) - 1; }

    //grab action method
    private void GrabGripMethod(Collider collider)
    {
        PlayerLevel2 curr_player = collider.gameObject.GetComponent<PlayerLevel2>();

        bool grabbing_grip_condition = curr_player.GetIfGrabbingGrip(-1, -1, -1);

        if (grabbing_grip_condition)
        {
            //PLAYER PREFERENCE
            if (CheckboxManagerLevel2.preferencePlayerStone && (parent.GetPreferenceGrip() < 0))
            {
                parent.SetPreferenceGrip(internal_id);

                if (parent.GetGrabbableType() == GameConstants.STONE_PHASE_1)
                {
                    grip_collider.center = new Vector3(-center_offset, 0, 0);
                    grip_collider.size = new Vector3(12f, 1f, 6f);
                }
            }

            //BIGGER GRIPS WHEN GRABBED
            if (CheckboxManagerLevel2.biggerGripsWhenGrabbed)
            {
                if (parent.GetGrabbableType() == GameConstants.STONE_PHASE_1)
                {
                    grip_collider.center = new Vector3(-grip_collider.center.x, 0f, 0f);
                    grip_collider.size = new Vector3(10f, 1f, 6f);
                }
                else if (parent.GetGrabbableType() == GameConstants.TREE_PHASE_2)
                {
                    grip_collider.center = new Vector3(0f, 0f, 0f);
                    grip_collider.size = new Vector3(10f, 1f, 5f);
                }
            }

            //GRAB ACTION
            assigned_player = GetPlayerId(collider.gameObject.name);

            curr_player.SetGrabbingGrip(grabbable_phase_id, grabbable_id, internal_id);

            //SOUND OF THE GRAB
            if (Settings.binauralSound)
            {
                nwsc.DynamicPlayClip(SoundInformationLevel2.GRAB_SOUND, this.transform);
            }
            else
            {
                sound_controller.PlayClipOneShot(SoundInformationLevel2.GRAB_SOUND);
            }

            //DATA EXTRACTION
            data_extractor.WriteDataLineMovingGrabbable(assigned_player, DataExtractorCSVLevel2.ACTIVE, grabbable_id, grabbable_shape);
        }
    }

    public void UngrabMethod(PlayerLevel2 curr_player)
    {
        bool grabbing_grip_condition = curr_player.GetIfGrabbingGrip(grabbable_phase_id, grabbable_id, internal_id);

        if (grabbing_grip_condition)
        {
            //PLAYER PREFERENCE
            if (CheckboxManagerLevel2.preferencePlayerStone && (parent.GetPreferenceGrip() == internal_id))
            {
                parent.SetPreferenceGrip(-1);

                if (parent.GetGrabbableType() == GameConstants.STONE_PHASE_1)
                {
                    grip_collider.center = new Vector3(center_offset, 0, 0);
                    grip_collider.size = new Vector3(6f, 1f, 2.2f);
                }
            }

            //BIGGER GRIPS WHEN GRABBED
            if (CheckboxManagerLevel2.biggerGripsWhenGrabbed)
            {
                if(parent.GetGrabbableType() == GameConstants.STONE_PHASE_1)
                {
                    grip_collider.center = new Vector3(-grip_collider.center.x, 0f, 0f);
                    grip_collider.size = new Vector3(6f, 1f, 6f);
                }
                else if(parent.GetGrabbableType() == GameConstants.TREE_PHASE_2)
                {
                    grip_collider.center = new Vector3(0f, 0f, 0f);
                    grip_collider.size = new Vector3(5f, 1f, 5f);
                }
            }

            //UNGRAB ACTION
            curr_player.ResetGrabbingGrip();
            int previous_player = assigned_player;  //for data extraction
            assigned_player = -1;

            //SOUND OF THE UNGRAB
            if (Settings.binauralSound)
            {
                nwsc.DynamicPlayClip(SoundInformationLevel2.DROP_SOUND, this.transform);
            }
            else
            {
                sound_controller.PlayClipOneShot(SoundInformationLevel2.DROP_SOUND);
            }

            //DATA EXTRACTION
            data_extractor.WriteDataLineMovingGrabbable(previous_player, DataExtractorCSVLevel2.INACTIVE, grabbable_id, grabbable_shape);
        }
    }

    public void ResetGripTimer()
    {
        timeManager.ResetTimer(timer);
    }

    //enter trigger
    void OnTriggerEnter(Collider collider)
    {
        bool condition = (collider.gameObject.tag == "Player") && (assigned_player < 0);
        bool playStyleCondition = CheckboxManagerLevel2.exclusivePlayStyleGrabbables && !collaborative && (parent.GetNumOfGrabbedGrips() >= 1);

        //In case it is not StayToPickStone, we use Enter Trigger
        if (!CheckboxManagerLevel2.stayToPickStone && condition && !playStyleCondition)
        {
            GrabGripMethod(collider);
        }
    }

    //stay trigger
    void OnTriggerStay(Collider collider)
    {
        bool condition = (collider.gameObject.tag == "Player") && (assigned_player < 0);
        bool playStyleCondition = CheckboxManagerLevel2.exclusivePlayStyleGrabbables && !collaborative && (parent.GetNumOfGrabbedGrips() >= 1);

        //In case it is StayToPickStone, we use Stay Trigger
        if (CheckboxManagerLevel2.stayToPickStone && condition && !playStyleCondition)
        {
            if(timeManager.WaitTime(timer, CheckboxManagerLevel2.stayToPickStoneTime)) { GrabGripMethod(collider); }
        }
    }

    //exit trigger
    void OnTriggerExit(Collider collider)
    {
        bool condition = (collider.gameObject.tag == "Player") && (assigned_player >= 0);

        bool resetPickStoneCondition = false;
        bool exitPlayerCondition = false;

        if(collider.gameObject.tag == "Player")
        {
            int exitPlayerId = GetPlayerId(collider.gameObject.name);
            exitPlayerCondition = assigned_player == exitPlayerId;
            resetPickStoneCondition = (assigned_player < 0) || exitPlayerCondition;
        }

        //Timer reset by StayPickStone condition
        if (CheckboxManagerLevel2.stayToPickStone && resetPickStoneCondition) { timeManager.ResetTimer(timer); }

        if (condition && exitPlayerCondition)
        {
            PlayerLevel2 curr_player = collider.gameObject.GetComponent<PlayerLevel2>();

            UngrabMethod(curr_player);

            if (CheckboxManagerLevel2.allGripsUntachCollaborative) { parent.ResetGripsOfThisGrabbable(); }
        }
    }

    //method to get the player id holding the grip
    public int GetAssignedPlayer() { return assigned_player; }

    public void SetAssignedPlayer(int assigned_player) { this.assigned_player = assigned_player; }

    //method to get if a grip is being holded by a player
    public bool GetIfBeingHold(string player_name)
    {
        int player_idx = GetPlayerId(player_name);
        return player_idx == assigned_player;
    }

    //method to get if the grip is being used
    public bool GetIfBeingUsed() { return assigned_player > -1; }

    public (int phase_id, int grab_id, int grip_id) GetInternalId() { return (grabbable_phase_id, grabbable_id, internal_id); }
    public void SetInternalId(int phase_id, int grab_id, int grip_id) { grabbable_phase_id = phase_id; grabbable_id = grab_id; internal_id = grip_id; }

    public void InitAudio()
    {
        sound_controller = GameObject.Find("SoundController").GetComponent<SoundController>();
        nwsc = GameObject.Find("SoundController").GetComponent<NewSoundController>();
    }
    public void InitBoxCollider()
    {
        grip_collider = this.gameObject.GetComponent<BoxCollider>();
        center_offset = grip_collider.center.x;
    }

    public void ChangeGripVisibility()
    {
        this.gameObject.GetComponentInChildren<MeshRenderer>().enabled = CheckboxManagerLevel2.show_grips; //show/hide grip
    }

    public void InitParent()
    {
        parent = this.transform.root.GetComponent<Grabbable>();
    }

    public void InitDataExtractor() { data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel2>(); }

    public int GetGrabbableShape() { return grabbable_shape; }
    public void SetGrabbableShape(int shape) { this.grabbable_shape = shape; }
    public void SetCollaborative(bool status) { this.collaborative = status; }
}
