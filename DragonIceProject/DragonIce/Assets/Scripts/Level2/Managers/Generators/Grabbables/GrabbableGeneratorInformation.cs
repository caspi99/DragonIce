using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableGeneratorInformation : MonoBehaviour, GeneratorInformationLevel2
{
    //Constants to make more visible the code
    private const bool INDIVIDUAL = false;
    private const bool COLLABORATIVE = true;

    //<--------------------POSITIONS------------------------->
    private Vector3[] positions_phase1 = new Vector3[] {
        new Vector3(20.0f, 1.431f, 30.0f),
        new Vector3(18.0f, 1.431f, 37.0f),
        new Vector3(15.68f, 1.431f, 44.43f),
        new Vector3(28.87f, 1.431f, 53.9f),

        new Vector3(26.0f, 1.431f, 62.0f),
        new Vector3(20.0f, 1.431f, 54.0f),
        new Vector3(25.0f, 1.431f, 46.0f),
        new Vector3(30.0f, 1.431f, 38.0f),

        new Vector3(31.0f, 1.431f, 80.0f),
        new Vector3(18.0f, 1.431f, 66.0f),
        new Vector3(24.0f, 1.431f, 76.0f),
        new Vector3(29.0f, 1.431f, 69.0f),

        new Vector3(73.0f, 1.431f, 29.0f),
        new Vector3(72.0f, 1.431f, 19.0f),
        new Vector3(82.0f, 1.431f, 30.0f),
        new Vector3(83.0f, 1.431f, 68.0f),

        new Vector3(75.0f, 1.431f, 63.0f),
        new Vector3(80.0f, 1.431f, 55.0f),
        new Vector3(66.0f, 1.431f, 47.0f),
        new Vector3(74.0f, 1.431f, 48.0f),

        new Vector3(87.0f, 1.431f, 50.0f),
        new Vector3(63f, 1.431f, 85.0f),
        new Vector3(71.0f, 1.431f, 78.0f),
        new Vector3(66.0f, 1.431f, 62.0f)
    };

    private float[] rotations_phase1 = new float[]
    {
        160f, 179f, 142f, 347f,

        171f, 106f, 228f, 132f,

        246f, 120f, 197f, 327f,

        164f, 101f, 66f, 202f,

        314f, 285f, 77f, 246f,

        110f, 320f, 210f, 303f
    };

    private bool[] playStyles_phase1 = new bool[]
    {
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        COLLABORATIVE, COLLABORATIVE, COLLABORATIVE, COLLABORATIVE,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        COLLABORATIVE, COLLABORATIVE, COLLABORATIVE, COLLABORATIVE
    };

    private Vector3[] positions_phase2 = new Vector3[] {
        new Vector3(33.3f, 2.6f, 68.16f),
        new Vector3(18.0f, 2.6f, 56.686f),
        new Vector3(20.0f, 2.6f, 60.0f),
        new Vector3(88.0f, 2.6f, 58.9f),

        new Vector3(25.0f, 2.6f, 70.0f),
        new Vector3(32.29f, 2.6f, 79.22f),
        new Vector3(26.0f, 2.6f, 79.0f),
        new Vector3(32.91f, 2.6f, 63.28f),

        new Vector3(32.26f, 2.6f, 56.54f),
        new Vector3(84.7f, 2.6f, 44.6f),
        new Vector3(20.0f, 2.6f, 65.0f),
        new Vector3(33.8f, 2.6f, 77.1f),

        new Vector3(76.0f, 2.6f, 52.0f),
        new Vector3(80.0f, 2.6f, 48.0f),
        new Vector3(67.0f, 2.6f, 76.0f),
        new Vector3(77.0f, 2.6f, 60.0f),

        new Vector3(75.0f, 2.6f, 73.0f),
        new Vector3(68.0f, 2.6f, 80.0f),
        new Vector3(68.81917f, 2.6f, 86.0f),
        new Vector3(36.25f, 2.6f, 86.27f),

        new Vector3(87.0f, 2.6f, 54.0f),
        new Vector3(18.4f, 2.6f, 52.5f),
        new Vector3(72.0f, 2.6f, 63.0f),
        new Vector3(83.0f, 2.6f, 68.0f)
    };

    private float[] rotations_phase2 = new float[]
    {
        -18.746f, 200.0f, -167.267f, 44.795f,

        166.746f, 145.617f, 141.303f, -22.784f,

        140.0f, 95.0f, -167.544f, 130f,

        127.155f, -62.028f, -8.95f, -53.737f,

        143.638f, -2.472f, -169.001f, 175.0f,

        51.179f, 60f, -64.953f, 37.197f
    };

    private bool[] playStyles_phase2 = new bool[]
    {
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        COLLABORATIVE, COLLABORATIVE, COLLABORATIVE, COLLABORATIVE,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL,
        COLLABORATIVE, COLLABORATIVE, COLLABORATIVE, COLLABORATIVE
    };

    private Vector3[] positions_phase3 = new Vector3[] {
        new Vector3(20.0f, 3.0f, 34.0f),
        new Vector3(40.0f, 3.0f, 22.0f),
        new Vector3(40.0f, 3.0f, 34.0f),
        new Vector3(20.0f, 3.0f, 22.0f)
    };

    private float[] rotations_phase3 = new float[] {
        160f, 179f, 142f, 347f
    };

    private bool[] playStyles_phase3 = new bool[]
    {
        INDIVIDUAL, INDIVIDUAL, INDIVIDUAL, INDIVIDUAL
    };

    //<--------------------TARGET PREFAB------------------------->
    [SerializeField]
    private List<GameObject> stones_phase1_prefabs;

    [SerializeField]
    private List<GameObject> trees_phase2_prefabs;

    [SerializeField]
    private List<GameObject> stones_phase3_prefabs;

    //<--------------------METHODS------------------------->
    public List<PositionList> GetPositionLists()
    {
        List<PositionList> position_lists = new List<PositionList>();

        position_lists.Add(new PositionList(positions_phase1));
        position_lists.Add(new PositionList(positions_phase2));
        position_lists.Add(new PositionList(positions_phase3));

        return position_lists;
    }

    public List<RotationList> GetRotationLists()
    {
        List<RotationList> rotation_lists = new List<RotationList>();

        rotation_lists.Add(new RotationList(rotations_phase1));
        rotation_lists.Add(new RotationList(rotations_phase2));
        rotation_lists.Add(new RotationList(rotations_phase3));

        return rotation_lists;
    }

    public List<PrefabList> GetPrefabLists()
    {
        List<PrefabList> prefab_lists = new List<PrefabList>();

        prefab_lists.Add(new PrefabList(stones_phase1_prefabs));
        prefab_lists.Add(new PrefabList(trees_phase2_prefabs));
        prefab_lists.Add(new PrefabList(stones_phase3_prefabs));

        return prefab_lists;
    }

    public int[] GetNumberOfInstances()
    {
        return new int[]
        {
            CheckboxManagerLevel2.num_of_stones_phase1, 
            CheckboxManagerLevel2.num_of_trees_phase2, 
            CheckboxManagerLevel2.num_of_stones_phase3 
        };
    }

    public List<PlayStyleStatusList> GetPlayStyleStatusLists()
    {
        List<PlayStyleStatusList> playStyleStatus_lists = new List<PlayStyleStatusList>();

        playStyleStatus_lists.Add(new PlayStyleStatusList(playStyles_phase1));
        playStyleStatus_lists.Add(new PlayStyleStatusList(playStyles_phase2));
        playStyleStatus_lists.Add(new PlayStyleStatusList(playStyles_phase3));

        return playStyleStatus_lists;
    }
}
