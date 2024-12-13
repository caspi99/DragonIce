using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    //<--------------------VARIABLE TO CONTROL SWIRL IN SEQUENCE MANAGER------------------------->
    bool swirl = false;

    //<--------------------CONTROL INDEXES------------------------->
    const int ACTIVE = 1;
    const int DISABLED = 0;
    const int FINISHED = -1;

    //<--------------------STONES INDEXES------------------------->
    private const int NUM_OF_STONES = 4;

    //<--------------------ROUND TYPE INDEXES------------------------->
    public const int TRAINING_ROUNDS = 0;
    public const int TUTORIAL_ROUNDS = 1;
    public const int TRUE_ROUNDS = 2;

    //<--------------------ROUND NUMBER VARIABLES------------------------->
    private int number_of_rounds;
    private int current_round;
    private int last_finished_round;

    //<--------------------ROUND INFO VARIABLES------------------------->
    //seconds given to kick a stone
    private List<float> first_round_seconds_to_kick = new List<float>();
    private List<float> last_round_seconds_to_kick = new List<float>();

    //seconds between rounds
    private List<float> seconds_between_rounds_min = new List<float>();
    private List<float> seconds_between_rounds_max = new List<float>();

    private List<float> seconds_between_rounds_for_stone = new List<float>();

    //<--------------------ROUND STATUS CONTROL------------------------->
    private List<int> round_being_played = new List<int>();

    //<---------------------TIMER VARIABLE------------------------->
    private List<int> timers = new List<int>();

    //<--------------------CONTROLLERS------------------------->
    private InteractionController ic;            //interaction controller
    private TimeManager time_c;                  //time controller
    private DataExtractorCSVLevel1 data_extractor;     //data extractor controller

    // Start is called before the first frame update
    void Start()
    {
        ic = GameObject.Find("InteractionController").GetComponent<InteractionController>();
        time_c = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel1>();

        ic.InitInteraction();

        first_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_training);
        first_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_tutorial.y);
        first_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_true.y);

        last_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_training);
        last_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_tutorial.x);
        last_round_seconds_to_kick.Add(CheckboxManager.seconds_to_kick_true.x);


        seconds_between_rounds_min.Add(CheckboxManager.time_between_previous_stone_training);
        seconds_between_rounds_min.Add(CheckboxManager.time_between_previous_stone_tutorial_interval.x);
        seconds_between_rounds_min.Add(CheckboxManager.time_between_previous_stone_true_interval.x);

        seconds_between_rounds_max.Add(CheckboxManager.time_between_previous_stone_training);
        seconds_between_rounds_max.Add(CheckboxManager.time_between_previous_stone_tutorial_interval.y);
        seconds_between_rounds_max.Add(CheckboxManager.time_between_previous_stone_true_interval.y);

        GetNumberOfRounds();

        InitRoundControl();

        InitTimers();

        InitSecondsBetweenStones();
    }

    //<----------------------TIMER METHODS-------------------------->

    //method to init the timer
    private void InitTimers()
    {
        for (int i = timers.Count; i < NUM_OF_STONES; i++) { timers.Add(time_c.CreateTimer()); time_c.PauseTimer(timers[i]); }
    }

    //method to reset the timer
    private void ResetTimer(int stone_idx) { time_c.ResetTimer(timers[stone_idx]); }

    //method to count the seconds of the stones without moving
    private bool WaitTime(float time, int stone_idx) { return time_c.WaitTime(timers[stone_idx], time); }


    //<----------------------METHODS FOR DATA EXTRACTION-------------------------->

    public (int, int) GetAssignedPlayerAndCurrentRound(string stone_name) { return ic.GetAssignedPlayerAndCurrentRound(stone_name); }
    public (float, float) GetCurrentStonePosition(int stone, int round) { return ic.GetCurrentStonePosition(stone, round); }
    public (float, float, float, float) GetAngleDistanceXYTuple(string identifier) { return ic.GetAngleDistanceXYTuple(identifier); }

    public float GetStoneSpeed(int stone, int round) { return ic.GetStoneSpeed(stone, round); }

    public int GetStoneOrder(int stone_idx, int round_idx)
    {
        List<(int Stone, float Seconds)> stone_seconds = new List<(int Stone, float Seconds)>();

        for(int i = 0; i < NUM_OF_STONES; i++)
        {
            int idx = i + round_idx * NUM_OF_STONES;

            float seconds = seconds_between_rounds_for_stone[idx];

            stone_seconds.Add((i, seconds));
        }

        stone_seconds.Sort((e1,e2) => e1.Seconds.CompareTo(e2.Seconds));

        for (int i = 0; i < NUM_OF_STONES; i++)
        {
            if(stone_seconds[i].Stone == stone_idx) { return i; }
        }

        return -1;
    }


    //<----------------------METHODS THAT USES THE ROUND CONTROLLER-------------------------->

    //method to init the round control bool list
    private void InitRoundControl()
    {
        for (int i = 0; i < number_of_rounds * NUM_OF_STONES; i++)
        {
            round_being_played.Add(DISABLED);
        }
    }

    //method to init the seconds between stones list
    private void InitSecondsBetweenStones()
    {
        for (int i = 0; i < number_of_rounds * NUM_OF_STONES; i++)
        {
            int round_idx = i / NUM_OF_STONES;

            if (Settings.sync) { seconds_between_rounds_for_stone.Add(CalculateNextSecondsBetweenStones(round_idx)); }
            else { seconds_between_rounds_for_stone.Add(GetRandomBetweenStonesSeconds(round_idx)); }
        }

        if (Settings.sync)
        {
            for(int i = 0; i < NUM_OF_STONES; i++)
            {
                seconds_between_rounds_for_stone[i] = 0.0f;
            }

            if (CheckboxManager.firstStonesNotDestroyed)
            {
                for (int i = NUM_OF_STONES; i < NUM_OF_STONES * (CheckboxManager.numberOfStonesNotDestroyed+1); i++)
                {
                    seconds_between_rounds_for_stone[i] = 1.0f;
                }
            }

            if (CheckboxManager.enableTrainingRounds)
            {
                for(int i = NUM_OF_STONES*CheckboxManager.number_of_training_rounds; i < NUM_OF_STONES * (CheckboxManager.number_of_training_rounds+1); i++)
                {
                    seconds_between_rounds_for_stone[i] = 0.0f;
                }
            }
        }
        else
        {
            int stone_decision = Random.Range(0, NUM_OF_STONES);

            if (CheckboxManager.firstStonesNotDestroyed)
            {
                for (int i = 1; i < CheckboxManager.numberOfStonesNotDestroyed + 1; i++)
                {
                    seconds_between_rounds_for_stone[stone_decision + (NUM_OF_STONES*i)] = 0.0f;
                }
            }

            seconds_between_rounds_for_stone[stone_decision] = 0.0f;

            CheckLastAndFirstCondition();
        }
    }

    //method to calculate the next time between stones
    private float CalculateNextSecondsBetweenStones(int round_idx)
    {
        int round_type = CheckRoundType(round_idx);

        if(round_type == TRAINING_ROUNDS) { return CheckboxManager.time_between_previous_stone_training; }

        int num_of_rounds = CheckboxManager.number_of_tutorial_rounds;

        if ((round_type == TRUE_ROUNDS) || (num_of_rounds <= 0)) { num_of_rounds = CheckboxManager.number_of_true_rounds; }

        float delta = (seconds_between_rounds_max[round_type] - seconds_between_rounds_min[round_type]) / num_of_rounds;

        int local_round_idx = round_idx;

        if (round_type == TUTORIAL_ROUNDS) { local_round_idx -= CheckboxManager.number_of_training_rounds; }
        if (round_type == TRUE_ROUNDS) { local_round_idx -= CheckboxManager.numOfRoundsPrevToTruePhase; }

        return seconds_between_rounds_max[round_type] - delta * local_round_idx;
    }

    //method to get a random quantity of seconds (returns float)
    private float GetRandomBetweenStonesSeconds(int round_idx)
    {
        int round_type = CheckRoundType(round_idx);

        return Random.Range(seconds_between_rounds_min[round_type], seconds_between_rounds_max[round_type]);
    }

    //method to swap to int
    private (float, float) SwapFloats(float f1, float f2) { return (f2, f1); }

    //method to change time if the player is going to receive in a round the last stone and in the next round the first stone
    private void CheckLastAndFirstCondition()
    {
        for(int i = 1; i < number_of_rounds; i++)
        {
            int min_idx = GetMinTimeBetweenStonesIdx(i);
            int max_prev_idx = GetMaxTimeBetweenStonesIdx(i - 1);

            if(min_idx == max_prev_idx)
            {
                int idx1 = min_idx;
                int idx2 = min_idx + 1;

                if (idx1 > 0) { idx2 = min_idx - 1; }

                (seconds_between_rounds_for_stone[idx1], seconds_between_rounds_for_stone[idx2]) = SwapFloats(seconds_between_rounds_for_stone[idx1], seconds_between_rounds_for_stone[idx2]);
            }
        }

        /*
        for (int i = 1; i < number_of_rounds; i++)
        {
            int last_stone_idx_prev = -1;
            int first_stone_idx_curr = -1;

            float max_time_stone_prev_round = -1.0f;
            float min_time_stone_curr_round = 10000.0f;

            for (int j = 0; j < NUM_OF_STONES; j++)
            {
                int prev_idx = ((i - 1) * NUM_OF_STONES) + j;
                int curr_idx = (i * NUM_OF_STONES) + j;

                float prev_seconds = seconds_between_rounds_for_stone[prev_idx];
                float curr_seconds = seconds_between_rounds_for_stone[curr_idx];

                if (max_time_stone_prev_round < prev_seconds)
                {
                    max_time_stone_prev_round = prev_seconds;
                    last_stone_idx_prev = j;
                }

                if (min_time_stone_curr_round > curr_seconds)
                {
                    min_time_stone_curr_round = curr_seconds;
                    first_stone_idx_curr = j;
                }
            }

            if(last_stone_idx_prev == first_stone_idx_curr)
            {
                int idx1 = last_stone_idx_prev;
                int idx2 = last_stone_idx_prev + 1;

                if (idx1 > 0) { idx2 = last_stone_idx_prev - 1; }

                (seconds_between_rounds_for_stone[idx1], seconds_between_rounds_for_stone[idx2]) = SwapFloats(seconds_between_rounds_for_stone[idx1], seconds_between_rounds_for_stone[idx2]);
            }
        }
        */
    }

    //method to check if the current round is tutorial or true round
    public int CheckRoundType(int round_idx)
    {
        if (round_idx < CheckboxManager.numOfRoundsPrevToTruePhase)
        {
            if(CheckboxManager.enableTrainingRounds && (round_idx < CheckboxManager.number_of_training_rounds))
            {
                return TRAINING_ROUNDS;
            }
            else
            {
                return TUTORIAL_ROUNDS;
            }
        }
        else { return TRUE_ROUNDS; }
    }

    //method to get the number of round from the checkboxes
    private void GetNumberOfRounds()
    {
        number_of_rounds = CheckboxManager.num_of_rounds;

        current_round = 0;

        last_finished_round = 0;
        //last_finished_round = CheckboxManager.number_of_tutorial_rounds;
    }

    //method to update the round counter
    private void UpdateRoundCounter() { current_round++; }

    //method to update the last finished round counter
    private bool UpdateLastFinishedRoundCounter()
    {
        bool condition = ic.GetIfRoundsFinished(last_finished_round) && ic.GetIfStonesExploded(last_finished_round);

        if (condition) { last_finished_round++; }

        return condition;
    }

    //method to calculate the next time to kick a stone
    private float CalculateNextSecondsToKick(int round_idx)
    {
        int round_type = CheckRoundType(round_idx);

        if (round_type == TRAINING_ROUNDS) { return CheckboxManager.seconds_to_kick_training; }

        int num_of_rounds = CheckboxManager.number_of_tutorial_rounds;

        if ((round_type == TRUE_ROUNDS) || (num_of_rounds <= 0)) { num_of_rounds = CheckboxManager.number_of_true_rounds; }

        float delta = (first_round_seconds_to_kick[round_type] - last_round_seconds_to_kick[round_type]) / num_of_rounds;

        int local_round_idx = round_idx;

        if (round_type == TUTORIAL_ROUNDS) { local_round_idx -= CheckboxManager.number_of_training_rounds; }
        if (round_type == TRUE_ROUNDS) { local_round_idx -= CheckboxManager.numOfRoundsPrevToTruePhase; }

        return first_round_seconds_to_kick[round_type] - delta * local_round_idx;
    }

    /*
    //method to know the global seconds between stones of a stone in a round
    private float GetGlobalSecondsBetweenStones(int stone_idx, int round_idx)
    {
        float result = 0.0f;

        for(int i = 0; i <= round_idx; i++)
        {
            int idx = stone_idx + (i * NUM_OF_STONES);

            result += seconds_between_rounds_for_stone[idx];
        }

        return result;
    }

    //method to know the minimum global seconds between stones of a stone in a round (swirl)
    private float GetMinGlobalSecondsBetweenStones(int round_idx)
    {
        float min_result = 10000.0f;

        for (int i = 0; i < NUM_OF_STONES; i++)
        {
            float seconds = GetGlobalSecondsBetweenStones(i, round_idx);

            if(min_result > seconds) { min_result = seconds; }
        }

        return min_result;
    }
    */

    //method to know the first stone thrown in every round
    private int GetFirstStoneThrown(int round_idx)
    {
        float minimum_seconds_between_stone = 100000.0f;
        int stone_idx = -1;

        for(int i = round_idx*NUM_OF_STONES; i < (round_idx + 1) * NUM_OF_STONES; i++)
        {
            if(minimum_seconds_between_stone > seconds_between_rounds_for_stone[i])
            {
                minimum_seconds_between_stone = seconds_between_rounds_for_stone[i];
                stone_idx = i % NUM_OF_STONES;
            }
        }

        return stone_idx;
    }

    //method to get the min time of between stones given a round
    private int GetMinTimeBetweenStonesIdx(int round_idx)
    {
        float min_time_between_stone = 1000.0f;
        int idx = -1;

        for(int i = round_idx*NUM_OF_STONES; i < (round_idx + 1) * NUM_OF_STONES; i++)
        {
            if (min_time_between_stone > seconds_between_rounds_for_stone[i])
            {
                min_time_between_stone = seconds_between_rounds_for_stone[i];
                idx = i;
            }
        }

        return idx;
    }

    //method to get the max time of between stones given a round
    private int GetMaxTimeBetweenStonesIdx(int round_idx)
    {
        float max_time_between_stone = -1.0f;
        int idx = -1;

        for (int i = round_idx * NUM_OF_STONES; i < (round_idx + 1) * NUM_OF_STONES; i++)
        {
            if (max_time_between_stone < seconds_between_rounds_for_stone[i])
            {
                max_time_between_stone = seconds_between_rounds_for_stone[i];
                idx = i;
            }
        }

        return idx;
    }

    //method to get the running current round
    private int GetCurrentRoundRunning()
    {
        int running_round = current_round;

        for(int i = last_finished_round; i <= current_round; i++)
        {
            for(int j = 0; j < NUM_OF_STONES; j++)
            {
                if (IsRoundPlaying(i, j)) { running_round = i; }
            }
        }

        return running_round;
    }

    //method to run a round
    private void RunRound(int round_idx, int stone_idx)
    {
        if (ic.ManageRunRound(CalculateNextSecondsToKick(round_idx), round_idx, CheckRoundType(round_idx), stone_idx))
        {
            FinishRoundPlaying(round_idx, stone_idx);
        }
    }

    //method to prepare a round
    private void PrepareRound(int round_idx, int stone_idx)
    {
        if(WaitTime(seconds_between_rounds_for_stone[stone_idx + round_idx * NUM_OF_STONES], stone_idx))
        {
            if (ic.ManagePrepareRound(round_idx, stone_idx))
            {
                if (stone_idx == GetFirstStoneThrown(round_idx)) { swirl = true; }  //we enable swirl

                ResetTimer(stone_idx);

                EnableRoundPlaying(round_idx, stone_idx);

                data_extractor.WriteDataLineStoneThrowing(stone_idx, round_idx); //data extraction
            }
        }
    }

    //method to play a round by index
    private void PlayRound(int round_idx)
    {
        //In case it is a training round or the first round after the training round
        bool roundTrainingCondition = CheckboxManager.enableTrainingRounds && 
            ((CheckRoundType(round_idx) == TRAINING_ROUNDS) || ((round_idx == CheckboxManager.number_of_training_rounds)));

        //Training play round
        if (roundTrainingCondition)
        {
            bool prepareRoundCondition = Input.GetKey(KeyCode.T);  //Only enable the next round if a key is pressed
            for (int i = 0; i < NUM_OF_STONES; i++)
            {
                if (IsRoundPlaying(round_idx, i) || IsRoundFinished(round_idx, i)) { RunRound(round_idx, i); }
                else if (IsRoundDisabled(round_idx, i) && prepareRoundCondition) { PrepareRound(round_idx, i); }

                //Only launch the next stone if the rest of stones have been launched or if it is the first stone after training
                prepareRoundCondition &= (IsRoundFinished(round_idx, i) || (round_idx == CheckboxManager.number_of_training_rounds));
            }
        }
        //Other cases
        else
        {
            for (int i = 0; i < NUM_OF_STONES; i++)
            {
                if (IsRoundPlaying(round_idx, i) || IsRoundFinished(round_idx, i)) { RunRound(round_idx, i); }
                else if (IsRoundDisabled(round_idx, i)) { PrepareRound(round_idx, i); }
            }
        }

        if (UpdateLastFinishedRoundCounter())
        {
            ic.CheckAndPlayNextRoundSound(round_idx);
        }
    }

    //method to update the current_round counter in a controlled way
    private void ControlledCurrentRoundUpdater()
    {
        //to wait until all the tutorial rounds are finished
        if ((current_round == CheckboxManager.number_of_tutorial_rounds - 1) && CheckboxManager.wait_until_famirialitzation_ends)
        {
            if (ic.CheckIfTutorialRoundsEnd()) { UpdateRoundCounter(); }
        }
        //to wait until all the true rounds are finished
        else if (current_round == number_of_rounds - 1)
        {
            if (ic.CheckIfTrueRoundsEnd() && ic.GetIfStonesExploded(current_round)) { UpdateRoundCounter(); }
        }
        else
        {
            bool condition = true;

            for(int i = 0; i < NUM_OF_STONES; i++)
            {
                condition = condition && !IsRoundDisabled(current_round, i);
            }

            //to update the round number, waiting the necessary seconds
            if (condition) { UpdateRoundCounter(); }
        }
    }

    //method to start a round
    private void StartRoundStones()
    {
        /*
        //first for that executes the famirialization phase rounds
        for (int i = 0; i <= Mathf.Min(current_round, CheckboxManager.number_of_tutorial_rounds - 1); i++)
        {
            PlayRound(i);
        }
        */

        //second for that executes the true phase rounds
        for (int i = last_finished_round; i <= current_round; i++)
        {
            // CONDITION TO CHECK IF ALL THE STONES OF THE NOT DESTROYED ROUNDS HAVE BEEN STEPPED
            bool roundEndCondition = true;

            for(int j = 0; j < NUM_OF_STONES * CheckboxManager.numberOfStonesNotDestroyed; j++)
            {
                roundEndCondition &= IsRoundFinished(0, j);
            }

            //Case when it has to wait to the first stones are destroyed
            if((i >= CheckboxManager.numberOfStonesNotDestroyed) && CheckboxManager.firstStonesNotDestroyed && roundEndCondition)
            {
                PlayRound(i);
            }
            //Case when it does not have to wait the first stones to be destroyed
            else if (!CheckboxManager.firstStonesNotDestroyed)
            {
                PlayRound(i);
            }
            //Case when it plays the round with first stones destroyed
            else if ((i < CheckboxManager.numberOfStonesNotDestroyed) && CheckboxManager.firstStonesNotDestroyed)
            {
                //Condition to see if the previous rounds have finished or not
                bool playRoundCondition = true;

                for(int j = 0; j < NUM_OF_STONES * i; j++)
                {
                    playRoundCondition &= IsRoundFinished(0, j);
                }

                if (playRoundCondition) { PlayRound(i); }
            }
        }
    }

    //<----------------------METHODS TO USE THE ROUND CONTROLLER-------------------------->

    //method to know if the dragon has to swirl
    public bool GetIfSwirl() { return swirl; }
    public void SetSwirl(bool status) { swirl = status; }

    //method to know if all the rounds have finished
    public bool StillRounds() { return number_of_rounds > current_round; }

    //method to get the number of rounds left
    //public int GetNumRemainingRounds() { return number_of_rounds; }

    //methods to get the current round status
    public bool IsRoundPlaying(int round_idx, int stone_idx) { return round_being_played[stone_idx + round_idx * NUM_OF_STONES] == ACTIVE; }
    public bool IsRoundDisabled(int round_idx, int stone_idx) { return round_being_played[stone_idx + round_idx * NUM_OF_STONES] == DISABLED; }
    public bool IsRoundFinished(int round_idx, int stone_idx) { return round_being_played[stone_idx + round_idx * NUM_OF_STONES] == FINISHED; }

    //method to change the round status
    private void ChangeRoundStatus(int status, int round_idx, int stone_idx) { round_being_played[stone_idx + round_idx * NUM_OF_STONES] = status; }

    public void EnableRoundPlaying(int round_idx, int stone_idx) { ChangeRoundStatus(ACTIVE, round_idx, stone_idx); }
    public void DisableRoundPlaying(int round_idx, int stone_idx) { ChangeRoundStatus(DISABLED, round_idx, stone_idx); }
    public void FinishRoundPlaying(int round_idx, int stone_idx) { ChangeRoundStatus(FINISHED, round_idx, stone_idx); }

    //method to start a round
    public void StartRound()
    {
        bool reproduceGameplayNarrative = true;

        //Only in case training rounds are enabled and the current round is the next stone after training phase
        if (CheckboxManager.enableTrainingRounds && (GetCurrentRoundRunning() == CheckboxManager.number_of_training_rounds))
        { reproduceGameplayNarrative = Input.GetKey(KeyCode.T); }

        if (ic.ReproduceGameplayNarrative(CheckRoundType(GetCurrentRoundRunning()), reproduceGameplayNarrative))
        {
            StartRoundStones();

            ControlledCurrentRoundUpdater();
        }
    }

    //method to get the current round time between stones
    public float GetTimeBetweenStonesForSwirl(int num_swirl)
    {
        if ((num_swirl > 0) && (num_swirl < number_of_rounds))
        {
            int min_idx = GetMinTimeBetweenStonesIdx(num_swirl);

            int max_prev_idx = GetMaxTimeBetweenStonesIdx(num_swirl - 1);
            int min_prev_idx = GetMinTimeBetweenStonesIdx(num_swirl - 1);

            float min_time_next_round = seconds_between_rounds_for_stone[min_idx];

            if (num_swirl > 1)
            {
                min_time_next_round += (seconds_between_rounds_for_stone[max_prev_idx] - seconds_between_rounds_for_stone[min_prev_idx]);
            }
            else
            {
                min_time_next_round += seconds_between_rounds_for_stone[max_prev_idx];
            }

            return min_time_next_round;
        }
        else
        {
            return 7.50f;
        }
    }

    //method to get if true phase started
    public bool GetIfFamiliarizationPhaseStarted() { return current_round >= CheckboxManager.number_of_training_rounds; }
    public bool GetIfTruePhaseStarted() { return current_round >= CheckboxManager.numOfRoundsPrevToTruePhase; }
}
