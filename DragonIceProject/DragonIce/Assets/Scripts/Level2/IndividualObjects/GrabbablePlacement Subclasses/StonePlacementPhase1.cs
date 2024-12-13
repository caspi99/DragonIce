using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePlacementPhase1 : RectangularGrabbablePlacement
{
    public override void InitGrabbablePlacement()
    {
        grabbable_placement_type = GameConstants.STONE_PHASE_1;
        corners_colliding = new bool[] { false, false, false };

        base.InitGrabbablePlacement();
    }
}
