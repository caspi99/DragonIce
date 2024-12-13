using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxAssignmentLevel1Sync : MonoBehaviour
{
    //<------------------------CHECKBOXES--------------------------->
    //This script puts all the values to the CheckboxManager

    [Header("Rounds Sync")]
    [Space(10)]

    public int number_of_tutorial_rounds = 2;                       //Inputbox to change the number of tutorial rounds
    public int number_of_true_rounds = 2;                           //Inputbox to change the number of true rounds

    public Vector2 time_between_previous_stone_tutorial_interval = new Vector2(5.0f, 10.0f);   //Inputbox to change the seconds between rounds
    public Vector2 time_between_previous_stone_true_interval = new Vector2(5.0f, 10.0f);   //Inputbox to change the seconds between rounds

    public bool wait_until_famirialitzation_ends = false;           //Checkbox to decide to start the next stage until famirialization stage ends
    public bool firstStonesNotDestroyed = true;
    public int numberOfStonesNotDestroyed = 1;

    [Header("Rounds/Stones Sync")]
    [Space(10)]

    public Vector2 stone_speed_tutorial_interval = new Vector2(10.0f, 4.0f); //Inputbox to change the speed of the stones
    public float stone_speed_true = 2.5f;                           //Inputbox to change the speed of the stones

    public float distance_to_target = 30.0f;                        //Inputbox to change the radius between the center of the map and the targets
    public float minimum_distance_to_target = 15.0f;                //Inputbox to change the minimum radius between the center of the map and the targets
    public float angle_addition = 45.0f;                            //Inputbox to change the angle addition between rounds in radians (without pi)

    public Vector2 seconds_to_kick_tutorial = new Vector2(2.0f, 2.0f);       //Inputbox to change the seconds to kick
    public Vector2 seconds_to_kick_true = new Vector2(2.0f, 2.0f);       //Inputbox to change the seconds to kick
}
