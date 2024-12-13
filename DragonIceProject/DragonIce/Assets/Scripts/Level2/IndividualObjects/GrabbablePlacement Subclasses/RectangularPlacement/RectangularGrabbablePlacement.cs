using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RectangularGrabbablePlacement : GrabbablePlacement
{
    [SerializeField]
    private List<SubColliderRectangularPlacement> sub_colliders = new List<SubColliderRectangularPlacement>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        GetSubCollidersInfo();
    }

    //<-------METHODS TO USE INTERNALLY------>

    //stay trigger
    protected override void OnTriggerStay(Collider collider)
    {
        bool condition_tag = GameConstants.grabbable_tags.Contains(collider.tag);
        bool condition_shape = (collider.gameObject.name == this.gameObject.name) && (!grabbable_placed);

        if (condition_tag && condition_shape && GetIfAllCornersCollided())
        {
            DataExtractionGrabbablePlaced(collider); //DATA EXTRACTION

            //We set this GrabbablePlacement as placed
            SetAsPlaced();

            //we change the name of TreeMiddle to disabled
            collider.gameObject.name = "disabled";

            //SOUND OF THE PLACEMENT
            if (Settings.binauralSound)
            {
                nwsc.DynamicPlayClip(SoundInformationLevel2.POP_SOUND, this.transform);
            }
            else
            {
                sound_controller.PlayClipOneShot(SoundInformationLevel2.POP_SOUND);
            }
        }
    }

    public override void InitGrabbablePlacement()
    {
        base.InitGrabbablePlacement();
    }

    protected virtual void GetSubCollidersInfo()
    {
        corners_colliding[GameConstants.LEFT_CORNER] = sub_colliders[GameConstants.LEFT_CORNER].GetCondition() && (!grabbable_placed);
        corners_colliding[GameConstants.RIGHT_CORNER] = sub_colliders[GameConstants.RIGHT_CORNER].GetCondition() && (!grabbable_placed);
        corners_colliding[GameConstants.MIDDLE] = sub_colliders[GameConstants.MIDDLE].GetCondition() && (!grabbable_placed);
    }
}
