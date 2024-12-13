using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableController : MonoBehaviour
{
    [SerializeField]
    private GrabbableGenerator grabbable_generator;

    [SerializeField]
    private List<Transform> players;

    private List<Grabbable> stones_phase_1;

    private List<Grabbable> trees_phase_2;

    private List<Grabbable> stones_phase_3;

    private List<int> num_of_grabbables = new List<int>();
    private string[] grabbables_tags = new string[] { "StonePhase1", "TreePhase2", "StonePhase3" };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //<-------GRABBABLE CONTROLLER METHODS------>

    public void InitGrabbableController()
    {
        //Getting the num of grabbables
        num_of_grabbables.Add(CheckboxManagerLevel2.num_of_stones_phase1);
        num_of_grabbables.Add(CheckboxManagerLevel2.num_of_trees_phase2);
        num_of_grabbables.Add(CheckboxManagerLevel2.num_of_stones_phase3);

        grabbable_generator.InitGenerator();

        //Getting the grabbables from the scene
        stones_phase_1 = GetAllGrabbablesFromScene(GameConstants.PHASE_1);
        trees_phase_2 = GetAllGrabbablesFromScene(GameConstants.PHASE_2);
        stones_phase_3 = GetAllGrabbablesFromScene(GameConstants.PHASE_3);
    }

    //<-------METHODS TO USE INTERNALLY------>

    private List<Grabbable> GetGrabbablesOfCurrentPhase(int current_phase)
    {
        switch (current_phase)
        {
            case GameConstants.PHASE_1:
                return stones_phase_1;

            case GameConstants.PHASE_2:
                return trees_phase_2;

            case GameConstants.PHASE_3:
                return stones_phase_3;

            default:
                return new List<Grabbable>();
        }
    }

    //Method to get all the grabbables of the scene
    private List<Grabbable> GetAllGrabbablesFromScene(int current_phase)
    {
        List<GameObject> result = new List<GameObject>();
        result.AddRange(GameObject.FindGameObjectsWithTag(grabbables_tags[current_phase]));

        List<Grabbable> grabbables = new List<Grabbable>();

        for(int i = 0; i < result.Count; i++)
        {
            grabbables.Add(result[i].GetComponent<Grabbable>());
        }

        return grabbables;
    }

    //Method to move a grabbable
    private void MoveGrabbable(int current_phase, int idx)
    {
        List<Grabbable> current_grabbables = GetGrabbablesOfCurrentPhase(current_phase);

        current_grabbables[idx].MoveGrabbable(players);
    }

    //Method to move the grabbables of a given phase
    private void MoveGrabbables(int current_phase)
    {
        List<Grabbable> current_grabbables = GetGrabbablesOfCurrentPhase(current_phase);

        for (int i = 0; i < num_of_grabbables[current_phase]; i++)
        {
            current_grabbables[i].MoveGrabbable(players);
        }
    }

    //method to update the grabbables status
    private void UpdateGrabbablesStatus(int current_phase)
    {
        List<Grabbable> current_grabbables = GetGrabbablesOfCurrentPhase(current_phase);

        for (int i = 0; i < num_of_grabbables[current_phase]; i++)
        {
            if (current_grabbables[i].GetIfGrabbablePlaced() && current_grabbables[i].gameObject.activeSelf)
            {
                current_grabbables[i].ResetGrabbedGripsPlayers(players);
                DisableGrabbable(current_phase, i);
            }
        }
    }

    private void ChangeGrabbableStatus(int current_phase, int idx, bool status)
    {
        List<Grabbable> current_grabbables = GetGrabbablesOfCurrentPhase(current_phase);

        current_grabbables[idx].gameObject.SetActive(status);
    }

    private void EnableGrabbable(int current_phase, int idx) { ChangeGrabbableStatus(current_phase, idx, true); }
    private void DisableGrabbable(int current_phase, int idx) { ChangeGrabbableStatus(current_phase, idx, false); }

    //<-------METHODS TO USE EXTERNALLY------>

    public void RunGrabbables(int current_phase)
    {
        MoveGrabbables(current_phase);
        UpdateGrabbablesStatus(current_phase);
    }

    public void ChangeAllGrabbableStatus(int current_phase, bool status, bool all_objects = false)
    {
        List<Grabbable> current_grabbables = GetGrabbablesOfCurrentPhase(current_phase);

        int num_of_iterations = num_of_grabbables[current_phase];

        if (all_objects) { num_of_iterations = current_grabbables.Count; }

        for (int i = 0; i < num_of_iterations; i++)
        {
            if(current_grabbables[i].gameObject.activeSelf != status)
            {
                current_grabbables[i].gameObject.SetActive(status);
            }
        }
    }

    //public void EnableAllGrabbable(int current_phase) { ChangeAllGrabbableStatus(current_phase, true); }
    //public void DisableAllGrabbable(int current_phase) { ChangeAllGrabbableStatus(current_phase, false); }
}
