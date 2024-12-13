using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    //<--------------------TARGET VARIABLES------------------------->

    private string assigned_player;     //String with the name of the assigned player to this stone
    private bool enable_kick_action;    //Bool to know if the player can kick the stone

    private bool stone_hitted;          //Bool to know if the stone that was in the target was hitted
    private int perfectTiming;

    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<---------------------EXTERNAL CONTROLLERS------------------------->
    private DataExtractorCSVLevel1 data_extractor;
    private TimeManager timeManager;
    private int timer;

    // Particle GameObject
    [SerializeField] private GameObject particle;

    private Color stoneColor;           //Color of the stones
    private Material particleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        data_extractor = FindObjectOfType<DataExtractorCSVLevel1>();
        timeManager = FindObjectOfType<TimeManager>();

        timer = timeManager.CreateTimer();
        timeManager.PauseTimer(timer);

        DisableKickAction(); ResetStoneHitted();

        perfectTiming = 0;
    }

    //<----------------------METHODS THAT USES THE TARGET SCRIPT-------------------------->

    //<----------------------COLLISIONS-------------------------->

    //for data extraction
    void OnTriggerEnter(Collider collider)
    {
        //bool player_kicked_at_correct_time = (collider.gameObject.name == assigned_player) && CanKick() && !StoneHasBeenHitted();
        bool player_kicked_at_bad_time = (collider.gameObject.name == assigned_player) && !CanKick() && !StoneHasBeenHitted();

        bool final_position_reached = (collider.gameObject.tag == "Target") && (collider.gameObject.name == this.gameObject.name);

        //if (player_kicked_at_correct_time) { data_extractor.WriteDataLineStoneSteppingFinalPosition(this.gameObject.name, DataExtractorCSV.ACTIVE); }

        if (player_kicked_at_bad_time) { data_extractor.WriteDataLineStoneSteppingThrowing(this.gameObject.name); }

        if (final_position_reached) { data_extractor.WriteDataLineStoneFinalPosition(this.gameObject.name); timeManager.ResumeTimer(timer); }
    }

    //another way to trigger
    void OnTriggerStay(Collider collider)
    {
        bool player_condition = (collider.gameObject.name == assigned_player) && CanKick() && !StoneHasBeenHitted();
        bool target_condition = (collider.gameObject.tag == "Target") && (collider.gameObject.name == this.gameObject.name);

        if (player_condition)
        {
            if(timeManager.WaitTime(timer, CheckboxManager.seconds_to_perfect_timing.x))
            {
                if (!timeManager.WaitTime(timer, CheckboxManager.seconds_to_perfect_timing.y))
                {
                    if (CheckboxManager.perfectTimingParticles) { GenerateParticleTrial(); }
                }
            }

            StoneHitted();
        }

        if (target_condition) { EnableKickAction(); }
    }

    void OnTriggerExit(Collider collider)
    {
        bool target_condition = (collider.gameObject.tag == "Target") && (collider.gameObject.name == this.gameObject.name);

        if (target_condition) { DisableKickAction(); ResetStoneHitted(); }
    }

    private void GenerateParticleTrial()
    {
        GameObject instantiated1 = Instantiate(particle, this.transform.position, Quaternion.identity);
        GameObject instantiated2 = Instantiate(particle, this.transform.position, Quaternion.identity);

        Vector3 direction = this.transform.position - new Vector3(50f,0f,50f);
        direction.y = 0f;

        instantiated1.GetComponent<Particle>().direction = direction.normalized;
        instantiated1.GetComponent<Particle>().particleColor = stoneColor;
        instantiated1.GetComponent<Particle>().particleMaterial = particleMaterial;
        instantiated2.GetComponent<Particle>().direction = direction.normalized;
        instantiated2.GetComponent<Particle>().particleColor = stoneColor;
        instantiated2.GetComponent<Particle>().particleMaterial = particleMaterial;
        instantiated2.GetComponent<Particle>().sign = -1;

        //We change the value of perfect timing in case the stone generates particles
        perfectTiming = 1;
    }

    //<----------------------MANAGEMENT OF THIS TARGET-------------------------->

    private void ChangeKickActionStatus(bool status) { enable_kick_action = status; }

    private void EnableKickAction() { ChangeKickActionStatus(ACTIVE); }
    public void DisableKickAction() { ChangeKickActionStatus(DISABLED); }

    private void ChangeStoneHittedStatus(bool status) { stone_hitted = status; }

    private void StoneHitted() { ChangeStoneHittedStatus(ACTIVE); }
    public void ResetStoneHitted() { ChangeStoneHittedStatus(DISABLED); }

    //<----------------------METHODS TO USE THE TARGET SCRIPT-------------------------->
    public void SetAssignedPlayer(string player) { assigned_player = player; }      //method to set an assigned player
    public void SetStoneColor(Color color) { stoneColor = color; }
    public void SetParticleMaterial(Material material) { particleMaterial = material; }

    public bool CanKick() { return enable_kick_action; }                            //method to know if the kick action has been enabled

    public bool StoneHasBeenHitted() { return stone_hitted; }                       //method to know if the stone has been hitted

    public int GetPerfectTiming() { return perfectTiming; }
}
