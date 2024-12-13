using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxManagerLevel2
{
    //<------------------------CHECKBOXES--------------------------->

    //This file is a static class in order to use these variables in other scripts
    //In any script, type CheckboxManager.name_of_variable to use or change a variable

    //<------------------------NARRATIVE--------------------------->

    public static bool skip_story_sequences { get; set; }
    public static bool shorterStorySequences { get; set; }

    //<-----------------PHASE PERSONALIZATION-------------------->

    public static int num_of_stones_phase1 { get; set; }
    public static int num_of_trees_phase2 { get; set; }
    public static int num_of_stones_phase3 { get; set; }

    //<-----------------------INTERACTION-------------------------->

    public static float grabbable_move_speed_solo { get; set; }
    public static float player_mass { get; set; }

    public static bool exclusivePlayStyleGrabbables { get; set; }
    public static bool use_rigidbody_translation { get; set; }
    public static bool inclinate_grabbable { get; set; }

    public static bool show_grips { get; set; }

    //<-----------------------PHASE 1-------------------------->

    public static bool phase_1_check_orientation { get; set; }
    public static bool show_stone1_placement { get; set; }
    public static bool stayToPickStone { get; set; }
    public static float stayToPickStoneTime { get; set; }
    public static bool allGripsUntachCollaborative { get; set; }
    public static bool preferencePlayerStone { get; set; }
    public static bool biggerGripsWhenGrabbed { get; set; }

    //<-----------------------PHASE 2-------------------------->

    public static bool show_table_placement { get; set; }
    public static float tree_mass_touching { get; set; }
    public static float tree_mass_default { get; set; }
    public static bool pullTreeMechanic { get; set; }
}
