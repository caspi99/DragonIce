using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //<------------------------SETTINGS--------------------------->

    //This file is a class with settings for both levels

    //<-------------------------TRACKING---------------------------->
    public static bool enableTracking { get; set; }
    public static bool screenToWorldPosTracking { get; set; }
    public static bool enableRotation { get; set; }
    public static bool enableYAxis { get; set; }
    public static bool tracking_player_reorder { get; set; }
    public static bool light_docks { get; set; }

    //<-------------------------LEVELS---------------------------->
    public static bool play_level_1 { get; set; }
    public static bool play_level_2 { get; set; }

    //<------------------------GAMEMODE--------------------------->
    public static bool sync { get; set; }

    //<----------------------DATA EXTRACTOR------------------------->
    public static bool enableDataExtraction { get; set; }
    public static bool continuous_data_saving { get; set; }
    public static float update_data_time { get; set; }

    //<-----------------------SOUND------------------------------>
    public static bool binauralSound { get; set; }

    //<------------------------PLAYERS--------------------------->
    public static bool visualizePlayerPosition { get; set; }
}
