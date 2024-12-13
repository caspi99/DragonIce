using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControllerLevel1 : MonoBehaviour
{
    //<----------------------VARIABLES TO CONTROL THE START MARKS-------------------------->
    private List<bool> players_assigned = new List<bool>();             //list to control if players had been assigned in all the controllers
    private List<string> assigned_players = new List<string>();         //list of the assigned players order

    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<--------------------CONTROLLERS INDEXES------------------------->
    const int NUM_OF_CONTROLLERS = 3;   //this isn't exactly the num of controllers, it is the num of controllers afected by assigned_players

    const int START_MARKS = 0;
    const int TARGETS = 1;
    const int STONES = 2;

    //<--------------------CONTROLLERS------------------------->

    private StartMarksController sm;             //start marks controller

    private TargetGenerator tcg;                 //targets generator
    private NewTargetController tc;                 //targets controller

    private StoneGenerator stg;                  //stone generator
    private NewStoneController stc;              //stone controller

    private TimeManager tm;
    private RoundController rc;                  //round controller

    private SoundController sc;                  //sound controller

    private PluginConnector pc;                  //plugin connector

    //<----------------------METHODS THAT USES THE GAMEPLAY CONTROLLER-------------------------->

    //<----------------------PRINCIPAL GAMEPLAY METHOD-------------------------->
    public void Gameplay(bool dragon_alignment)
    {
        ManageStartMarks(dragon_alignment);         //this method manage all the instructions related to the start marks
        ManageTargets();            //this method manage all the instructions related to the targets
        ManageStones();             //this method manage all the instructions related to the stones

        ManageRounds();             //this method manage all the instructions related to the rounds
    }

    //<----------------------METHODS TO MANAGE THE PLAYERS ASSIGNED BOOL LIST-------------------------->

    //method to init the players assigned bool list
    private void InitPlayersAssigned()
    {
        for (int i = 0; i < NUM_OF_CONTROLLERS; i++) { players_assigned.Add(DISABLED); }
    }

    //method to get a specific players assigned bool
    private bool PlayersAssignedAt(int controller_idx) { return players_assigned[controller_idx]; }

    //method to set a value to a specific players assigned bool
    private void ChangePlayersAssignedAt(int controller_idx, bool status) { players_assigned[controller_idx] = status; }

    private void MarkAsAssignedPlayersAt(int controller_idx) { ChangePlayersAssignedAt(controller_idx, ACTIVE); }
    private void MarkAsNotAssignedPlayersAt(int controller_idx) { ChangePlayersAssignedAt(controller_idx, DISABLED); }


    //<----------------------METHODS TO CONTROL THE START MARKS-------------------------->
    private void EnableStartMarks() { sm.ActivateStartMarks(); }
    private void DisableStartMarks() { sm.DeactivateStartMarks(); }

    private bool ReadyToStart() { return sm.ReadyToStart(); }
    private List<string> GetPlayersStartPos() { return sm.GetPlayersStartPos(); }

    /*
    //method to show the start marks at the start of the gameplay
    private void ShowStartMarksAtStart()
    {
        if (!PlayersAssignedAt(START_MARKS)) { EnableStartMarks(); }
    }
    */

    //method to show the startmarks colors
    private void ShowStartMarksColors()
    {
        sm.ShowStartMarksColors(Settings.tracking_player_reorder && !PlayersAssignedAt(START_MARKS));
    }

    //method to assign the players
    private void AssignPlayers()
    {
        if (Settings.tracking_player_reorder)
        {
            List<int> order = sm.GetPlayersStartOrder();
            pc.SetOrderOfTrackingPlayers(order);

            GameConstants.player_order = order.ToArray();
        }

        assigned_players = GetPlayersStartPos();
    }

    //method to assign players to start marks
    private void AssignPlayersToStartMarks(bool dragon_alignment)
    {
        bool animation_fluence = !sc.IsPlaying(SoundInformationLevel1.NARRATIVE) && dragon_alignment;  //to make the transitions cleaner

        if (!PlayersAssignedAt(START_MARKS) && ReadyToStart() && animation_fluence)
        {
            AssignPlayers();

            MarkAsAssignedPlayersAt(START_MARKS);

            sc.PlayClipOnce(SoundInformationLevel1.CONFIRMATION);   //play startmark sound
        }
    }

    //method to shutdown start marks
    private void ShutDownStartMarks() { if (PlayersAssignedAt(START_MARKS)) { sm.enabled = DISABLED; } }

    //principal method to manage the start marks
    private void ManageStartMarks(bool dragon_alignment)
    {
        //ShowStartMarksAtStart();

        ShowStartMarksColors();

        AssignPlayersToStartMarks(dragon_alignment);
        ShutDownStartMarks();
    }


    //<----------------------METHODS TO CONTROL THE TARGETS-------------------------->
    //private List<GameObject> GetTargetGameObjects() { return tc.GetTargetGameObjects(); } //method to get the target objects

    //method to assign players to targets
    private void AssignPlayersToTargets()
    {
        if (PlayersAssignedAt(START_MARKS) && !PlayersAssignedAt(TARGETS))
        {
            tcg.InitTargetGenerator(assigned_players);

            tc.InitTargetController(assigned_players);

            MarkAsAssignedPlayersAt(TARGETS);
        }
    }

    //principal method to manage the targets
    private void ManageTargets()
    {
        AssignPlayersToTargets();
    }

    //<----------------------METHODS TO CONTROL THE STONES-------------------------->

    //method to init stones
    private void InitStones()
    {
        if (PlayersAssignedAt(TARGETS) && !PlayersAssignedAt(STONES))
        {
            stg.InitStoneGenerator();
            stc.InitStoneController(tc.GetTargetGameObjects());
            MarkAsAssignedPlayersAt(STONES);

            //we deactivate targets and stones
            tc.ActivateAllTargets();
            stc.DeactivateAllStones();
        }
    }

    //principal method to manage the stones
    private void ManageStones()
    {
        InitStones();
    }


    //<----------------------METHODS TO CONTROL THE ROUNDS-------------------------->

    /*
    //method to export controllers to RoundController
    private void ExportToRoundController() {
        //rc.ImportControllersFromGameplayController(tc, stc, tm);
        rc.ImportPlayersAssigned(assigned_players);
    }
    */

    //principal method to manage the rounds
    private void ManageRounds()
    {
        bool condition = PlayersAssignedAt(START_MARKS) && PlayersAssignedAt(TARGETS) && PlayersAssignedAt(STONES);

        if (condition) { rc.StartRound(); }
    }


    //<----------------------METHODS TO USE THE GAMEPLAY CONTROLLER-------------------------->

    //method to init the Gameplay Controller
    public void InitGameplayController()
    {
        sm = GameObject.Find("StartMarksManager").GetComponent<StartMarksController>();
        tcg = GameObject.Find("TargetController").GetComponent<TargetGenerator>();
        tc = GameObject.Find("TargetController").GetComponent<NewTargetController>();
        stg = GameObject.Find("StoneController").GetComponent<StoneGenerator>();
        stc = GameObject.Find("StoneController").GetComponent<NewStoneController>();
        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        rc = GameObject.Find("RoundController").GetComponent<RoundController>();
        sc = GameObject.Find("SoundController").GetComponent<SoundController>();
        pc = GameObject.Find("PluginController").GetComponent<PluginConnector>();

        //Init players assignment control
        InitPlayersAssigned();
    }

    //method to end the Gameplay Controller
    public void EndGameplayController()
    {
        //Targets
        tc.DeactivateAllTargets();

        //Stones
        stc.DeactivateAllStones();
    }

    public bool GetIfSwirl() { return rc.GetIfSwirl(); }    //method to know if the dragon has to swirl
    public void SetSwirl(bool status) { rc.SetSwirl(status); }    //method to know if the dragon has to swirl
    public bool StillRounds() { return rc.StillRounds(); }    //method to know if rounds have finished

    public bool GetIfFamiliarizationPhaseStarted() { return rc.GetIfFamiliarizationPhaseStarted(); }
    public bool GetIfTruePhaseStarted() { return rc.GetIfTruePhaseStarted(); }

    //method to get the current round time between stones
    public float GetTimeBetweenStonesForSwirl(int num_swirl) { return rc.GetTimeBetweenStonesForSwirl(num_swirl); }

    //method to know if all players assigned
    public bool GetIfAllPlayersAssigned()
    {
        return PlayersAssignedAt(START_MARKS) && PlayersAssignedAt(TARGETS) && PlayersAssignedAt(STONES);
    }
}
