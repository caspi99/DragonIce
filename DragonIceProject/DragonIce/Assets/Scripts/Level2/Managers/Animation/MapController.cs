using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> maps;

    [SerializeField]
    private List<GameObject> visuals;

    /*
    [SerializeField]
    private List<GameObject> visual_stone_phase1_prefabs;

    [SerializeField]
    private List<GameObject> visual_tablas_phase2_prefabs;

    [SerializeField]
    private List<GameObject> visual_stone_phase3_prefabs;

    private Vector3[] positions_visual_stones_phase1 = new Vector3[] {
        new Vector3(509.53f, 12.76f, 790.8721f),
        new Vector3(42.9f, 3.0f, 82.94f),
        new Vector3(52.3f, 3.0f, 24.76f),
        new Vector3(42.9f, 3.0f, 45.88f),

        new Vector3(51.6f, 3.0f, 30.2f),
        new Vector3(52.3f, 3.0f, 83.42f),
        new Vector3(42.9f, 3.0f, 40.37f),
        new Vector3(43.2f, 3.0f, 77.4f),

        new Vector3(52.3f, 3.0f, 56.6f),
        new Vector3(52.3f, 3.0f, 67.65f),
        new Vector3(42.9f, 3.0f, 56.55f),
        new Vector3(52.3f, 3.0f, 72.45f),

        new Vector3(42.2f, 3.0f, 35.4f),
        new Vector3(52.3f, 3.0f, 40.3f),
        new Vector3(42.9f, 3.0f, 72.22f),
        new Vector3(52.3f, 3.0f, 46.01f),

        new Vector3(42.6f, 3.0f, 51.1f),
        new Vector3(52.3f, 3.0f, 51.5f),
        new Vector3(52.3f, 3.0f, 35.14f),
        new Vector3(52.3f, 3.0f, 62.4f),

        new Vector3(42.9f, 3.0f, 66.8f),
        new Vector3(42.9f, 3.0f, 61.5f),
        new Vector3(52.3f, 3.0f, 78.01f),
        new Vector3(42.5f, 3.0f, 29.7f)
    };

    private Vector3[] positions_visual_tablas_phase2 = new Vector3[] {};

    private Vector3[] positions_visual_stones_phase3 = new Vector3[] {};
    */

    // Start is called before the first frame update
    void Start()
    {
        //InitVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //<-------VISUALS GENERATOR------>
    /*
    //method to init all the stones
    private void InitVisuals()
    {
        for (int i = 0; i < GameConstants.MAX_STONES_PHASE_1; i++)
        {
            GenerateClone(i, GameConstants.PHASE_1, i % 4);
        }

        for (int i = 0; i < GameConstants.MAX_BRIDGE_PHASE_2; i++)
        {
            GenerateClone(i, GameConstants.PHASE_2, i);
        }

        for (int i = 0; i < GameConstants.MAX_STONES_PHASE_3; i++)
        {
            GenerateClone(i, GameConstants.PHASE_3, i % 4);
        }
    }

    private GameObject GetPrefab(int phase, int shape)
    {
        switch (phase)
        {
            case GameConstants.PHASE_1:
                return visual_stone_phase1_prefabs[shape];
            case GameConstants.PHASE_2:
                return tree_placements_phase2_prefabs[shape];
            case GameConstants.PHASE_3:
                return stone_placements_phase3_prefabs[shape];

            default:
                return null;
        }
    }

    private Vector3 GetPosition(int phase, int internal_idx)
    {
        switch (phase)
        {
            case GameConstants.PHASE_1:
                return positions_visual_stones_phase1[internal_idx];
            case GameConstants.PHASE_2:
                return positions_placements_phase2[internal_idx];
            case GameConstants.PHASE_3:
                return positions_placements_phase3[internal_idx];

            default:
                return Vector3.zero;
        }
    }

    //method to generate a clone
    private void GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        Quaternion rotation = Quaternion.identity;

        if (grabbable_phase == GameConstants.PHASE_1) { rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f); }

        GameObject clone = Instantiate(GetPrefab(grabbable_phase, shape), GetPosition(grabbable_phase, internal_idx), rotation);    //we generate the clone
        clone.GetComponent<GrabbablePlacement>().InitGrabbablePlacement();    //we init the grabbable placement
    }

    private void ChangeAllVisualsStatus(int phase, bool status, bool all_objects = false)
    {
        List<GrabbablePlacement> current_grabbable_placements = GetGrabbablePlacementsOfCurrentPhase(phase);

        int num_of_iterations = num_of_grabbables[phase];

        if (all_objects) { num_of_iterations = current_grabbable_placements.Count; }

        for (int i = 0; i < num_of_iterations; i++)
        {
            if (current_grabbable_placements[i].gameObject.activeSelf != status)
            {
                current_grabbable_placements[i].gameObject.SetActive(status);
            }

        }
    }
    */

    //<-------METHODS TO USE EXTERNALLY------>

    //<-------MAP------>

    private void ChangeMapStatus(int map_idx, bool status) { maps[map_idx].SetActive(status); }

    public void ShowMap(int map_idx) { ChangeMapStatus(map_idx, true); }
    public void HideMap(int map_idx) { ChangeMapStatus(map_idx, false); }

    public void ChangeMapPosition(int map_idx, Vector3 position)
    {
        maps[map_idx].transform.position = position;
    }

    public Vector3 GetCurrentPosition(int map_idx) { return maps[map_idx].transform.position; }

    public void Translation(int map_idx, Vector3 position_increment)
    {
        ChangeMapPosition(map_idx, GetCurrentPosition(map_idx) + position_increment);
    }

    public void ChangeMapScale(int map_idx, float input_scale)
    {
        maps[map_idx].transform.localScale = new Vector3(input_scale, input_scale, input_scale);
    }

    public float GetCurrentScale(int map_idx) { return maps[map_idx].transform.localScale.x; }

    public void AddZoom(int map_idx, float zoom_increment)
    {
        ChangeMapScale(map_idx, GetCurrentScale(map_idx) + zoom_increment);
    }


    //<-------VISUALS------>

    private void ChangeVisualStatus(int visual_idx, bool status) { visuals[visual_idx].SetActive(status); }

    public void ShowVisual(int visual_idx) { ChangeVisualStatus(visual_idx, true); }
    public void HideVisual(int visual_idx) { ChangeVisualStatus(visual_idx, false); }

}
