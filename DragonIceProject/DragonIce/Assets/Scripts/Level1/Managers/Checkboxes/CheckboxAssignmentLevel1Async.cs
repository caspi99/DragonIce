using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxAssignmentLevel1Async : MonoBehaviour
{
    //<------------------------CHECKBOXES--------------------------->
    //This script puts all the values to the CheckboxManager

    [Header("Rounds Async")]
    [Space(10)]

    public int number_of_tutorial_rounds = 2;                       //Inputbox to change the number of tutorial rounds
    public int number_of_true_rounds = 2;                           //Inputbox to change the number of true rounds

    public Vector2 time_between_previous_stone_tutorial_interval = new Vector2(6.0f, 10.0f);      //Inputbox to change the seconds between rounds
    public Vector2 time_between_previous_stone_true_interval = new Vector2(2.0f, 5.0f);      //Inputbox to change the seconds between rounds

    public bool wait_until_famirialitzation_ends = false;
    public bool firstStonesNotDestroyed = true;
    public int numberOfStonesNotDestroyed = 1;

    [Header("Rounds/Stones Async")]
    [Space(10)]

    public Vector2 stone_speed_tutorial_interval = new Vector2(2.5f, 6.0f);  //Inputbox to change the speed of the stones
    public Vector2 stone_speed_true_interval = new Vector2(6.0f, 9.0f);  //Inputbox to change the speed of the stones

    public float distance_to_target = 30.0f;                        //Inputbox to change the radius between the center of the map and the targets
    public float minimum_distance_to_target = 15.0f;                //Inputbox to change the minimum radius between the center of the map and the targets
    public Vector2 angle_addition_interval = new Vector2(45.0f, 60.0f);      //Inputbox to change the angle addition between rounds in radians (without pi)
    
    public Vector2 seconds_to_kick_tutorial = new Vector2(2.0f, 5.0f);       //Inputbox to change the seconds to kick
    public Vector2 seconds_to_kick_true = new Vector2(2.0f, 5.0f);       //Inputbox to change the seconds to kick
    public bool fixedDistanceToCenter = true;     //Inputbox to decide if the random distance to the center is fixed for each player
    public float[] fixedDistancesToCenter = new float[4] { 35f, 29f, 31f, 33f };
}
