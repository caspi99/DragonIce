using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceController : MonoBehaviour
{
	//<------------------------ICE PLANE VARIABLES--------------------------->
	public List<GameObject> ice_game_objects = new List<GameObject>();

	//<------------------------ICE PLANE VARIABLES--------------------------->
	public const int ICE_PLANE = 0;
	public const int ICE_HOLE_PLANE = 1;
	public const int ICE_HOLE_CENTRAL_PLANE = 2;
	public const int FINAL_CRACKS = 3;
	public const int ICE_CRACKS = 4;
	public const int BACKGROUND = 5;

	private int remapIce;			//variable to control the remap of the ice
	private int remapIceCentral;    //variable to control the remap of the central ice

	private List<int> timers;  //timer for lake shine
	private List<bool> shine_lake; //bool to know if the lake has to shine or not
	private int shine_lake_idx = -1;

    private float wait_time_shine_lake = 1.0f;

    private TimeManager time_c;  //time controller

    // Start is called before the first frame update
    void Start()
    {
        remapIce = 0; remapIceCentral = 80;

		//init timer and control bool for shine lake
        time_c = GameObject.Find("TimeManager").GetComponent<TimeManager>();

		InitShineControl();

        if (CheckboxManager.skip_story_sequences)
		{
			ShowObject(ICE_PLANE);
			ShowObject(ICE_HOLE_CENTRAL_PLANE);
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (shine_lake_idx >= 0)
		{
            if (shine_lake[shine_lake_idx])
            {
                if (time_c.WaitTimeWithReset(timers[shine_lake_idx], wait_time_shine_lake)) { shine_lake[shine_lake_idx] = false; }
            }

            ChangeEmissiveColorLake();
        }
    }

	//<----------------------METHODS THAT USES THE ICE CONTROLLER-------------------------->

	//private bool GreaterThanFloat(float float1, float float2) { return float1 > float2; }       //internal method mostly used to check the time
	private bool GreaterThanInt(int int1, int int2) { return int1 > int2; }                     //internal method mostly used to check the remap

	//method to get the renderer of a GameObject
	private Renderer GetRenderer(int object_idx) { return ice_game_objects[object_idx].GetComponent<Renderer>(); }

	//<----------------------METHODS TO USE THE ICE CONTROLLER-------------------------->

	//Method to freeze the water, first part, where the humans are in the animator state "KnightWires"
	public void FreezeWater1()
	{
		if(GreaterThanInt(150, remapIce))
        {
			Renderer renderer = GetRenderer(ICE_HOLE_PLANE);

			renderer.material.SetFloat("RemapParam_", remapIce);
			remapIce++;
		}
	}

	//Method to freeze the water, second part, where the humans are in the animator state "KnightHole"
	public void FreezeWater2()
	{
		if (!ice_game_objects[ICE_HOLE_PLANE].activeInHierarchy) { ShowObject(ICE_HOLE_CENTRAL_PLANE); }

		if (GreaterThanInt(1000, remapIce))
		{
			Renderer renderer = GetRenderer(ICE_HOLE_PLANE);

			renderer.material.SetFloat("RemapParam_", remapIce); //gel exterior
			remapIce += 3;
		}

		if (GreaterThanInt(remapIce, 990))
		{
			Renderer renderer = GetRenderer(ICE_HOLE_CENTRAL_PLANE);

			renderer.material.SetFloat("RemapParam_", remapIceCentral); //gel exterior
			remapIceCentral += 10;
		}

		if (GreaterThanInt(remapIce, 1000) && GreaterThanInt(remapIceCentral, 500)) //acabar la congelació
		{
			HideObject(ICE_HOLE_CENTRAL_PLANE);
			HideObject(ICE_HOLE_PLANE);
			ShowObject(ICE_PLANE);
		}
	}

	//method to disable all the ice cracks
	public void DisableAllIceCracks()
	{
		List<GameObject> result = new List<GameObject>();
		result.AddRange(GameObject.FindGameObjectsWithTag("IceCrack"));

		for (int i = 0; i < result.Count; i++)
		{
			result[i].SetActive(false);
		}
	}

	//method to change the visibility of a game object
	public void ChangeVisibility(int object_idx, bool status) { ice_game_objects[object_idx].SetActive(status); }

	public void ShowObject(int object_idx) { ChangeVisibility(object_idx, true); }
	public void HideObject(int object_idx) { ChangeVisibility(object_idx, false); }

	//<-------METHODS TO BRIGHT THE LAKE------->

	private void InitShineControl()
	{
		shine_lake = new List<bool>();
        timers = new List<int>();

        int num_of_rounds = CheckboxManager.number_of_tutorial_rounds + CheckboxManager.number_of_true_rounds;

        for (int i = 0; i < num_of_rounds; i++)
		{
			shine_lake.Add(Settings.sync && CheckboxManager.light_lake);

            timers.Add(time_c.CreateTimer()); time_c.PauseTimer(timers[i]);
        }
	}

    //method to calculate the light intensity of the "blink" effect of the lake
    private float CalculateBlink()
    {
        float coeff = time_c.GetTime(timers[shine_lake_idx]);

        if (coeff > wait_time_shine_lake / 2)
        {
            return ((wait_time_shine_lake - coeff) / wait_time_shine_lake) * 2.0f;
        }
        else
        {
            return (coeff / wait_time_shine_lake) * 2.0f;
        }
    }

    //method to change emissive color of the lake
    private void ChangeEmissiveColorLake()
    {
        Renderer renderer = ice_game_objects[BACKGROUND].GetComponent<Renderer>();

        if (shine_lake[shine_lake_idx])
        {
            //LIGHT
            renderer.material.SetColor("_EmissiveColor", Color.white * CalculateBlink() * CheckboxManager.lake_light_intensity);
        }
        else
		{
            renderer.material.SetColor("_EmissiveColor", Color.black);
		}
    }

	//method to activate the shine of the lake
	public void ShineLake(int current_round){ shine_lake_idx = current_round; }
}
