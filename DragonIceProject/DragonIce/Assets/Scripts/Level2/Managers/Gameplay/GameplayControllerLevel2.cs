using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayControllerLevel2 : MonoBehaviour
{
    //<--------------------CONTROLLERS------------------------->

    [SerializeField]
    private GrabbableController grab_controller;    //grabbable controller

    [SerializeField]
    private GrabbablePlacementController grab_placement_controller;    //grabbable placement controller

    [SerializeField]
    private PlayersControllerLevel2 players_controller;    //players controller

    [SerializeField]
    private TablaGenerator tabla_generator;

    private int current_phase;

    private bool[] phase_shown = new bool[] {false, false, false};

    //<----------------------METHODS THAT USES THE GAMEPLAY CONTROLLER-------------------------->

    private void Start()
    {
        InitGameplayController();
    }

    //<----------------------PRINCIPAL GAMEPLAY METHODS-------------------------->
    public bool Gameplay(bool sequence_condition, bool skip_condition)
    {
        if (sequence_condition)
        {
            if (!phase_shown[current_phase])
            {
                ShowPhaseObjects(current_phase);

                phase_shown[current_phase] = true;
            }

            if ((current_phase == GameConstants.PHASE_2)&&!CheckboxManagerLevel2.pullTreeMechanic) { players_controller.ChangePlayersInteractionLevel(GameConstants.COLLISION); }
            else { players_controller.ChangePlayersInteractionLevel(GameConstants.TRIGGER); }

            grab_controller.RunGrabbables(current_phase);

            if (skip_condition) { grab_placement_controller.FinishPhase(current_phase); }
        }

        return CheckIfPhaseFinished();
    }

    public void NextPhase(bool sequence_condition)
    {
        HidePhaseObjects(current_phase);
        UpdatePhase();
    }

    //<----------------------METHODS THAT USES THE GAMEPLAY CONTROLLER INTERNALLY-------------------------->

    //method to init the Gameplay Controller
    private void InitGameplayController()
    {
        current_phase = GameConstants.PHASE_1;

        tabla_generator.InitGenerator();   //to create the tablas

        grab_controller.InitGrabbableController();
        grab_placement_controller.InitGrabbablePlacementController();

        HidePhaseObjects(GameConstants.PHASE_1, true);
        HidePhaseObjects(GameConstants.PHASE_2, true);
        HidePhaseObjects(GameConstants.PHASE_3, true);
    }

    private bool CheckIfPhaseFinished() { return grab_placement_controller.GetIfPhaseFinished(current_phase); }

    private void UpdatePhase()
    {
        current_phase++;
        players_controller.ResetGrabbingGripVariablesInPlayers();
    }

    private void ChangePhaseObjectsVisibility(bool status, int phase, bool all_objects = false)
    {
        if (!CheckIfGameplayFinished())
        {
            grab_controller.ChangeAllGrabbableStatus(phase, status, all_objects);
            grab_placement_controller.ChangeAllGrabbablePlacementStatus(phase, status, all_objects);
        }
    }

    private void ShowPhaseObjects(int phase, bool all_objects = false) { ChangePhaseObjectsVisibility(true, phase, all_objects); }
    private void HidePhaseObjects(int phase, bool all_objects = false) { ChangePhaseObjectsVisibility(false, phase, all_objects); }

    //<----------------------METHODS TO USE THE GAMEPLAY CONTROLLER-------------------------->

    public int GetCurrentPhase() { return current_phase; }
    public bool CheckIfGameplayFinished()
    {
        return current_phase > 2;
    }
}
