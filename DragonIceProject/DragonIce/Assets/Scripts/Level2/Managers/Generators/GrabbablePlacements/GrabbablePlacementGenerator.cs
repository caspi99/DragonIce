using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbablePlacementGenerator : GeneratorLevel2
{
    protected override Quaternion GetRotation(int grabbable_phase, int internal_idx)
    {
        if (grabbable_phase == GameConstants.PHASE_1) { return Quaternion.Euler(0.0f, 180.0f, 0.0f); }
        else { return Quaternion.identity; }
    }

    protected override GameObject GetPrefab(int grabbable_phase, int shape)
    {
        if(grabbable_phase == GameConstants.PHASE_2) { shape = 0; }
        return base.GetPrefab(grabbable_phase, shape);
    }

    //method to generate a clone
    protected override GameObject GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        GameObject clone = base.GenerateClone(internal_idx, grabbable_phase, shape);

        //we init the grabbable placement
        if (grabbable_phase == GameConstants.PHASE_2) { clone.GetComponent<TreePlacementPhase2>().InitTreePlacementPhase2(shape); }
        else { clone.GetComponent<GrabbablePlacement>().InitGrabbablePlacement(); }

        return clone;
    }
}
