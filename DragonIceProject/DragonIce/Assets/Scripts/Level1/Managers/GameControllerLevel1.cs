using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerLevel1 : GameController
{
    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<--------------------START PHASE CONTROL (FOR DATA EXTRACTOR)------------------------->
    private bool[] phase_control = new bool[] { ACTIVE, ACTIVE, ACTIVE, ACTIVE, ACTIVE };

    //<------------------------------------------------------------->
    //<-------------------VARIABLES DECLARATION--------------------->
    //<------------------------------------------------------------->

    private GameplayControllerLevel1 gameplay_controller;

    private AnimationController animation_controller;
    private DataExtractorCSVLevel1 data_extractor;

    //int to count the swirls the dragon has made
    private int num_swirl;

    private void Awake()
    {
        // We set the values of the variables
        if (!CheckboxManager.enableTrainingRounds) { CheckboxManager.number_of_training_rounds = 0; }
        CheckboxManager.num_of_rounds = CheckboxManager.number_of_training_rounds + CheckboxManager.number_of_tutorial_rounds + CheckboxManager.number_of_true_rounds;
        CheckboxManager.numOfRoundsPrevToTruePhase = CheckboxManager.number_of_training_rounds + CheckboxManager.number_of_tutorial_rounds;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindControllers();

        SkipLevel1();
        StartGameplayController();
        InitGameplayControlValues(DISABLED, DISABLED);
        ChangeBloomIntensity(0.5f);

        num_swirl = 0;
    }

    // Update is called once per frame
    void Update()
    {
        DataExtractorCSVUpdater();

        GetIfGameplayFinished();

        RunStorySequences();

        RunGameplay();

        EndLevel1();

        //(DOESN'T WORK)
        /*
        if (Input.GetKeyDown("k"))
        {
            sequence_manager.ReturnToIdle();
            CheckboxManager.skip_story_sequences = true;
        }
        */

    }

    //<----------------------DATA EXTRACTOR CSV METHODS-------------------------->
    protected void UpdatePhaseData(int phase_idx)
    {
        data_extractor.WriteDataLinePhaseStart(phase_idx);
        data_extractor.SetPhase(phase_idx);

        phase_control[phase_idx] = DISABLED;
    }

    protected override void DataExtractorCSVUpdater()
    {
        //START OF INTRODUCTION PHASE
        if (phase_control[DataExtractorCSVLevel1.INTRODUCTION])
        {
            UpdatePhaseData(DataExtractorCSVLevel1.INTRODUCTION);
        }

        if (CheckboxManager.enableTrainingRounds)
        {
            //START OF TRAINING PHASE
            if (GetIfAllPlayersAssigned() && phase_control[DataExtractorCSVLevel1.TRAINING])
            {
                UpdatePhaseData(DataExtractorCSVLevel1.TRAINING);
            }

            //START OF FAMILIARIZATION PHASE
            if (GetIfFamiliarizationPhaseStarted() && phase_control[DataExtractorCSVLevel1.FAMILIARIZATION])
            {
                UpdatePhaseData(DataExtractorCSVLevel1.FAMILIARIZATION);
            }
        }
        else
        {
            //START OF FAMILIARIZATION PHASE
            if (GetIfAllPlayersAssigned() && phase_control[DataExtractorCSVLevel1.FAMILIARIZATION])
            {
                UpdatePhaseData(DataExtractorCSVLevel1.FAMILIARIZATION);
            }
        }

        //START OF GAME PHASE
        if (GetIfTruePhaseStarted() && phase_control[DataExtractorCSVLevel1.GAME])
        {
            UpdatePhaseData(DataExtractorCSVLevel1.GAME);
        }

        //START OF END PHASE
        if (gameplay_finished && phase_control[DataExtractorCSVLevel1.END])
        {
            UpdatePhaseData(DataExtractorCSVLevel1.END);

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

    //<----------------------METHODS THAT USES THE GAME CONTROLLER-------------------------->

    protected override void RunStorySequences()
    {
        if (CheckboxManager.skip_story_sequences)
        {
            sequence_controller.FinishSequence(SequenceInformationLevel1.SEQUENCE_1);
            //sequence_controller.FinishSequence(SequenceInformationLevel1.SEQUENCE_2);
        }

        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_1);
        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_2);
        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_3);
        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_4);
        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_5);
    }

    protected override void RunGameplay()
    {
        //SWIRL
        bool get_if_swirl = GetIfSwirl();

        if (get_if_swirl) { num_swirl++; SetSwirl(false); }

        //TIME
        float time_between_stones = 7.5f;

        if (ingame)
        {
            float swirl_offset = -0.3f;
            time_between_stones = gameplay_controller.GetTimeBetweenStonesForSwirl(num_swirl) + swirl_offset;
        }

        if (time_between_stones <= 0.0f) { time_between_stones = 7.5f; }

        //SEQUENCE RUN
        sequence_controller.ExecuteSequence(SequenceInformationLevel1.SEQUENCE_INGAME, time_between_stones, GetIfAllPlayersAssigned(), gameplay_finished, get_if_swirl);
        ingame = sequence_controller.ReadyToPlaySequence(SequenceInformationLevel1.SEQUENCE_INGAME) && !gameplay_finished;

        //RUN GAMEPLAY
        if (ingame) { gameplay_controller.Gameplay(CheckDragonAnimationAlignment()); }

        //END GAMEPLAY
        if (gameplay_finished && CheckDragonAnimationAlignment())
        {
            gameplay_controller.EndGameplayController();
            sequence_controller.FinishSequence(SequenceInformationLevel1.SEQUENCE_INGAME);
        }
    }

    //method to know if rounds have finished
    protected override void GetIfGameplayFinished() { gameplay_finished = !gameplay_controller.StillRounds(); }

    protected override void FindControllers()
    {
        base.FindControllers();

        gameplay_controller = GameObject.Find("GameplayController").GetComponent<GameplayControllerLevel1>();
        animation_controller = GameObject.Find("AnimationController").GetComponent<AnimationController>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel1>();
    }

    private void SkipLevel1()
    {
        if (!Settings.play_level_1) { SceneManager.LoadScene("Level2"); }
    }
    private void StartGameplayController() { gameplay_controller.InitGameplayController(); }            //method to start the gameplay controller

    private bool CheckDragonAnimationAlignment()
    {
        return animation_controller.CheckIfAnimationIsNearToFinish(AnimationInformationLevel1.DRAGON);
    }

    private void EndLevel1()
    {
        if (sequence_controller.GetIfSequencesFinished())
        {
            if (!Settings.play_level_2) { Application.Quit(); }
            else { SceneManager.LoadScene("Level2"); }
        }
    }

    //method to know if the dragon has to swirl
    private bool GetIfSwirl() { return gameplay_controller.GetIfSwirl(); }
    public void SetSwirl(bool status) { gameplay_controller.SetSwirl(status); }    //method to know if the dragon has to swirl
    private bool GetIfFamiliarizationPhaseStarted() { return gameplay_controller.GetIfFamiliarizationPhaseStarted(); }
    private bool GetIfTruePhaseStarted() { return gameplay_controller.GetIfTruePhaseStarted(); }

    //method to know if all players assigned
    private bool GetIfAllPlayersAssigned() { return gameplay_controller.GetIfAllPlayersAssigned(); }

    //<----------------------METHODS TO USE THE GAME CONTROLLER-------------------------->

}
