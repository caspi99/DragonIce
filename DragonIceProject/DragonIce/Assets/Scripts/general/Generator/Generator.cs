using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Generator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class GeneratorLevel1 : Generator
{
    //<--------------------DEFAULT PREFAB IDENTIFIER------------------------->
    protected List<string> prefab_default_idx = new List<string>();

    //<--------------------CONTROL INDIVIDUAL PREFAB ROUNDS------------------------->
    protected List<int> current_rounds_prefabs = new List<int>();

    //<-----------ASSIGNED PLAYERS TO EACH PREFAB-------------->
    protected List<string> prefabs_assigned_players = new List<string>();

    //<--------------------PREFAB------------------------->
    [SerializeField]
    protected GameObject prefab;

    //<----------------------METHODS THAT USES THE STONE GENERATOR-------------------------->

    //method to init all the prefabs
    protected void InitPrefabs()
    {
        int num_of_rounds = CheckboxManager.num_of_rounds;

        for (int i = 0; i < num_of_rounds; i++)
        {
            GeneratePrefabs(i);
        }
    }

    //method to init the current rounds prefabs
    protected void InitCurrentRoundsPrefabs()
    {
        for (int i = 0; i < 4; i++) { current_rounds_prefabs.Add(-1); }
    }

    //method to update the round of a given prefab
    protected void UpdateCurrentRoundOfPrefab(int stone_idx) { current_rounds_prefabs[stone_idx]++; }

    //method to check if a prefab of a given round has been generated
    protected bool CheckIfPrefabAlreadyGenerated(int current_round, int stone_idx)
    {
        return (current_round == current_rounds_prefabs[stone_idx]);
    }

    //method to check if all prefabs of a given round have been generated
    protected bool CheckIfAllPrefabsAlreadyGenerated(int current_round)
    {
        bool condition = true;

        for (int i = 0; i < current_rounds_prefabs.Count; i++) { condition = condition && CheckIfPrefabAlreadyGenerated(current_round, i); }

        return condition;
    }

    //method to generate a prefab identifier
    protected string GetPrefabIdentifier(int current_round, int target_idx)
    {
        return prefab_default_idx[target_idx] + current_round.ToString();
    }

    //method to generate a clone
    protected virtual GameObject GenerateClone(string identifier)
    {
        GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity);    //we generate the clone
        clone.name = identifier;    //we set the name of the clone as the identifier

        return clone;
    }

    //method to get the player assigned to the prefab and the round from a prefab identifier
    protected (int, int) ProcessPrefabIdentifier(string identifier)
    {
        string[] splitArray = identifier.Split(char.Parse("_"));

        return (int.Parse(splitArray[0]), int.Parse(splitArray[1]));
    }

    //method to generate a single prefab given a round
    protected void GeneratePrefab(int current_round, int stone_idx)
    {
        if (!CheckIfPrefabAlreadyGenerated(current_round, stone_idx))
        {
            UpdateCurrentRoundOfPrefab(stone_idx);

            string identifier = GetPrefabIdentifier(current_round, stone_idx);   //we generate the identifier

            GenerateClone(identifier);
        }
    }

    //method to generate prefabs given a round
    protected void GeneratePrefabs(int current_round)
    {
        for (int i = 0; i < current_rounds_prefabs.Count; i++) { GeneratePrefab(current_round, i); }
    }
}

public class PositionList
{
    public Vector3[] positions { get; private set; }
    public PositionList(Vector3[] positions) { this.positions = positions; }
}

public class RotationList
{
    public float[] rotations { get; private set; }
    public RotationList(float[] rotations) { this.rotations = rotations; }
}

public class PrefabList
{
    public List<GameObject> prefabs { get; private set; }
    public PrefabList(List<GameObject> prefabs) { this.prefabs = prefabs; }
}

public class GeneratorLevel2 : Generator
{
    protected List<PositionList> position_lists;
    protected List<RotationList> rotation_lists;
    protected List<PrefabList> prefab_lists;
    protected int[] number_of_instances;

    protected GeneratorInformationLevel2 grabbable_generator_info;

    public void InitGenerator()
    {
        GetVariables();

        for (int k = 0; k < number_of_instances.Length; k++)
        {
            for (int i = 0; i < number_of_instances[k]; i++)
            {
                GenerateClone(i, k, i % 4);
            }
        }
    }

    protected virtual void GetVariables()
    {
        grabbable_generator_info = this.gameObject.GetComponent<GeneratorInformationLevel2>();

        position_lists = grabbable_generator_info.GetPositionLists();
        rotation_lists = grabbable_generator_info.GetRotationLists();
        prefab_lists = grabbable_generator_info.GetPrefabLists();
        number_of_instances = grabbable_generator_info.GetNumberOfInstances();
    }

    //<----------------------METHODS THAT USES THE GENERATOR LEVEL 2-------------------------->

    protected Vector3 GetPosition(int grabbable_phase, int internal_idx)
    {
        return position_lists[grabbable_phase].positions[internal_idx];
    }

    protected virtual Quaternion GetRotation(int grabbable_phase, int internal_idx)
    {
        return Quaternion.Euler(0.0f, rotation_lists[grabbable_phase].rotations[internal_idx], 0.0f);
    }

    protected virtual GameObject GetPrefab(int grabbable_phase, int shape)
    {
        return prefab_lists[grabbable_phase].prefabs[shape];
    }

    //method to generate a clone
    protected virtual GameObject GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        return Instantiate(GetPrefab(grabbable_phase, shape), GetPosition(grabbable_phase, internal_idx), GetRotation(grabbable_phase, internal_idx));    //we generate the clone
    }
}