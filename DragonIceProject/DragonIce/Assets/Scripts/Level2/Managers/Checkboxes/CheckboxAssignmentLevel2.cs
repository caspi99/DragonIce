using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckboxAssignmentLevel2 : MonoBehaviour
{
    //<------------------------CHECKBOXES--------------------------->
    //This script puts all the values to the CheckboxManager

    [Header("Narrative")]
    [Space(10)]

    [SerializeField] private bool skip_story_sequences = false;
    [SerializeField] private bool shorterStorySequences = false;

    //[Header("Sequences")]
    //[Space(10)]

    [Header("Phase Personalization")]
    [Space(10)]

    [SerializeField]
    private int num_of_stones_phase1 = 24;                       //Inputbox to change the number of stones in phase 1

    [SerializeField]
    private int num_of_trees_phase2 = 24;                        //Inputbox to change the number of trees in phase 2

    [SerializeField]
    private int num_of_stones_phase3 = 24;                       //Inputbox to change the number of stones in phase 3

    [Header("Interaction")]
    [Space(10)]

    [SerializeField]
    private float grabbable_move_speed_solo = 10.0f;             //Inputbox to change the move speed solo of a grabbable

    [SerializeField]
    private float player_mass = 10.0f;                           //Inputbox to change the mass of a player

    [SerializeField]
    private bool use_rigidbody_translation = true;               //Checkbox to decide if you want to use physics moving the grabbables

    [SerializeField]
    private bool inclinate_grabbable = true;                     //Checkbox to decide if you want to inclinate a grabbable when picking from one side

    [SerializeField]
    private bool show_grips = true;

    [Header("Phase 1")]
    [Space(10)]

    [SerializeField]
    private bool phase_1_check_orientation = true;

    [SerializeField]
    private bool show_stone1_placement = false;                  //Checkbox to decide if you want to see the table placement

    [SerializeField] private bool stayToPickStone = true;       //Checkbox to decide if you need to be staying a time to get a stone
    [SerializeField] private float stayToPickStoneTime = 0.5f;  //Inputbox to decide the time staying to get a stone
    [SerializeField] private bool allGripsUntachCollaborative = true;       //Checkbox to decide if all the grips are disabled when one ungrabs
    [SerializeField] private bool preferencePlayerStone = false;       //Checkbox to decide if a grip has preference over the rest of the stone
    [SerializeField] private bool biggerGripsWhenGrabbed = true;       //Checkbox to decide if a grip is bigger when grabbed

    [Header("Phase 2")]
    [Space(10)]

    [SerializeField]
    private bool show_table_placement = false;                   //Checkbox to decide if you want to see the table placement

    [SerializeField]
    private float tree_mass_touching = 20f;                   //Inputbox to decide the mass of the tree while moving it

    [SerializeField]
    private float tree_mass_default = 7500f;                  //Inputbox to decide the mass of the tree while not moving it
    
    [SerializeField] private bool pullTreeMechanic = true;   //Inputbox to decide to use the Pull Tree Mechanic

    // Faster than start
    void Awake()
    {
        AssignCheckboxes();
    }

    private void AssignCheckboxes()
    {
        //Checkboxes related to Narrative
        CheckboxManagerLevel2.skip_story_sequences = skip_story_sequences;
        CheckboxManagerLevel2.shorterStorySequences = shorterStorySequences;

        //Checkboxes related to phase personalization
        CheckboxManagerLevel2.num_of_stones_phase1 = Mathf.Clamp(num_of_stones_phase1, 0, GameConstants.MAX_STONES_PHASE_1);
        CheckboxManagerLevel2.num_of_trees_phase2 = Mathf.Clamp(num_of_trees_phase2, 0, GameConstants.MAX_TABLAS_PHASE_2);
        CheckboxManagerLevel2.num_of_stones_phase3 = Mathf.Clamp(num_of_stones_phase3, 0, GameConstants.MAX_STONES_PHASE_3);

        //Checkboxes related to interaction
        CheckboxManagerLevel2.grabbable_move_speed_solo = grabbable_move_speed_solo;
        CheckboxManagerLevel2.player_mass = player_mass;

        CheckboxManagerLevel2.use_rigidbody_translation = use_rigidbody_translation;
        CheckboxManagerLevel2.inclinate_grabbable = inclinate_grabbable;

        CheckboxManagerLevel2.show_grips = show_grips;

        //Checkboxes related to Phase 1
        CheckboxManagerLevel2.phase_1_check_orientation = phase_1_check_orientation;
        CheckboxManagerLevel2.show_stone1_placement = show_stone1_placement;
        CheckboxManagerLevel2.stayToPickStone = stayToPickStone;
        CheckboxManagerLevel2.stayToPickStoneTime = stayToPickStoneTime;
        CheckboxManagerLevel2.allGripsUntachCollaborative = allGripsUntachCollaborative;
        CheckboxManagerLevel2.preferencePlayerStone = preferencePlayerStone;
        CheckboxManagerLevel2.biggerGripsWhenGrabbed = biggerGripsWhenGrabbed;

        //Checkboxes related to Phase 2
        CheckboxManagerLevel2.show_table_placement = show_table_placement;
        CheckboxManagerLevel2.tree_mass_touching = tree_mass_touching;
        CheckboxManagerLevel2.tree_mass_default = tree_mass_default;
        CheckboxManagerLevel2.pullTreeMechanic = pullTreeMechanic;
    }
}
