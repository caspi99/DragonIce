using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGeneratorInformation : MonoBehaviour, GeneratorInformationLevel2
{
    [SerializeField]
    private List<GameObject> smoke_prefab;

    //<--------------------METHODS------------------------->
    public List<PositionList> GetPositionLists()
    {
        List<PositionList> position_lists = new List<PositionList>();

        List<Vector3> positions = new List<Vector3>();

        for(int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
        {
            float random_offset_x = GameConstants.MAX_OFFSET_SMOKE * Random.Range(0.0f, 1.0f);
            float random_offset_z = GameConstants.MAX_OFFSET_SMOKE * Random.Range(0.0f, 1.0f);

            Vector3 offset = new Vector3(random_offset_x, 0.0f, random_offset_z);
            positions.Add(GameConstants.SMOKE_BASE_POSITION + offset);
        }

        position_lists.Add(new PositionList(positions.ToArray()));

        return position_lists;
    }

    public List<RotationList> GetRotationLists()
    {
        return new List<RotationList>();
    }

    public List<PrefabList> GetPrefabLists()
    {
        List<PrefabList> prefab_lists = new List<PrefabList>();

        prefab_lists.Add(new PrefabList(smoke_prefab));

        return prefab_lists;
    }

    public int[] GetNumberOfInstances()
    {
        return new int[]
        {
            GameConstants.SMOKE_INSTANCES
        };
    }
}
