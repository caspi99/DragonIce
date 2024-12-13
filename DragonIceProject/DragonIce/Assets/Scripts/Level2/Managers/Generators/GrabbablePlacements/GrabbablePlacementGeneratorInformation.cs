using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbablePlacementGeneratorInformation : MonoBehaviour, GeneratorInformationLevel2
{
    //<--------------------POSITIONS------------------------->
    private Vector3[] positions_placements_phase1 = new Vector3[] {
        new Vector3(42.03f, -0.5f, 72.94f),
        new Vector3(42f, -0.5f, 57.2f),
        new Vector3(42.3f, -0.5f, 15f),
        new Vector3(42.33f, -0.5f, 36.88f),


        new Vector3(41.9f, -0.5f, 67.4f),
        new Vector3(41.5f, -0.5f, 52.5f),
        new Vector3(42.5f, -0.5f, 20.4f),
        new Vector3(42.53f, -0.5f, 30.97f),


        new Vector3(42.03f, -0.5f, 62f),
        new Vector3(41.8f, -0.5f, 47.8f),
        new Vector3(42.5f, -0.5f, 25.9f),
        new Vector3(42.03f, -0.5f, 42.8f),


        new Vector3(51.5f, -0.5f, 72.65f),
        new Vector3(51.5f, -0.5f, 57.3f),
        new Vector3(52f, -0.5f, 15.1f),
        new Vector3(51.7f, -0.5f, 37.31f),


        new Vector3(51.5f, -0.5f, 67.4f),
        new Vector3(51.6f, -0.5f, 52.7f),
        new Vector3(52f, -0.5f, 20.6f),
        new Vector3(52.0f, -0.5f, 31.6f),


        new Vector3(51.5f, -0.5f, 62.3f),
        new Vector3(51.3f, -0.5f, 48f),
        new Vector3(52f, -0.5f, 26.14f),
        new Vector3(51.5f, -0.5f, 42.7f)

    };

    private Vector3[] positions_placements_phase2 = new Vector3[] {
        new Vector3(45.9f, 4.2f, 36.42f),
        new Vector3(45.9f, 4.2f, 19.34f),
        new Vector3(55.44f, 4.2f, 19.34f),
        new Vector3(55.44f, 4.2f, 36.42f)
    };

    private Vector3[] positions_placements_phase3 = new Vector3[] {
        new Vector3(60.0f, 3.0f, 34.0f),
        new Vector3(80.0f, 3.0f, 22.0f),
        new Vector3(80.0f, 3.0f, 34.0f),
        new Vector3(60.0f, 3.0f, 22.0f)
    };

    //<--------------------TARGET PREFAB------------------------->
    [SerializeField]
    private List<GameObject> stone_placements_phase1_prefabs;

    [SerializeField]
    private List<GameObject> tree_placements_phase2_prefabs;

    [SerializeField]
    private List<GameObject> stone_placements_phase3_prefabs;

    //<--------------------METHODS------------------------->
    public List<PositionList> GetPositionLists()
    {
        List<PositionList> position_lists = new List<PositionList>();

        position_lists.Add(new PositionList(positions_placements_phase1));
        position_lists.Add(new PositionList(positions_placements_phase2));
        position_lists.Add(new PositionList(positions_placements_phase3));

        return position_lists;
    }

    public List<RotationList> GetRotationLists()
    {
        return new List<RotationList>();
    }

    public List<PrefabList> GetPrefabLists()
    {
        List<PrefabList> prefab_lists = new List<PrefabList>();

        prefab_lists.Add(new PrefabList(stone_placements_phase1_prefabs));
        prefab_lists.Add(new PrefabList(tree_placements_phase2_prefabs));
        prefab_lists.Add(new PrefabList(stone_placements_phase3_prefabs));

        return prefab_lists;
    }

    public int[] GetNumberOfInstances()
    {
        return new int[]
        {
            GameConstants.MAX_STONES_PHASE_1,
            GameConstants.MAX_BRIDGE_SECTIONS_PHASE_2,
            CheckboxManagerLevel2.num_of_stones_phase3
        };
    }
}
