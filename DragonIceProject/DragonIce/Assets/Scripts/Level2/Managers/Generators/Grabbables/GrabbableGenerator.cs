using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStyleStatusList
{
    public bool[] playStyleStatus { get; private set; }
    public PlayStyleStatusList(bool[] playStyleStatus) { this.playStyleStatus = playStyleStatus; }
}

public class GrabbableGenerator : GeneratorLevel2
{
    protected List<PlayStyleStatusList> playStyleStatus_lists;
    protected override void GetVariables()
    {
        base.GetVariables();
        GrabbableGeneratorInformation info = (GrabbableGeneratorInformation)grabbable_generator_info;
        playStyleStatus_lists = info.GetPlayStyleStatusLists();
    }
    protected bool GetPlayStyleStatus(int grabbable_phase, int internal_idx)
    {
        return playStyleStatus_lists[grabbable_phase].playStyleStatus[internal_idx];
    }

    //method to generate a clone
    protected override GameObject GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        GameObject clone = base.GenerateClone(internal_idx, grabbable_phase, shape);
        clone.GetComponent<Grabbable>().InitGrabbable(internal_idx, GetPlayStyleStatus(grabbable_phase, internal_idx));    //we init the grabbable

        if((grabbable_phase == GameConstants.TREE_PHASE_2)&&CheckboxManagerLevel2.pullTreeMechanic)
        {
            clone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        return clone;
    }
}
