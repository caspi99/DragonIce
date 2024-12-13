using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStoneController : MonoBehaviour
{
    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<-------------------STONE INDEXES------------------------>
    const int STONE_1 = 0;
    const int STONE_2 = 1;
    const int STONE_3 = 2;
    const int STONE_4 = 3;

    private const int NUM_OF_STONES = 4;

    //<--------------------STONE VARIABLES------------------------->
    private List<float> speed = new List<float>();  //speed of every stone at every round

    //<------------------------STONES--------------------------->
    private List<GameObject> cloned_stones = new List<GameObject>();         //cloned stones list
    private List<Vector3> stones_vector_direction = new List<Vector3>();     //stones vector direction list
    private List<bool> stones_kicked = new List<bool>();                     //stones kicked vector
    private List<Color> stones_color = new List<Color>();                    //stones color vector

    // Method to init the stone controller
    public void InitStoneController(List<GameObject> targets)
    {
        GetAllStonesOfScene();
        InitStonesVectorDirections();
        InitStonesKicked();
        InitStonesColors();
        ResetStonesPosition();
        GetStoneSpeed();
        TraceRoute(targets);
    }

    //<----------------------METHODS THAT USES THE STONES CONTROLLER-------------------------->

    public (int, int) GetAssignedPlayerAndCurrentRound(string stone_name)
    {
        string[] numbers = stone_name.Split('_');

        return (int.Parse(numbers[0]), int.Parse(numbers[1]));
    }

    //method to get all the stones from a scene
    private void GetAllStonesOfScene()
    {
        List<GameObject> result = new List<GameObject>();
        result.AddRange(GameObject.FindGameObjectsWithTag("Stone"));

        cloned_stones = result;
    }

    private float CalculateStoneSpeedSync(int round_idx)
    {
        float speed_training = CheckboxManager.stone_speed_training;
        Vector2 speed_vec_tutorial = CheckboxManager.stone_speed_tutorial_interval;
        Vector2 speed_vec_true = CheckboxManager.stone_speed_true_interval;

        if(round_idx < CheckboxManager.number_of_training_rounds) { return speed_training; }

        if (round_idx < CheckboxManager.numOfRoundsPrevToTruePhase)
        {
            int num_of_rounds = CheckboxManager.numOfRoundsPrevToTruePhase;
            int local_round_idx = round_idx - CheckboxManager.number_of_training_rounds;

            float delta = (speed_vec_tutorial.y - speed_vec_tutorial.x) / num_of_rounds;

            return speed_vec_tutorial.x + delta * local_round_idx;
        }

        return speed_vec_true.x;
    }

    private float CalculateStoneSpeedAsync(int round_idx)
    {
        float speed_training = CheckboxManager.stone_speed_training;
        Vector2 speed_vec_tutorial = CheckboxManager.stone_speed_tutorial_interval;
        Vector2 speed_vec_true = CheckboxManager.stone_speed_true_interval;

        if (round_idx < CheckboxManager.number_of_training_rounds) { return speed_training; }

        if (round_idx < CheckboxManager.numOfRoundsPrevToTruePhase)
        {
            return Random.Range(speed_vec_tutorial.x, speed_vec_tutorial.y);
        }

        return Random.Range(speed_vec_true.x, speed_vec_true.y);
    }

    //method to get the stone speed from the checkboxes
    private void GetStoneSpeed()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < NUM_OF_STONES * num_of_rounds; i++) {

            int round_idx = i / NUM_OF_STONES;

            if (Settings.sync) { speed.Add(CalculateStoneSpeedSync(round_idx)); }
            else { speed.Add(CalculateStoneSpeedAsync(round_idx));}
        }
    }

    //method to init the stones vector directions list
    private void InitStonesVectorDirections()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < NUM_OF_STONES * num_of_rounds; i++) { stones_vector_direction.Add(new Vector3(0.0f, 0.0f, 0.0f)); }
    }

    //method to init the stones kicked list
    private void InitStonesKicked()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < NUM_OF_STONES * num_of_rounds; i++) { stones_kicked.Add(DISABLED); }
    }

    //method to init the stones color list
    private void InitStonesColors()
    {
        stones_color.Add(new Color(1f, 0.357f, 0.204f));
        stones_color.Add(new Color(0.204f, 1f, 0.525f));
        stones_color.Add(new Color(0.537f, 0.282f, 1f));
        stones_color.Add(new Color(1f, 0.902f, 0.204f));
    }

    //method to update the list of the kicked stones
    public void UpdateStoneKicks()
    {
        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int p, int r) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            int idx = p + (r * 4);

            stones_kicked[idx] = cloned_stones[i].GetComponent<Stone>().StoneHasBeenHitted();
        }
    }

    private void ChangeAlpha(int idx, float a_value)
    {
        Renderer renderer = cloned_stones[idx].GetComponent<Renderer>();

        //ALPHA
        Color color = renderer.material.color;
        color.a = a_value;
        renderer.material.color = color;
    }

    //method to change emissive color of a stone
    private void ChangeEmissiveColor(int idx, bool light, float intensity, int color_idx)
    {
        Renderer renderer = cloned_stones[idx].GetComponent<Renderer>();

        if (light)
        {
            //LIGHT
            renderer.material.SetColor("_EmissiveColor", stones_color[color_idx] * intensity);
        }
        else
        {
            renderer.material.SetColor("_EmissiveColor", Color.black);
        }
    }

    //method to calculate the direction of a stone
    private Vector3 CalculateStoneVector(Vector3 stone_position, Vector3 target_position)
    {
        //Vector3 result = new Vector3(target_position.x, 0.0f, target_position.z) - new Vector3(stone_position.x, 0.0f, stone_position.z);

        Vector3 result = target_position - stone_position;

        return result.normalized;
    }

    //method to check if the stone and the target matches
    private bool CheckMatch(string target_tag, string stone_tag) { return target_tag == stone_tag; }

    //method to assign a direction to a stone
    private void CalculateStoneVectorFromTarget(GameObject target, int idx)
    {
        stones_vector_direction[idx] = CalculateStoneVector(cloned_stones[idx].transform.position, target.transform.position);
    }

    //<----------------------METHODS TO USE THE STONES CONTROLLER-------------------------->
    private void ChangeStoneStatus(bool status, int round, int stone)
    {
        int idx = stone + round * NUM_OF_STONES;
        cloned_stones[idx].SetActive(status);
    }

    public void ActivateStone(int round, int stone) { ChangeStoneStatus(ACTIVE, round, stone); }                //activate the stones
    public void DeactivateStone(int round, int stone) { ChangeStoneStatus(DISABLED, round, stone); }            //deactivate the stones

    //method to change the targets to active or disabled
    private void ChangeStonesStatus(bool status, int round)
    {
        for (int i = round* NUM_OF_STONES; i < (round+1)* NUM_OF_STONES; i++)
        {
            cloned_stones[i].SetActive(status);
        }
    }

    public void ActivateStones(int round) { ChangeStonesStatus(ACTIVE, round); }                //activate the stones
    public void DeactivateStones(int round) { ChangeStonesStatus(DISABLED, round); }            //deactivate the stones

    public void ActivateAllStones()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < num_of_rounds; i++) { ActivateStones(i); }
    }

    public void DeactivateAllStones()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < num_of_rounds; i++) { DeactivateStones(i); }
    }

    //method to reset the stones position
    public void ResetStonesPosition()
    {
        for (int i = 0; i < cloned_stones.Count; i++)
        {
            cloned_stones[i].transform.position = new Vector3(50.0f, -1, 50.0f);
        }
    }

    //method to reset the stones kicked status
    public void ResetStonesStatus()
    {
        for (int i = 0; i < cloned_stones.Count; i++)
        {
            cloned_stones[i].GetComponent<Stone>().DisableKickAction(); cloned_stones[i].GetComponent<Stone>().ResetStoneHitted();
        }
    }

    //method to trace the route of the stones to their targets
    private void TraceRoute(List<GameObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(targets[i].name);

            int idx = a_player + (curr_round * NUM_OF_STONES);

            CalculateStoneVectorFromTarget(targets[i], idx);
        }
    }

    //method to move the stones to their targets
    public void MoveStone(bool slow, int round, int stone)
    {
        float dt = Time.deltaTime;

        float final_speed = speed[stone + round*NUM_OF_STONES];

        if (slow) { final_speed *= 0.1f; }

        //Debug.Log(final_speed);

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if((round == curr_round)&&(stone == a_player))
            {
                int idx = a_player + (curr_round * NUM_OF_STONES);

                cloned_stones[i].transform.position += stones_vector_direction[idx] * final_speed * dt;
            }
        }
    }

    /*
    //method to get if the stones are able to be kicked
    public bool GetIfStoneKickHasBeenEnabled(int index) { return stones[index].CanKick(); }

    //method to get if the stones have been kicked
    public bool GetIfStoneHasBeenHitted(int index) { return stones[index].StoneHasBeenHitted(); }
    */

    public (float, float) GetCurrentStonePosition(int stone, int round)
    {
        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if ((round == curr_round) && (stone == a_player))
            {
                return (cloned_stones[i].transform.position.x, cloned_stones[i].transform.position.z);
            }
        }
        return (float.NaN, float.NaN);
    }

    //method to know if a rock can be kicked
    public bool IsReadyToKick(int round, int stone)
    {
        bool condition = true;

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if ((round == curr_round) && (stone == a_player))
            {
                condition = condition && cloned_stones[i].GetComponent<Stone>().CanKick();
            }
        }
        return condition;
    }

    //method to get if at least one stone has been kicked
    public bool GetIfSomeStoneHasBeenKicked(int round)
    {
        bool condition = false;

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int _, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if (round == curr_round)
            {
                condition = condition | cloned_stones[i].GetComponent<Stone>().StoneHasBeenHitted();
            }
        }

        return condition;
    }

    //method to get if the four stones of a round have been kicked
    public bool GetIfAllStonesOfRoundHasBeenKicked(int round)
    {
        bool condition = true;

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int _, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if (round == curr_round)
            {
                condition = condition && cloned_stones[i].GetComponent<Stone>().StoneHasBeenHitted();
            }
        }

        return condition;
    }

    //method to get the stones transform component
    public List<Vector3> GetStonePositions(int round)
    {
        List<Vector3> stones_positions = new List<Vector3>();

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int _, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if (round == curr_round)
            {
                stones_positions.Add(cloned_stones[i].transform.position);
            }
        }

        return stones_positions;
    }
    
    //method to get if a stones has been kicked in perfect timing
    public int GetIfStonePerfectTime(int round, int stone)
    {
        int idxToFind = stone + (round * 4);

        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int p, int r) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            int idx = p + (r * 4);

            if(idxToFind == idx) { return cloned_stones[i].GetComponent<Stone>().GetPerfectTiming(); }
        }

        return 0;
    }

    //method to get if a stone has been kicked
    public bool GetIfStoneKicked(int round, int stone)
    {
        int idx = stone + (round * NUM_OF_STONES);

        return stones_kicked[idx];
    }

    //method to get the list of the kicked stones
    public List<bool> GetStoneKicks(int round)
    {
        List<bool> sub_stones_kicked = new List<bool>();

        for (int i = 0; i < cloned_stones.Count; i++) {

            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            int idx = a_player + (curr_round * NUM_OF_STONES);

            if (round == curr_round)
            {
                sub_stones_kicked.Add(stones_kicked[idx]);
            }
        }

        return sub_stones_kicked;
    }

    /*
    //method to check if the sound kick has to be played
    public bool HasBeenKickedAStone(int round)
    {
        List<bool> previous = new List<bool>();

        for (int i = round* NUM_OF_STONES; i < (round+1) * NUM_OF_STONES; i++)
        {
            previous.Add(stones_kicked[i]);
        }

        UpdateStoneKicks();

        for (int i = 0; i < previous.Count; i++)
        {
            bool condition = stones_kicked[i + (round * NUM_OF_STONES)] && (stones_kicked[i + (round * NUM_OF_STONES)] != previous[i]);

            if (condition) { return true; }
        }

        return false;
    }
    */

    //method to make the stones bright
    public void BrightStone(int round, int stone, float intensity, bool use_emissive_color, float alpha_value = 1.0f)
    {
        int idx = stone + (round * 4);

        ChangeAlpha(idx, alpha_value);

        ChangeEmissiveColor(idx, use_emissive_color, intensity, stone);
    }

    //method to make the stones bright
    public void StopBrightStone(int round, int stone)
    {
        int idx = stone + (round * 4);
        ChangeEmissiveColor(idx, DISABLED, 0.0f, 0);
    }

    //method to get the transform of a stone
    public Transform GetStoneTransform(int round, int stone)
    {
        for (int i = 0; i < cloned_stones.Count; i++)
        {
            (int a_player, int curr_round) = GetAssignedPlayerAndCurrentRound(cloned_stones[i].name);

            if ((round == curr_round) && (stone == a_player))
            {
                return cloned_stones[stone + round * NUM_OF_STONES].transform;
            }
        }
        return this.transform;
    }

    //methods for data extraction
    public float GetStoneSpeed(int stone, int round) { return speed[stone + round * NUM_OF_STONES]; }
}
