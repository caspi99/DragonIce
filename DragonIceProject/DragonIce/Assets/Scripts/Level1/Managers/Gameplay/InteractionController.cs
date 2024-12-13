using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractionController : MonoBehaviour
{
    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<--------------------STONES INDEXES------------------------->
    private const int NUM_OF_STONES = 4;

    //<--------------------ROUND TYPE INDEXES------------------------->
    private const int TRAINING_ROUNDS = 0;
    private const int TUTORIAL_ROUNDS = 1;
    private const int TRUE_ROUNDS = 2;

    //<------------------------STONE VARIABLES--------------------------->
    private List<bool> stones_shined = new List<bool>();   //control list see if in a round the stones have already shined

    private float light_intensity;
    private float explosion_intensity;

    private float wait_time_explosion = 1.0f;

    //<------------------------ICE CRACK VARIABLES--------------------------->
    public GameObject ice_crack_prefab;
    private List<bool> ice_crack_spawned = new List<bool>();   //control list to spawn just once every ice crack

    //<---------------------TIMER VARIABLE------------------------->
    private List<int> timers = new List<int>();
    private int narrative_timer_idx = -1;   //idx of the narrative timer
    private int explosion_effect_timer_offset = -1; //offset to calculate the timer idx for explosion effects

    //<---------------------NARRATIVE VARIABLES------------------------->
    private List<bool> wait_narrative = new List<bool>();   //control list to make the stones wait until the narrative has finished

    //<--------------------CONTROLLERS------------------------->
    private SoundController sc;                  //sound controller
    private NewSoundController nwsc;                  //sound controller
    private NewTargetController tc;              //targets controller
    private NewStoneController stc;              //stone controller
    private TimeManager time_c;                  //time controller
    private DataExtractorCSVLevel1 data_extractor;     //data extractor controller
    private IceController ice_controller;     //data extractor controller

    //method to init the interaction controller
    public void InitInteraction()
    {
        tc = GameObject.Find("TargetController").GetComponent<NewTargetController>();
        stc = GameObject.Find("StoneController").GetComponent<NewStoneController>();
        time_c = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        sc = GameObject.Find("SoundController").GetComponent<SoundController>();
        nwsc = GameObject.Find("SoundController").GetComponent<NewSoundController>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel1>();
        ice_controller = GameObject.Find("IceController").GetComponent<IceController>();

        InitTimers();
        InitNarrativeControl();
        InitStoneShineControl();
        InitIceSpawnControl();
        GetStoneLightIntensity();
    }

    //<----------------------TIMER METHODS-------------------------->

    //method to init the timer
    private void InitTimers()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        int total_num_of_timers = (num_of_rounds * NUM_OF_STONES) + (num_of_rounds * NUM_OF_STONES) + 1;

        for (int i = timers.Count; i < total_num_of_timers; i++)  //the first i is timers.Count because, if they are already created, it won't enter to the loop
        {
            timers.Add(time_c.CreateTimer());
            time_c.PauseTimer(timers[i]);
        }

        explosion_effect_timer_offset = (num_of_rounds * NUM_OF_STONES);

        narrative_timer_idx = total_num_of_timers-1; //narrative timer
    }

    //method to reset the timer
    private void ResetTimer(int round_idx, int stone_idx)
    {
        int idx = stone_idx + round_idx * NUM_OF_STONES;

        time_c.ResetTimer(timers[idx]);
    }

    //method to count the seconds of the stones without moving
    private bool WaitTimeWithoutReset(float time, int round_idx, int stone_idx)
    {
        int idx = stone_idx + round_idx * NUM_OF_STONES;

        return time_c.WaitTime(timers[idx], time);
    }

    //method to count the seconds of the stones without moving
    private bool WaitTime(float time, int round_idx, int stone_idx)
    {
        int idx = stone_idx + round_idx * NUM_OF_STONES;

        return time_c.WaitTimeWithReset(timers[idx], time);
    }

    //method to get the time of a timer
    private float GetTimeOfTimer(int idx)
    {
        return time_c.GetTime(timers[idx]);
    }

    //method to check if a timer is "finished"
    private bool GetIfFinishedTimer(int round_idx, int stone_idx)
    {
        int idx = stone_idx + round_idx * NUM_OF_STONES;

        return time_c.GetIfFinishedTimer(timers[idx]);
    }

    //method to set a timer as "finished" with a negative time
    private void SetFinishedTimer(int round_idx, int stone_idx)
    {
        int idx = stone_idx + round_idx * NUM_OF_STONES;
        time_c.SetFinishedTimer(timers[idx]);
    }

    //method to know, given a round, if all the previous and that round have been finished
    public bool GetIfRoundsFinished(int last_finished_round)
    {
        bool condition = true;

        for(int i = 0; i <= last_finished_round; i++)
        {
            for(int j = 0; j < NUM_OF_STONES; j++)
            {
                condition &= GetIfFinishedTimer(i, j);
            }
        }

        return condition;
    }

    //method to know, given a round, if all the previous and that round stones have explode
    public bool GetIfStonesExploded(int last_finished_round)
    {
        bool condition = true;

        for (int i = 0; i <= last_finished_round; i++)
        {
            for (int j = 0; j < NUM_OF_STONES; j++)
            {
                int explosion_timer_idx = explosion_effect_timer_offset + j + (i * 4);

                condition &= GetIfFinishedTimer(0, explosion_timer_idx);
            }
        }

        return condition;
    }

    //method to know if the tutorial has finished counting time
    public bool CheckIfTutorialRoundsEnd()
    {
        bool condition = true;

        for(int i = 0; i < CheckboxManager.number_of_tutorial_rounds; i++)
        {
            for(int j = 0; j < NUM_OF_STONES; j++)
            {
                condition = condition && GetIfFinishedTimer(i, j);
            }
        }

        return condition;
    }

    //method to know if a timer is counting time
    public bool CheckIfTrueRoundsEnd()
    {
        bool condition = true;

        int num_of_rounds = CheckboxManager.number_of_tutorial_rounds + CheckboxManager.number_of_true_rounds;

        for (int i = CheckboxManager.number_of_tutorial_rounds; i < num_of_rounds; i++)
        {
            for (int j = 0; j < NUM_OF_STONES; j++)
            {
                condition = condition && GetIfFinishedTimer(i, j);
            }
        }

        return condition;
    }

    //<----------------------SOUND METHODS-------------------------->

    //method to play a sound
    private void PlayControlledClipOnce(SoundInfo soundInfo) { sc.PlayControlledClipOnce(soundInfo); }

    //method to play a sound one shot
    private void PlayClipOneShot(SoundInfo soundInfo, int current_round, int current_stone, float volume = 0.55f)
    {
        if (!Settings.sync)
        {
            volume = 0.9f;
        }

        if (Settings.binauralSound && (current_round>=0) && (current_stone >= 0))
        {
            nwsc.DynamicPlayClip(soundInfo, stc.GetStoneTransform(current_round, current_stone));
        }
        else
        {
            sc.PlayClipOneShot(soundInfo, volume);
        }
    }

    //method to get the clip length
    private float ClipLength(int clip_list, int clip_idx) { return sc.ClipLength(clip_list, clip_idx); }

    //<----------------------MIXED SOUND/EFFECTS METHODS-------------------------->

    //<----------------------STONES SOUNDS AND INFO-------------------------->

    //Method to init the narrative control
    private void InitStoneShineControl()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        //the first i is wait_narrative.Count because, if they are already created, it won't enter to the loop
        for (int i = stones_shined.Count; i < num_of_rounds * NUM_OF_STONES; i++) { stones_shined.Add(false); }
    }

    //method to get the stone speed from the checkboxes
    private void GetStoneLightIntensity()
    {
        light_intensity = CheckboxManager.stone_light_intensity;
        explosion_intensity = CheckboxManager.stone_explosion_intensity;
    }

    //method to get if a stone shined at first tutorial round
    private bool AnyStoneShined()
    {
        bool condition = false;
        int start_round = CheckboxManager.number_of_training_rounds;

        for (int i = start_round * NUM_OF_STONES; i < (start_round+1)*NUM_OF_STONES; i++)
        {
            condition = condition || stones_shined[i];
        }

        return condition;
    }

    //method to calculate the light intensity of the "blink" effect
    private float CalculateBlink(int idx)
    {
        float coeff = GetTimeOfTimer(idx);

        if (coeff > wait_time_explosion / 2)
        {
            return ((wait_time_explosion - coeff) / wait_time_explosion) * 2.0f;
        }
        else
        {
            return (coeff / wait_time_explosion) * 2.0f;
        }
    }

    //method to make stones shine
    private void BrightStone(int current_round, int current_stone)
    {
        //Sound Effect
        if (CheckboxManager.play_shine_stone_sound && !stones_shined[current_stone + current_round*NUM_OF_STONES])
        {
            PlayClipOneShot(SoundInformationLevel1.STONE_SHINE, current_round, current_stone);  //stone shine sound

            stones_shined[current_stone + current_round * NUM_OF_STONES] = true;
        }

        //Visual effect
        int timer_idx = explosion_effect_timer_offset + current_stone + (current_round * 4);

        float final_light_intensity = light_intensity;

        //blink
        bool blink_condition = stc.GetIfStoneKicked(current_round, current_stone) && !GetIfFinishedTimer(0, timer_idx);

        if (blink_condition)
        {
            if (WaitTime(wait_time_explosion, 0, timer_idx)) { ResetTimer(0, timer_idx); SetFinishedTimer(0, timer_idx); }

            final_light_intensity = explosion_intensity * CalculateBlink(timer_idx);
        }

        //if the timer is finished (if the blink has been done)
        if (GetIfFinishedTimer(0, timer_idx))
        {
            stc.DeactivateStone(current_round, current_stone);
        }
        //if the timer isn't finished (if the blink hasn't been done)
        else
        {
            bool use_emissive_color_condition = CheckboxManager.light_stone_when_ready_to_kick || blink_condition;

            stc.BrightStone(current_round, current_stone, final_light_intensity, use_emissive_color_condition);
        }
    }

    //method to make stones not shine
    private void StopBrightStone(int current_round, int current_stone) { stc.StopBrightStone(current_round, current_stone); }

    //method to bright the lake
    private void BrightLake(int current_round, int current_stone)
    {
        if (GetIfAllStonesOfRoundHasBeenKicked(current_round) && !WaitTimeWithoutReset(CheckboxManager.seconds_to_light_lake, current_round, current_stone)) { ice_controller.ShineLake(current_round); }
    }

    //method to check and play a sound if a stone has been thrown
    private void PlayThrowSound(int current_round, int current_stone)
    {
        //stone throw sound
        if (CheckboxManager.play_throw_stone_sound) { PlayClipOneShot(SoundInformationLevel1.STONE_THROW, current_round, current_stone); }
    }

    //method to check and play a sound if a stone has been kicked
    private void PlayKickSound(int current_round, int currentStone)
    {
        if (CheckboxManager.play_kick_stone_sound)
        {
            if(GetIfStoneKickedPerfect(current_round, currentStone) || (CheckboxManager.onlyKickPerfectSound && !CheckboxManager.muteGoodSound))
            {
                PlayClipOneShot(SoundInformationLevel1.KICK_STONE_PERFECT, current_round, currentStone);
            }
            else
            {
                if (!CheckboxManager.muteGoodSound) { PlayClipOneShot(SoundInformationLevel1.KICK_STONE_GOOD, current_round, currentStone); }
            }
        }
    }

    //method to get if it is able to kick the stone
    private bool IsReadyToKick(int current_round, int current_stone) { return stc.IsReadyToKick(current_round, current_stone); }

    //<----------------------GAMEPLAY NARRATIVE-------------------------->
    //Method to init the narrative control
    private void InitNarrativeControl()
    {
        //the first i is wait_narrative.Count because, if they are already created, it won't enter to the loop
        for (int i = wait_narrative.Count; i < 3; i++) { wait_narrative.Add(DISABLED); }
    }

    //Method to control the times between narratives
    private void ControlTimesBetweenNarratives(int clip_list, int clip_idx, int round_type, bool condition)
    {
        if (condition)
        {
            if (WaitTime(ClipLength(clip_list, clip_idx), 0, narrative_timer_idx)) { wait_narrative[round_type] = ACTIVE; }
        }
    }

    public bool ReproduceGameplayNarrative(int round_type, bool reproduceNarrative)
    {
        bool condition = wait_narrative[round_type];

        switch (round_type)
        {
            case TRAINING_ROUNDS:
                wait_narrative[round_type] = ACTIVE;    //don't wait

                break;

            case TUTORIAL_ROUNDS:

                condition = condition | !CheckboxManager.play_First_Stone_Narrative;

                if (CheckboxManager.play_First_Stone_Narrative && reproduceNarrative) { PlayControlledClipOnce(SoundInformationLevel1.N_7_FIRST_STONE); }
                //ControlTimesBetweenNarratives(SoundController.NARRATIVE, SoundController.N_7_FIRST_STONE, round_type, !condition);
                wait_narrative[round_type] = ACTIVE;    //don't wait

                //ADELANTE CLIP
                if (AnyStoneShined() && !sc.IsPlaying(SoundInformationLevel1.NARRATIVE) && CheckboxManager.play_Adelante_Stone_Narrative)
                    PlayControlledClipOnce(SoundInformationLevel1.ADELANTE);

                break;

            case TRUE_ROUNDS:

                condition = condition | !CheckboxManager.play_Follow_Stone_Narrative;

                if (CheckboxManager.play_Follow_Stone_Narrative && reproduceNarrative) { PlayControlledClipOnce(SoundInformationLevel1.N_8_FOLLOW_STONE); }

                if (CheckboxManager.wait_until_famirialitzation_ends)
                {
                    ControlTimesBetweenNarratives(SoundInformationLevel1.NARRATIVE, SoundInformationLevel1.N_8_FOLLOW_STONE.clipIdx, round_type, !condition);
                }
                else
                {
                    wait_narrative[round_type] = ACTIVE;    //don't wait
                }

                break;
        }

        return condition;
    }

    //<----------------------ROUND SOUND METHODS-------------------------->

    //method to check and play a sound if a stone has been kicked
    public void CheckAndPlayNextRoundSound(int current_round)
    {
        if (stc.GetIfSomeStoneHasBeenKicked(current_round) && CheckboxManager.play_round_pass_sound) { PlayClipOneShot(SoundInformationLevel1.ROUND_PASS, -1, -1); }
    }

    //<----------------------ICE CONTROL AND SOUND METHODS-------------------------->

    //Method to init the ice spawn control
    private void InitIceSpawnControl()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        //the first i is ice_crack_spawned.Count because, if they are already created, it won't enter to the loop
        for (int i = ice_crack_spawned.Count; i < num_of_rounds * NUM_OF_STONES; i++) { ice_crack_spawned.Add(DISABLED); }
    }

    private void SpawnIceCrack(Vector3 position)
    {
        Instantiate(ice_crack_prefab, new Vector3(position.x, 1.1f, position.z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    //method to spawn an ice crack under a stone
    private void SpawnIceCrackUnderStone(int current_round)
    {
        List<Vector3> stones_position = stc.GetStonePositions(current_round);
        List<bool> stone_kicks = stc.GetStoneKicks(current_round);

        for (int i = 0; i < stone_kicks.Count; i++)
        {
            if (stone_kicks[i] && !ice_crack_spawned[i + (NUM_OF_STONES * current_round)])
            {
                PlayKickSound(current_round, i);
                if (CheckboxManager.play_ice_crack_sound) { PlayClipOneShot(SoundInformationLevel1.CRACK, current_round, i); }
                SpawnIceCrack(stones_position[i]);

                ice_crack_spawned[i + (NUM_OF_STONES * current_round)] = ACTIVE;

                //write data about the kicked stones
                string identifier = i.ToString() + "_" + current_round.ToString();
                data_extractor.WriteDataLineStoneSteppingFinalPosition(identifier, DataExtractorCSVLevel1.ACTIVE, stc.GetIfStonePerfectTime(current_round, i));
            }
        }
    }

    //<----------------------METHODS TO USE THE INTERACTION CONTROLLER-------------------------->
    public bool ManageRunRound(float seconds_to_kick, int current_round, int round_type, int current_stone)
    {
        //stc.MoveStones(stc.GetIfStoneKickHasBeenEnabled(DISABLED));  //slow the stone MECHANIC

        //condition to don't repeat a round
        if (GetIfFinishedTimer(current_round, current_stone))
        {
            BrightStone(current_round, current_stone);
            return true;
        }

        if (IsReadyToKick(current_round, current_stone))                  //if you can kick the stone
        {
            stc.UpdateStoneKicks();
            
            BrightStone(current_round, current_stone);
            BrightLake(current_round, current_stone);

            SpawnIceCrackUnderStone(current_round); //it stores data if the stone is kicked

            if(CheckboxManager.firstStonesNotDestroyed && (current_round < CheckboxManager.numberOfStonesNotDestroyed))
            {
                if(GetIfStoneKicked(current_round, current_stone))
                {
                    SetFinishedTimer(current_round, current_stone);
                }
            }
            else if (WaitTime(seconds_to_kick, current_round, current_stone))       //if the seconds to kick has passed, then finish the round
            {
                SetFinishedTimer(current_round, current_stone);

                //StopBrightStone(current_round, current_stone);

                //write data about the not kicked stones
                if (!GetIfStoneKicked(current_round, current_stone))
                {
                    string identifier = current_stone.ToString() + "_" + current_round.ToString();

                    data_extractor.WriteDataLineStoneSteppingFinalPosition(identifier, DataExtractorCSVLevel1.INACTIVE, -1);

                    //deactivate stone
                    int explosion_timer_idx = explosion_effect_timer_offset + current_stone + (current_round * 4);
                    SetFinishedTimer(0, explosion_timer_idx);
                    BrightStone(current_round, current_stone);
                }

                //tc.DeactivateTarget(current_round, current_stone);
                //stc.DeactivateStone(current_round, current_stone);

                return true;
            }
        }
        else   //else, move the stones
        {
            stc.MoveStone(DISABLED, current_round, current_stone);
            stc.BrightStone(current_round, current_stone, CheckboxManager.stone_light_intensity_while_going_to_final_position, CheckboxManager.light_stone_while_going_to_final_position, CheckboxManager.stone_transparency_while_going_to_final_position);
        }

        return false;
    }

    public bool ManagePrepareRound(int current_round, int current_stone)
    {
        if (GetIfFinishedTimer(current_round, current_stone)) { return true; } //condition to don't repeat a round

        //tc.ActivateTarget(current_round, current_stone);
        stc.ActivateStone(current_round, current_stone);

        if (WaitTime(0.05f, current_round, current_stone))  //to avoid bugs with trigger exit of stones
        {
            PlayThrowSound(current_round, current_stone);
            return true;
        }

        return false;
    }


    private bool GetIfAllStonesOfRoundHasBeenKicked(int round) { return stc.GetIfAllStonesOfRoundHasBeenKicked(round); }

    //methods for data extraction
    public (int, int) GetAssignedPlayerAndCurrentRound(string stone_name) { return stc.GetAssignedPlayerAndCurrentRound(stone_name); }
    public (float, float) GetCurrentStonePosition(int stone, int round) { return stc.GetCurrentStonePosition(stone, round); }
    public (float, float, float, float) GetAngleDistanceXYTuple(string identifier) { return tc.GetAngleDistanceXYTuple(identifier); }

    public bool GetIfStoneKicked(int round, int stone) { return stc.GetIfStoneKicked(round, stone); }
    public bool GetIfStoneKickedPerfect(int round, int stone) { return stc.GetIfStonePerfectTime(round, stone)>0; }

    public float GetStoneSpeed(int stone, int round) { return stc.GetStoneSpeed(stone, round); }
}
