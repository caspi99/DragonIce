using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxAssignment : MonoBehaviour
{
    //<------------------------CHECKBOXES--------------------------->
    //This script puts all the values to the CheckboxManager

    [Header("Narrative")]
    [Space(10)]

    [SerializeField] private bool play_dragon_Introduction_Narrative = true;          //Checkbox to play Narrative Audio: Introduction 1
    [SerializeField] private bool play_dragon_Fly_Narrative = true;                   //Checkbox to play Narrative Audio: Introduction 2
    [SerializeField] private bool play_character_introduction_Narrative = true;       //Checkbox to play Narrative Audio: Characters Introduction
    [SerializeField] private bool play_freeze_Narrative = true;                       //Checkbox to play Narrative Audio: Freeze
    [SerializeField] private bool play_help_dragon_Narrative = true;                  //Checkbox to play Narrative Audio: Dragon Needs Help
    [SerializeField] private bool play_docks_Narrative = true;                        //Checkbox to play Narrative Audio: Docks Audio
    [SerializeField] private bool play_First_Stone_Narrative = true;                  //Checkbox to play Narrative Audio: First Stone
    [SerializeField] private bool play_Follow_Stone_Narrative = true;                 //Checkbox to play Narrative Audio: Following Stones
    [SerializeField] private bool play_Final_Narrative = true;                        //Checkbox to play Narrative Audio: Final

    [Header("Sequences")]
    [Space(10)]

    [SerializeField] private bool skip_story_sequences = false;                       //Checkbox to indicate if you want to skip story sequences
    [SerializeField] private bool ingame_stay_dragon = false;                         //Checkbox to indicate if you want to activate the stay animation of the dragon
    [SerializeField] private bool demoTutorialSequence = false;                       //Checkbox to indicate if you want to activate the demo tutorial sequence

    [Header("Interaction")]
    [Space(10)]

    [SerializeField] private bool play_round_pass_sound = true;                       //Checkbox to play the round pass sound
    [SerializeField] private bool play_throw_stone_sound = true;                      //Checkbox to play the throw stone sound
    [SerializeField] private bool play_shine_stone_sound = true;                      //Checkbox to play the shine stone sound
    [SerializeField] private bool light_stone_when_ready_to_kick = true;              //Checkbox to decide if the stone is light when ready to kick
    [SerializeField] private float stone_light_intensity = 100.0f;                    //Checkbox to change the light intensity of the stones
    [SerializeField] private bool light_stone_while_going_to_final_position = true;              //Checkbox to decide if the stone is light when going to the final position
    [SerializeField] private float stone_light_intensity_while_going_to_final_position = 10.0f;                    //Checkbox to change the light intensity of the stones
    [SerializeField] private float stone_transparency_while_going_to_final_position = 0.4f;                    //Checkbox to change the light intensity of the stones
    [SerializeField] private float stone_explosion_intensity = 1000.0f;               //Checkbox to change the explosion intensity of the stones
    [SerializeField] private bool play_kick_stone_sound = true;                       //Checkbox to play the kick stone sound
    [SerializeField] private bool play_ice_crack_sound = true;                        //Checkbox to play the ice crack sound
    [SerializeField] private bool light_lake = true;              //Checkbox to decide if the lake is light when 4 stones of the same round have been kicked
    [SerializeField] private float lake_light_intensity = 100.0f;                    //Checkbox to change the light intensity of the lake
    [SerializeField] private float seconds_to_light_lake = 1.0f;                    //Checkbox to change the seconds needed to light the lake
    [SerializeField] private bool onlyKickPerfectSound = true;                      //Checkbox to decide if only perfect kick sound is played
    [SerializeField] private bool muteGoodSound = false;                      //Checkbox to decide if the good sound is muted or not

    [Header("Particles")]
    [Space(10)]

    [SerializeField] private bool perfectTimingParticles = false;                    //Checkbox to decide if particles are activated or not
    [SerializeField] private Vector2 seconds_to_perfect_timing = new Vector2(0.1f, 1.0f);   //Checkbox to change the seconds needed to do a perfect
    [SerializeField] private float particleSpeed = 35f;     //Checkbox to decide the speed of the particles
    [SerializeField] private float particleMovingTime = 3f;         //Checkbox to decide the time a particle is moving
    [SerializeField] private float particleDestroyTime = 5f;        //Checkbox to decide the time a particle is alive
    [SerializeField] private Vector2 particleRateRange = new Vector2(15, 2);        //Checkbox to decide the rate range
    [SerializeField] private Vector2 particleSizeRange = new Vector2(1f, 0.1f);     //Checkbox to decide the particle size range
    [SerializeField] private bool brightParticles = false;     //Checkbox to decide if the particles bright

    private CheckboxAssignmentLevel1Sync checkboxAssignmentLevel1Sync;
    private CheckboxAssignmentLevel1Async checkboxAssignmentLevel1Async;

    void Awake()
    {
        checkboxAssignmentLevel1Sync = this.GetComponent<CheckboxAssignmentLevel1Sync>();
        checkboxAssignmentLevel1Async = this.GetComponent<CheckboxAssignmentLevel1Async>();
        AssignCheckboxes();
    }

    private void AssignCheckboxes()
    {
        //Checkboxes related to Narrative
        CheckboxManager.play_dragon_Introduction_Narrative = play_dragon_Introduction_Narrative;
        CheckboxManager.play_dragon_Fly_Narrative = play_dragon_Fly_Narrative;
        CheckboxManager.play_character_introduction_Narrative = play_character_introduction_Narrative;
        CheckboxManager.play_freeze_Narrative = play_freeze_Narrative;
        CheckboxManager.play_help_dragon_Narrative = play_help_dragon_Narrative;
        CheckboxManager.play_docks_Narrative = play_docks_Narrative;
        CheckboxManager.play_First_Stone_Narrative = play_First_Stone_Narrative;
        CheckboxManager.play_Follow_Stone_Narrative = play_Follow_Stone_Narrative;
        CheckboxManager.play_Final_Narrative = play_Final_Narrative;

        //Checkboxes related to sequences
        CheckboxManager.skip_story_sequences = skip_story_sequences;
        CheckboxManager.ingame_stay_dragon = ingame_stay_dragon;
        CheckboxManager.demoTutorialSequence = demoTutorialSequence;

        //Checkboxes related to interaction
        CheckboxManager.play_round_pass_sound = play_round_pass_sound;
        CheckboxManager.play_throw_stone_sound = play_throw_stone_sound;
        CheckboxManager.play_shine_stone_sound = play_shine_stone_sound;
        CheckboxManager.light_stone_when_ready_to_kick = light_stone_when_ready_to_kick;
        CheckboxManager.light_stone_while_going_to_final_position = light_stone_while_going_to_final_position;
        CheckboxManager.stone_light_intensity = stone_light_intensity;
        CheckboxManager.stone_light_intensity_while_going_to_final_position = stone_light_intensity_while_going_to_final_position;
        CheckboxManager.stone_transparency_while_going_to_final_position = stone_transparency_while_going_to_final_position;
        CheckboxManager.stone_explosion_intensity = stone_explosion_intensity;
        CheckboxManager.play_kick_stone_sound = play_kick_stone_sound;
        CheckboxManager.play_ice_crack_sound = play_ice_crack_sound;
        CheckboxManager.light_lake = light_lake;
        CheckboxManager.lake_light_intensity = lake_light_intensity;
        CheckboxManager.seconds_to_light_lake = seconds_to_light_lake;
        CheckboxManager.onlyKickPerfectSound = onlyKickPerfectSound;
        CheckboxManager.muteGoodSound = muteGoodSound;

        //Checkboxes related to Particles
        CheckboxManager.perfectTimingParticles = perfectTimingParticles;
        CheckboxManager.seconds_to_perfect_timing = seconds_to_perfect_timing;
        CheckboxManager.particleSpeed = particleSpeed;
        CheckboxManager.particleMovingTime = particleMovingTime;
        CheckboxManager.particleDestroyTime = particleDestroyTime;
        CheckboxManager.particleRateRange = particleRateRange;
        CheckboxManager.particleSizeRange = particleSizeRange;
        CheckboxManager.brightParticles = brightParticles;

        //if synchronous
        if (Settings.sync)
        {
            //Checkboxes related to rounds
            CheckboxManager.number_of_tutorial_rounds = checkboxAssignmentLevel1Sync.number_of_tutorial_rounds;
            CheckboxManager.number_of_true_rounds = checkboxAssignmentLevel1Sync.number_of_true_rounds;

            CheckboxManager.time_between_previous_stone_tutorial_interval = checkboxAssignmentLevel1Sync.time_between_previous_stone_tutorial_interval;
            CheckboxManager.time_between_previous_stone_true_interval = checkboxAssignmentLevel1Sync.time_between_previous_stone_true_interval;

            CheckboxManager.wait_until_famirialitzation_ends = checkboxAssignmentLevel1Sync.wait_until_famirialitzation_ends;
            CheckboxManager.firstStonesNotDestroyed = checkboxAssignmentLevel1Sync.firstStonesNotDestroyed;
            CheckboxManager.numberOfStonesNotDestroyed = checkboxAssignmentLevel1Sync.numberOfStonesNotDestroyed;

            //Checkboxes related to stones/rounds
            CheckboxManager.stone_speed_tutorial_interval = checkboxAssignmentLevel1Sync.stone_speed_tutorial_interval;
            CheckboxManager.stone_speed_true_interval = new Vector2(checkboxAssignmentLevel1Sync.stone_speed_true, checkboxAssignmentLevel1Sync.stone_speed_true);

            CheckboxManager.distance_to_target = checkboxAssignmentLevel1Sync.distance_to_target;
            CheckboxManager.minimum_distance_to_target = checkboxAssignmentLevel1Sync.minimum_distance_to_target;
            CheckboxManager.angle_addition_interval = new Vector2(checkboxAssignmentLevel1Sync.angle_addition, checkboxAssignmentLevel1Sync.angle_addition);

            CheckboxManager.seconds_to_kick_tutorial = checkboxAssignmentLevel1Sync.seconds_to_kick_tutorial;
            CheckboxManager.seconds_to_kick_true = checkboxAssignmentLevel1Sync.seconds_to_kick_true;
            CheckboxManager.fixedDistanceToCenter = false;
        }
        //if asynchronous
        else
        {
            //Checkboxes related to rounds
            CheckboxManager.number_of_tutorial_rounds = checkboxAssignmentLevel1Async.number_of_tutorial_rounds;
            CheckboxManager.number_of_true_rounds = checkboxAssignmentLevel1Async.number_of_true_rounds;

            CheckboxManager.time_between_previous_stone_tutorial_interval = checkboxAssignmentLevel1Async.time_between_previous_stone_tutorial_interval;
            CheckboxManager.time_between_previous_stone_true_interval = checkboxAssignmentLevel1Async.time_between_previous_stone_true_interval;

            CheckboxManager.wait_until_famirialitzation_ends = checkboxAssignmentLevel1Async.wait_until_famirialitzation_ends;
            CheckboxManager.firstStonesNotDestroyed = checkboxAssignmentLevel1Async.firstStonesNotDestroyed;
            CheckboxManager.numberOfStonesNotDestroyed = checkboxAssignmentLevel1Async.numberOfStonesNotDestroyed;

            //Checkboxes related to stones/rounds
            CheckboxManager.stone_speed_tutorial_interval = checkboxAssignmentLevel1Async.stone_speed_tutorial_interval;
            CheckboxManager.stone_speed_true_interval = checkboxAssignmentLevel1Async.stone_speed_true_interval;

            CheckboxManager.distance_to_target = checkboxAssignmentLevel1Async.distance_to_target;
            CheckboxManager.minimum_distance_to_target = checkboxAssignmentLevel1Async.minimum_distance_to_target;
            CheckboxManager.angle_addition_interval = checkboxAssignmentLevel1Async.angle_addition_interval;

            CheckboxManager.seconds_to_kick_tutorial = checkboxAssignmentLevel1Async.seconds_to_kick_tutorial;
            CheckboxManager.seconds_to_kick_true = checkboxAssignmentLevel1Async.seconds_to_kick_true;
            CheckboxManager.fixedDistanceToCenter = checkboxAssignmentLevel1Async.fixedDistanceToCenter;
            CheckboxManager.fixedDistancesToCenter = checkboxAssignmentLevel1Async.fixedDistancesToCenter;
        }
    }
}
