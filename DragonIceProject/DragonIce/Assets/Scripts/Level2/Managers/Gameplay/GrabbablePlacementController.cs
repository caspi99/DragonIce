using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbablePlacementController : MonoBehaviour
{
    [SerializeField]
    private GrabbablePlacementGenerator grabbable_placement_generator;

    private List<GrabbablePlacement> stone_placements_phase_1;

    private List<GrabbablePlacement> tree_placements_phase_2;

    private List<GrabbablePlacement> stone_placements_phase_3;

    private List<int> num_of_grabbables = new List<int>();
    private string[] grabbable_placements_tags = new string[] { "StonePlacementPhase1", "TreePlacementPhase2", "StonePlacementPhase3" };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //<-------GRABBABLE PLACEMENT CONTROLLER METHODS------>

    public void InitGrabbablePlacementController()
    {
        //Getting the num of grabbables
        num_of_grabbables.Add(GameConstants.MAX_STONES_PHASE_1);
        num_of_grabbables.Add(GameConstants.MAX_BRIDGE_SECTIONS_PHASE_2);
        num_of_grabbables.Add(CheckboxManagerLevel2.num_of_stones_phase3);

        //Debug.Log("generation start");

        grabbable_placement_generator.InitGenerator();

        //Debug.Log("generation ends");

        //Getting the grabbables from the scene
        stone_placements_phase_1 = GetAllGrabbablePlacementsFromScene(GameConstants.PHASE_1);
        tree_placements_phase_2 = GetAllGrabbablePlacementsFromScene(GameConstants.PHASE_2);
        stone_placements_phase_3 = GetAllGrabbablePlacementsFromScene(GameConstants.PHASE_3);
    }

    //<-------METHODS TO USE INIT THE GRABBABLE PLACEMENTS------>

    //Method to get all the grabbables of the scene
    private List<GrabbablePlacement> GetAllGrabbablePlacementsFromScene(int current_phase)
    {
        List<GameObject> result = new List<GameObject>();
        result.AddRange(GameObject.FindGameObjectsWithTag(grabbable_placements_tags[current_phase]));

        List<GrabbablePlacement> grabbables = new List<GrabbablePlacement>();

        for (int i = 0; i < result.Count; i++)
        {
            GrabbablePlacement grabbable = result[i].GetComponent<GrabbablePlacement>();

            if((current_phase == GameConstants.PHASE_1)&&(i >= CheckboxManagerLevel2.num_of_stones_phase1))
            {
                //We set this GrabbablePlacement as placed
                grabbable.SetAsPlaced();
            }

            grabbables.Add(grabbable);
        }

        return grabbables;
    }

    //<-------METHODS TO USE INTERNALLY------>

    //method to check if a grabbable has been placed
    private bool CheckIfGrabbablePlaced(int current_phase, int idx)
    {
        List<GrabbablePlacement> current_grabbable_placements = GetGrabbablePlacementsOfCurrentPhase(current_phase);

        return current_grabbable_placements[idx].GetIfGrabbablePlaced();
    }

    //method to check if all the grabbables have been placed
    private bool CheckIfAllGrabbablesPlaced(int current_phase)
    {
        bool condition = true;
        List<GrabbablePlacement> current_grabbable_placements = GetGrabbablePlacementsOfCurrentPhase(current_phase);

        for (int i = 0; i < num_of_grabbables[current_phase]; i++)
        {
            condition &= current_grabbable_placements[i].GetIfGrabbablePlaced();
        }

        return condition;
    }

    private List<GrabbablePlacement> GetGrabbablePlacementsOfCurrentPhase(int current_phase)
    {
        switch (current_phase)
        {
            case GameConstants.PHASE_1:
                return stone_placements_phase_1;

            case GameConstants.PHASE_2:
                return tree_placements_phase_2;

            case GameConstants.PHASE_3:
                return stone_placements_phase_3;

            default:
                return new List<GrabbablePlacement>();
        }
    }

    //<-------METHODS TO USE EXTERNALLY------>

    public bool GetIfPhaseFinished(int current_phase)
    {
        return CheckIfAllGrabbablesPlaced(current_phase);
    }

    public void FinishPhase(int current_phase)
    {
        List<GrabbablePlacement> current_grabbable_placements = GetGrabbablePlacementsOfCurrentPhase(current_phase);

        for (int i = 0; i<current_grabbable_placements.Count; i++)
        {
            current_grabbable_placements[i].SetAsPlaced();
        }
    }

    public void ChangeAllGrabbablePlacementStatus(int current_phase, bool status, bool all_objects = false)
    {
        List<GrabbablePlacement> current_grabbable_placements = GetGrabbablePlacementsOfCurrentPhase(current_phase);

        int num_of_iterations = num_of_grabbables[current_phase];

        if (all_objects) { num_of_iterations = current_grabbable_placements.Count; }

        for (int i = 0; i < num_of_iterations; i++)
        {
            if (current_grabbable_placements[i].gameObject.activeSelf != status)
            {
                current_grabbable_placements[i].gameObject.SetActive(status);
            }
            
        }
    }
}
