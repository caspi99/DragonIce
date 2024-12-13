using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTargetController : MonoBehaviour
{
    //<--------------------MATH CONSTANTS------------------------->
    private float c_radius;
    private float c_min_radius;
    private Vector2 c_angle_addition;

    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<-------------------TARGET INDEXES------------------------>
    const int TARGET_1 = 0;
    const int TARGET_2 = 1;
    const int TARGET_3 = 2;
    const int TARGET_4 = 3;

    private const int NUM_OF_TARGETS = 4;

    //<------------------------TARGETS--------------------------->
    private List<GameObject> cloned_targets;

    //<-----------ASSIGNED PLAYERS TO EACH TARGET-------------->
    private List<string> targets_assigned_players = new List<string>();

    //<------------------------FOR DATA EXTRACTION--------------------------->
    private List<(float Angle, float Distance, float X, float Y, string Identifier)> angle_distance_x_y_data = new List<(float, float, float, float, string)>();


    // Method to init the target controller
    public void InitTargetController(List<string> players)
    {
        InitElipseParameters();
        GetAllTargetsOfScene();
        SetTargetAssignedPlayers(players);

        int num_of_rounds = CheckboxManager.num_of_rounds;

        ChangeTargetsPosition(num_of_rounds);
    }

    //<----------------------METHODS THAT USES THE TARGET CONTROLLER-------------------------->

    //method to init the elipse parameters
    private void InitElipseParameters()
    {
        c_radius = CheckboxManager.distance_to_target;
        c_min_radius = CheckboxManager.minimum_distance_to_target;

        c_angle_addition.x = CheckboxManager.angle_addition_interval.x;
        c_angle_addition.y = CheckboxManager.angle_addition_interval.y;
    }

    //method to get all the targets from a scene
    private void GetAllTargetsOfScene()
    {
        List<GameObject> result = new List<GameObject>();
        result.AddRange(GameObject.FindGameObjectsWithTag("Target"));

        cloned_targets = result;
    }

    //method to init the stones assigned players list
    private void SetTargetAssignedPlayers(List<string> players) { targets_assigned_players = players; }

    //method to check if a float is inside an interval
    private bool CheckIfFloatInInterval(float value1, float value2, float interval)
    {
        bool condition = true;

        condition = condition && (value1 > (value2 - interval));
        condition = condition && (value1 < (value2 + interval));

        return condition;
    }

    //method to change the targets position (espiral)
    private void ChangeTargetsPosition(int num_rounds)
    {
        //this method calculates the position of the stones as if it was calculating 4 points in a circle

        float center = 50.0f;

        //angle of the circle
        float angle_between_targets = Mathf.PI / 2.0f;          //necessary to calculate the angle for each point
        float angle_offset = Mathf.PI / 4.0f;                   //angle_offset is important because we want that the starter points are in diagonal

        List<float> previous_angle_additions = new List<float>();
        for (int i = 0; i < NUM_OF_TARGETS; i++) { previous_angle_additions.Add(angle_offset); }

        for (int i = 0; i < num_rounds * NUM_OF_TARGETS; i++)
        {
            int current_round = i / NUM_OF_TARGETS;

            if (Settings.sync)
            {
                if (CheckboxManager.trainingAlwaysWithTheSameAngle)
                {
                    if (current_round < CheckboxManager.number_of_training_rounds) { current_round = 0; }
                    else { current_round -= CheckboxManager.number_of_training_rounds; }
                }

                float angle = (current_round * c_angle_addition.x * Mathf.Deg2Rad) + angle_offset;

                float stone_angle = angle + angle_between_targets * i;  //current stone angle

                //cosinus and sinus of the angle
                float angle_cos = Mathf.Cos(stone_angle);   //Mathf.Rad2Deg
                float angle_sin = Mathf.Sin(stone_angle);   //Mathf.Rad2Deg

                float radius = c_min_radius + ((c_radius - c_min_radius) * (num_rounds - current_round) / num_rounds);

                //x axis calculation
                float x = center + radius * angle_cos;

                //z axis calculation
                float z = center + radius * angle_sin;

                cloned_targets[i].transform.position = new Vector3(x, -1.0f, z);

                //for data extraction
                angle_distance_x_y_data.Add(((stone_angle * Mathf.Rad2Deg) % 360, radius, x, z, cloned_targets[i].name));
            }
            else
            {
                //radius and center of the circle
                float radius = c_min_radius + (Random.Range(0, (c_radius - c_min_radius)));

                //fixed distance
                if (CheckboxManager.fixedDistanceToCenter) { radius = CheckboxManager.fixedDistancesToCenter[i % NUM_OF_TARGETS]; }

                float random_value = Random.Range(c_angle_addition.x, c_angle_addition.y) * Mathf.Deg2Rad;

                // Always the same angle rounds
                bool alwaysSameAngleCondition = (i < NUM_OF_TARGETS)
                    || (CheckboxManager.trainingAlwaysWithTheSameAngle && (current_round < CheckboxManager.number_of_training_rounds + 1));
                if (alwaysSameAngleCondition)
                {
                    radius = c_radius;
                    random_value = 0.0f;
                }

                float angle = random_value + previous_angle_additions[i % NUM_OF_TARGETS];

                float stone_angle = angle + angle_between_targets * i;  //current stone angle

                previous_angle_additions[i % NUM_OF_TARGETS] += random_value;

                //cosinus and sinus of the angle
                float angle_cos = Mathf.Cos(stone_angle);   //Mathf.Rad2Deg
                float angle_sin = Mathf.Sin(stone_angle);   //Mathf.Rad2Deg

                //x axis calculation
                float x = center + radius * angle_cos;

                //z axis calculation
                float z = center + radius * angle_sin;

                cloned_targets[i].transform.position = new Vector3(x, -1.0f, z);

                //for data extraction
                angle_distance_x_y_data.Add(((stone_angle * Mathf.Rad2Deg) % 360, radius, x, z, cloned_targets[i].name));
            }
        }
    }

    //<----------------------METHODS TO USE THE TARGET CONTROLLER-------------------------->

    //method to change the targets to active or disabled
    private void ChangeTargetsStatus(bool status, int round)
    {
        for (int i = round * 4; i < (round + 1) * 4; i++)
        {
            cloned_targets[i].SetActive(status);
        }
    }

    public void ActivateTargets(int round) { ChangeTargetsStatus(ACTIVE, round); }                //activate the targets
    public void DeactivateTargets(int round) { ChangeTargetsStatus(DISABLED, round); }            //deactivate the targets

    public void ActivateAllTargets()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < num_of_rounds; i++) { ActivateTargets(i); }
    }

    public void DeactivateAllTargets()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < num_of_rounds; i++) { DeactivateTargets(i); }
    }

    //method to get the target objects
    public List<GameObject> GetTargetGameObjects()
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < cloned_targets.Count; i++)
        {
            result.Add(cloned_targets[i]);
        }

        return result;
    }



    //<------------------------METHODS FOR DATA EXTRACTION--------------------------->
    public (float, float, float, float) GetAngleDistanceXYTuple(string identifier)
    {
        for (int i = 0; i < angle_distance_x_y_data.Count; i++)
        {
            if (angle_distance_x_y_data[i].Identifier == identifier)
            {
                return (angle_distance_x_y_data[i].Angle, angle_distance_x_y_data[i].Distance,
                    angle_distance_x_y_data[i].X, angle_distance_x_y_data[i].Y);
            }
        }

        return (float.NaN, float.NaN, float.NaN, float.NaN);
    }
}
