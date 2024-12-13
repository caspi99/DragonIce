using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmokeGenerator : GeneratorLevel2
{
    //method to generate a clone
    protected override GameObject GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        GameObject clone = base.GenerateClone(internal_idx, 0, 0);
        clone.tag = "SmokeInstance";    //we init the grabbable

        return clone;
    }

    protected override Quaternion GetRotation(int grabbable_phase, int internal_idx)
    {
        return Quaternion.identity;
    }

    public List<Vector3> GetInitPositions() { return position_lists[0].positions.ToList(); }
}
