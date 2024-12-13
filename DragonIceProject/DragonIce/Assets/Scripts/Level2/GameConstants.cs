using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    //This file is a static class in order to use these variables in other scripts
    //In any script, type GameConstants.name_of_variable to use a variable

    //<-------GRABBABLE TYPE CONSTANTS------>
    public const int STONE_PHASE_1 = 0;
    public const int TREE_PHASE_2 = 1;
    public const int STONE_PHASE_3 = 2;

    //<-------GRABBABLE SHAPE CONSTANTS------>
    public const int SHAPE_1 = 0;
    public const int SHAPE_2 = 1;
    public const int SHAPE_3 = 2;
    public const int SHAPE_4 = 3;

    //<-------SOLO/COLLABORATIVE GRABBABLE MOVEMENT CONSTANTS------>
    public const int SOLO = 0;
    public const int COLLABORATIVE = 1;

    //<-------GRIP SIDE CONSTANTS------>
    public const int LEFT_SIDE = 0;
    public const int RIGHT_SIDE = 1;

    //<-------GRABBABLE PLACEMENT TREE_PHASE_2 CONSTANTS------>
    public const int LEFT_CORNER = 0;
    public const int RIGHT_CORNER = 1;
    public const int MIDDLE = 2;

    //<--------------------CONTROL INDEXES------------------------->
    public const bool ACTIVE = true;
    public const bool DISABLED = false;
    public const bool FINISHED = false;

    //<-------PHASE CONSTANTS------>
    public const int PHASE_1 = 0;
    public const int PHASE_2 = 1;
    public const int PHASE_3 = 2;

    public const int MAX_STONES_PHASE_1 = 24;
    public const int MAX_BRIDGE_SECTIONS_PHASE_2 = 4;
    public const int MAX_TABLAS_PHASE_2 = 24;
    public const int MAX_STONES_PHASE_3 = 24;

    public static readonly List<string> grabbable_tags = new List<string> {"StonePhase1", "TreePhase2", "StonePhase3"};
    public static readonly List<string> grabbable_placement_tags = new List<string> { "StonePlacementPhase1", "TreePlacementPhase2", "StonePlacementPhase3" };
    public static readonly List<string> grabbable_placement_subcolliders_tags = new List<string> { "LeftCorner", "RightCorner", "RectangleMiddle" };

    //<-------MAP CONSTANTS------>

    public static readonly Vector2 MAP_CENTER = new Vector2(51.2f, 50f);
    public const float MAP_RADIUS = 43.0f;

    public const int MAP_BEFORE_EXPLOSION = 0;
    public const int MAP_AFTER_EXPLOSION = 1;

    public const int GLOBAL_TRANSFORM = 0;
    public const int PHASE_1_TRANSFORM = 1;
    public const int PHASE_2_TRANSFORM = 2;
    public const int PHASE_3_1_TRANSFORM = 3;
    public const int PHASE_3_2_TRANSFORM = 4;

    public static readonly Vector3[] map_positions = new Vector3[]
    {
        new Vector3(0f, -3f, 0f),
        new Vector3(1.5f, -3f, -163.2f),
        new Vector3(77.7f, -3f, 71.3f),   //71.3 - 66.7 = 4.6
        new Vector3(73.3f, -3f, 190.7f),
        //new Vector3(-156.4f, -3f, 129.7f)
        new Vector3(-126.4f, -3f, 99.7f)
    };

    public static readonly float[] map_scales = new float[] { 0.1f, 0.66f, 0.66f, 0.66f, 0.5f };

    public const int STONES_VISUAL = 0;
    public const int TREES_VISUAL = 1;
    public const int STONES_PLACEMENTS_VISUAL = 2;
    public const int BRIDGE_EMPTY_VISUAL = 3;
    public const int BRIDGE_FULL_VISUAL = 4;
    public const int TABLA_INDIVIDUAL = 5;
    public const int TABLA_COLECTIVA = 6;
    public const int PIEDRA_INDIVIDUAL = 7;
    public const int PIEDRA_COLECTIVA = 8;


    //<-------SMOKE CONSTANTS------>

    public const int SMOKE_INSTANCES = 12 * 12;
    public const int SMOKE_NUM_OF_ROWS = 12;
    public const int DISTANCE_BETWEEN_SMOKES = 10;

    public static readonly Vector3 SMOKE_BASE_POSITION = new Vector3(79.56533f, 12.7f, 57.70007f);
    public const float MAX_OFFSET_SMOKE = 1.5f;

    //<-------PLAYERS INTERACION CONSTANTS------>

    public const bool TRIGGER = true;
    public const bool COLLISION = false;



    //<-----PLAYER ORDER - NOT CONSTANT----->

    public static int[] player_order = new int[4] { 0, 1, 2, 3 };   //PLAYER REORDER

    //<-----PATH FOR DATA - NOT CONSTANT----->

    public static string filename_mechanics = "None";
    public static string filename_positions = "None";
    public static string filename_settings = "None";
    public static string folder = "None";
}
