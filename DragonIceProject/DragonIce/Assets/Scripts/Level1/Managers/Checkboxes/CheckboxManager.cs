using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxManager
{
    //<------------------------CHECKBOXES--------------------------->

    //This file is a static class in order to use these variables in other scripts
    //In any script, type CheckboxManager.name_of_variable to use or change a variable

    //<------------------------NARRATIVE--------------------------->

    public static bool play_dragon_Introduction_Narrative { get; set; }
    public static bool play_dragon_Fly_Narrative { get; set; }
    public static bool play_character_introduction_Narrative { get; set; }
    public static bool play_freeze_Narrative { get; set; }
    public static bool play_help_dragon_Narrative { get; set; }
    public static bool play_docks_Narrative { get; set; }
    public static bool play_First_Stone_Narrative { get; set; }
    public static bool play_Adelante_Stone_Narrative { get; set; }
    public static bool play_Follow_Stone_Narrative { get; set; }
    public static bool play_Final_Narrative { get; set; }

    //<-----------------------SEQUENCES-------------------------->

    public static bool skip_story_sequences { get; set; }
    public static bool ingame_stay_dragon { get; set; }
    public static bool demoTutorialSequence { get; set; }

    //<-----------------------INTERACTION-------------------------->
    
    public static bool light_stone_when_ready_to_kick { get; set; }
    public static float stone_light_intensity { get; set; }
    public static bool light_stone_while_going_to_final_position { get; set; }
    public static float stone_light_intensity_while_going_to_final_position { get; set; }
    public static float stone_transparency_while_going_to_final_position { get; set; }
    public static float stone_explosion_intensity { get; set; }
    public static bool play_round_pass_sound { get; set; }
    public static bool play_throw_stone_sound { get; set; }
    public static bool play_shine_stone_sound { get; set; }
    public static bool play_kick_stone_sound { get; set; }
    public static bool play_ice_crack_sound { get; set; }
    public static bool light_lake { get; set; }
    public static float lake_light_intensity { get; set; }
    public static float seconds_to_light_lake { get; set; }
    public static bool onlyKickPerfectSound { get; set; }
    public static bool muteGoodSound { get; set; }

    //<-----------------------PARTICLES-------------------------->

    public static bool perfectTimingParticles { get; set; }
    public static Vector2 seconds_to_perfect_timing { get; set; }
    public static float particleSpeed { get; set; }
    public static float particleMovingTime { get; set; }
    public static float particleDestroyTime { get; set; }
    public static Vector2 particleRateRange { get; set; }
    public static Vector2 particleSizeRange { get; set; }
    public static bool brightParticles { get; set; }

    //<-----------------------ROUNDS-------------------------->

    public static bool enableTrainingRounds { get; set; }
    public static bool trainingAlwaysWithTheSameAngle { get; set; }
    public static int number_of_training_rounds { get; set; }
    public static int number_of_tutorial_rounds { get; set; }
    public static int number_of_true_rounds { get; set; }

    public static float time_between_previous_stone_training { get; set; }
    public static Vector2 time_between_previous_stone_tutorial_interval { get; set; }
    public static Vector2 time_between_previous_stone_true_interval { get; set; }

    public static bool wait_until_famirialitzation_ends { get; set; }
    public static bool firstStonesNotDestroyed { get; set; }
    public static int numberOfStonesNotDestroyed { get; set; }

    //<-----------------------STONES-------------------------->

    public static float stone_speed_training { get; set; }
    public static Vector2 stone_speed_tutorial_interval { get; set; }
    public static Vector2 stone_speed_true_interval { get; set; }

    public static float distance_to_target { get; set; }
    public static float minimum_distance_to_target { get; set; }
    public static Vector2 angle_addition_interval { get; set; }

    public static float seconds_to_kick_training { get; set; }
    public static Vector2 seconds_to_kick_tutorial { get; set; }
    public static Vector2 seconds_to_kick_true { get; set; }

    public static bool fixedDistanceToCenter { get; set; }
    public static float[] fixedDistancesToCenter { get; set; }

    // Num of rounds level 1
    public static int num_of_rounds { get; set; }
    public static int numOfRoundsPrevToTruePhase { get; set; }
}
