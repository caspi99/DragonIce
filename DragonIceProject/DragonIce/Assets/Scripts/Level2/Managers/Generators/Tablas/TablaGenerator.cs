using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablaGenerator : GeneratorLevel2
{
    protected override Quaternion GetRotation(int grabbable_phase, int internal_idx)
    {
        return Quaternion.Euler(0, -90, 0);
    }

    protected override GameObject GenerateClone(int internal_idx, int grabbable_phase, int shape)
    {
        GameObject clone = base.GenerateClone(internal_idx, grabbable_phase, shape);
        clone.name = shape.ToString();

        return clone;
    }
}
