using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablaGeneratorInformation : MonoBehaviour, GeneratorInformationLevel2
{
    //<--------------------POSITIONS------------------------->
    private Vector3[] positions_tablas = new Vector3[] {
        new Vector3(45.48f, 1.77f, 33.9f + 6.9f),
        new Vector3(45.48f, 1.77f, 15.5f + 6.9f),
        new Vector3(55.54f, 1.77f, 15.5f + 6.9f),
        new Vector3(55.54f, 1.77f, 33.9f + 6.9f),

        new Vector3(45.48f, 1.77f, 31.6f + 6.9f),
        new Vector3(45.48f, 1.77f, 13.2f + 6.9f),
        new Vector3(55.54f, 1.77f, 13.2f + 6.9f),
        new Vector3(55.54f, 1.77f, 31.6f + 6.9f),

        new Vector3(45.48f, 1.77f, 29.3f + 6.9f),
        new Vector3(45.48f, 1.77f, 10.9f + 6.9f),
        new Vector3(55.54f, 1.77f, 10.9f + 6.9f),
        new Vector3(55.54f, 1.77f, 29.3f + 6.9f),

        new Vector3(45.48f, 1.77f, 27.0f + 6.9f),
        new Vector3(45.48f, 1.77f, 8.6f + 6.9f),
        new Vector3(55.54f, 1.77f, 8.6f + 6.9f),
        new Vector3(55.54f, 1.77f, 27.0f + 6.9f),

        new Vector3(45.48f, 1.77f, 24.7f + 6.9f),
        new Vector3(45.48f, 1.77f, 6.3f + 6.9f),
        new Vector3(55.54f, 1.77f, 6.3f + 6.9f),
        new Vector3(55.54f, 1.77f, 24.7f + 6.9f),

        new Vector3(45.48f, 1.77f, 22.4f + 6.9f),
        new Vector3(45.48f, 1.77f, 4.0f + 6.9f),
        new Vector3(55.54f, 1.77f, 4.0f + 6.9f),
        new Vector3(55.54f, 1.77f, 22.4f + 6.9f)
    };

    //<--------------------TARGET PREFAB------------------------->
    [SerializeField]
    private List<GameObject> tablas_prefabs;

    //<--------------------METHODS------------------------->
    public List<PositionList> GetPositionLists()
    {
        List<PositionList> position_lists = new List<PositionList>();

        position_lists.Add(new PositionList(positions_tablas));

        return position_lists;
    }

    public List<RotationList> GetRotationLists()
    {
        return new List<RotationList>();
    }

    public List<PrefabList> GetPrefabLists()
    {
        List<PrefabList> prefab_lists = new List<PrefabList>();

        prefab_lists.Add(new PrefabList(tablas_prefabs));

        return prefab_lists;
    }

    public int[] GetNumberOfInstances()
    {
        return new int[]
        {
            GameConstants.MAX_TABLAS_PHASE_2
        };
    }
}
