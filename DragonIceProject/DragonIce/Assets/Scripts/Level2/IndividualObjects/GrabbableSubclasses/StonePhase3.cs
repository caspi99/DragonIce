using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePhase3 : Grabbable
{
    public override void InitGrabbable(int id, bool status)
    {
        //We change the value of the grabbable to its corresponding type
        grabbable_type = GameConstants.STONE_PHASE_3;

        base.InitGrabbable(id, status);
    }
}