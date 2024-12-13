using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour
{
    [SerializeField] private SmokeGenerator smoke_generator;

    private List<GameObject> smoke_gameobjects;

    private List<Vector3> init_positions;
    private List<Vector3> final_positions;

    //private float max_distance; //to calculate speed of smokes later

    // Start is called before the first frame update
    void Start()
    {
        smoke_generator.InitGenerator();

        smoke_gameobjects = GetAllSmokeFromScene();

        init_positions = smoke_generator.GetInitPositions();
        InitFinalPositions();
        //CalculateMaxDistance();
    }

    //Method to get all the grabbables of the scene
    private List<GameObject> GetAllSmokeFromScene()
    {
        List<GameObject> result = new List<GameObject>();
        result.AddRange(GameObject.FindGameObjectsWithTag("SmokeInstance"));

        return result;
    }

    //Method to init the final_positions
    private void InitFinalPositions()
    {
        final_positions = new List<Vector3>();

        for(int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
        {
            int row = i / GameConstants.SMOKE_NUM_OF_ROWS;
            int column = i % GameConstants.SMOKE_NUM_OF_ROWS;

            float x = row * GameConstants.DISTANCE_BETWEEN_SMOKES;
            float y = GameConstants.SMOKE_BASE_POSITION.y;
            float z = column * GameConstants.DISTANCE_BETWEEN_SMOKES;

            final_positions.Add(new Vector3(x, y, z));
        }
    }

    //method to get all the renderers from a GameObject
    private MeshRenderer GetRenderer(int smoke_idx)
    {
        return smoke_gameobjects[smoke_idx].GetComponentInChildren<MeshRenderer>();
    }

    //method to change smoke visibility
    private void FlipSmokeTransparency(int smoke_idx, bool status)
    {
        MeshRenderer renderer = GetRenderer(smoke_idx);

        Color col = renderer.material.color;

        float value = 0f;

        if (status) { value = 255f; }

        col.a = value;

        renderer.material.color = col;
    }

    public void ShowSmoke(int smoke_idx) { FlipSmokeTransparency(smoke_idx, true); }
    public void HideSmoke(int smoke_idx) { FlipSmokeTransparency(smoke_idx, false); }

    //method to change smoke visibility
    public void ChangeSmokeTransparencyProgressive(float seconds, int smoke_idx, bool alpha_descending = false)
    {
        MeshRenderer renderer = GetRenderer(smoke_idx);

        Color col = renderer.material.color;

        float dt = Time.deltaTime;

        if (alpha_descending) { dt = -1 * dt; }

        col.a += dt / seconds;

        renderer.material.color = col;
    }

    public void ChangeSmokePosition(int smoke_idx, Vector3 position)
    {
        smoke_gameobjects[smoke_idx].transform.position = position;
    }

    public Vector3 GetCurrentPosition(int smoke_idx) { return smoke_gameobjects[smoke_idx].transform.position; }
    public Vector3 GetInitPosition(int smoke_idx) { return init_positions[smoke_idx]; }
    public Vector3 GetFinalPosition(int smoke_idx) { return final_positions[smoke_idx]; }

    public void Translation(int smoke_idx, Vector3 position_increment)
    {
        ChangeSmokePosition(smoke_idx, GetCurrentPosition(smoke_idx) + position_increment);
    }

    /*
    private void CalculateMaxDistance()
    {
        max_distance = -1f;

        for(int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
        {
            float curr_distance = Vector3.Distance(GetCurrentPosition(i), GetFinalPosition(i));

            max_distance = Mathf.Max(max_distance, curr_distance);
        }
    }

    public float GetMaxDistance() { return max_distance; }
    */

    //public void ShowSmokeProgressive(float seconds, int smoke_idx) { ChangeSmokeTransparencyProgressive(seconds, smoke_idx); }
    //public void HideSmokeProgressive(float seconds, int smoke_idx) { ChangeSmokeTransparencyProgressive(seconds, smoke_idx, true); }
}