using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerLevel2 : GameController
{
    //<------------------------------------------------------------->
    //<-------------------VARIABLES DECLARATION--------------------->
    //<------------------------------------------------------------->

    private GameplayControllerLevel2 gameplay_controller;

    private PlayersControllerLevel2 players_controller;
    private DataExtractorCSVLevel2 data_extractor;

    private bool data_extraction_control = true;
    private bool startLevel2 = false;

    // Start is called before the first frame update
    void Start()
    {
        FindControllers();
        InitGameplayControlValues(GameConstants.ACTIVE, GameConstants.DISABLED);
        ChangeBloomIntensity(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        ActivateLevel2();   //Method to get if P key is pressed to activate level 2

        //In case the game is activated, start the game
        if (startLevel2)
        {
            RunStorySequences();
            GetIfGameplayFinished();
            RunGameplay();
            EndLevel2();
        }
    }

    private void ActivateLevel2()
    {
        if (CheckboxManagerLevel2.skip_story_sequences || Input.GetKeyDown(KeyCode.P)) { startLevel2 = true; }
    }

    //<----------------------METHODS THAT USES THE GAME CONTROLLER-------------------------->

    protected override void DataExtractorCSVUpdater()
    {
        //END OF CAVE
        if (sequence_controller.GetIfSequencesFinished() && data_extraction_control)
        {
            data_extraction_control = false;

            //we save all the data collected at the end

            if (Settings.continuous_data_saving)
            {
                data_extractor.SetFalseDataFlow();
            }
            else
            {
                data_extractor.WriteMechanicsCSV();
                data_extractor.WritePositionTrackingsCSV();
            }
        }
    }

    protected override void FindControllers()
    {
        base.FindControllers();
        gameplay_controller = GameObject.Find("GameplayController").GetComponent<GameplayControllerLevel2>();
        players_controller = GameObject.Find("PlayersController").GetComponent<PlayersControllerLevel2>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel2>();
    }

    protected override void RunStorySequences()
    {
        /*
        if (CheckboxManagerLevel2.skip_story_sequences)
        {
            sequence_controller.FinishSequence(SequenceInformationLevel2.SEQUENCE_1);
        }
        */

        if (gameplay_controller.GetCurrentPhase() <= SequenceInformationLevel2.SEQUENCE_3)
        {
            sequence_controller.ExecuteSequence(gameplay_controller.GetCurrentPhase());
        }

        //provisional run sequence 3
        if (gameplay_finished) { sequence_controller.ExecuteSequence(SequenceInformationLevel2.SEQUENCE_3); }
    }

    protected override void RunGameplay()
    {
        if (ingame) {
            bool sequence_condition = sequence_controller.GetIfSequenceFinished(gameplay_controller.GetCurrentPhase());

            ManagePlayerVisibleTools(sequence_condition);

            if (gameplay_controller.Gameplay(sequence_condition, Input.GetKeyDown("k")))
            {
                gameplay_controller.NextPhase(sequence_condition);
            }
        }
    }

    protected override void GetIfGameplayFinished()
    {
        if (gameplay_controller.CheckIfGameplayFinished())
        {
            ingame = GameConstants.DISABLED; gameplay_finished = GameConstants.ACTIVE;
        }
    }

    private void ManagePlayerVisibleTools(bool sequence_condition)
    {
        bool player_visibility = sequence_condition && Settings.visualizePlayerPosition;
        bool rope_visibility = false;
        bool branch_visibility = false;

        if ((gameplay_controller.GetCurrentPhase() == GameConstants.PHASE_1) || (CheckboxManagerLevel2.pullTreeMechanic && (gameplay_controller.GetCurrentPhase() == GameConstants.PHASE_2)))
        {
            //rope_visibility = sequence_condition && CheckboxManagerLevel2.visualizePlayerPosition;
            rope_visibility = sequence_condition;
        }
        else if(gameplay_controller.GetCurrentPhase() == GameConstants.PHASE_2)
        {
            //branch_visibility = sequence_condition && CheckboxManagerLevel2.visualizePlayerPosition;
            branch_visibility = sequence_condition;

            players_controller.CalculateDirectionBranchOfPlayer();
        }
        
        players_controller.ChangePlayerVisibility(player_visibility);
        players_controller.ChangePlayerRopeVisibility(rope_visibility);
        players_controller.ChangePlayerBranchVisibility(branch_visibility);
    }

    private void EndLevel2() { if (sequence_controller.GetIfSequencesFinished()) { Application.Quit(); } }

}
