using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePlacementPhase3 : RectangularGrabbablePlacement
{
    public override void InitGrabbablePlacement()
    {
        grabbable_placement_type = GameConstants.STONE_PHASE_3;
        base.InitGrabbablePlacement();
    }
}
