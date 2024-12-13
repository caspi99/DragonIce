using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubColliderRectangularPlacement : MonoBehaviour
{
    private bool condition = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //enter trigger
    void OnTriggerEnter(Collider collider)
    {
        bool condition_corner = (collider.gameObject.name == this.gameObject.name);

        bool condition_check_orientation = !((this.gameObject.name == "RectangleMiddle") && (collider.gameObject.name == "RectangleMiddle") && CheckboxManagerLevel2.phase_1_check_orientation);

        if (condition_check_orientation)
        {
            condition_corner = GameConstants.grabbable_placement_subcolliders_tags.Contains(collider.gameObject.name);
        }

        if (condition_corner)
        {
            bool condition_shape = (collider.transform.root.name == this.transform.root.name);
            bool condition_tag = GameConstants.grabbable_tags.Contains(collider.transform.root.tag);
            if (condition_tag && condition_shape) { condition = true; Debug.Log("Enter" + this.gameObject.name); }
        }
    }

    //exit trigger
    void OnTriggerExit(Collider collider)
    {
        bool condition_corner = (collider.gameObject.name == this.gameObject.name);

        bool condition_check_orientation = !((this.gameObject.name == "RectangleMiddle") && (collider.gameObject.name == "RectangleMiddle") && CheckboxManagerLevel2.phase_1_check_orientation);

        if (condition_check_orientation)
        {
            condition_corner = GameConstants.grabbable_placement_subcolliders_tags.Contains(collider.gameObject.name);
        }

        if (condition_corner)
        {
            bool condition_shape = (collider.transform.root.name == this.transform.root.name);
            bool condition_tag = GameConstants.grabbable_tags.Contains(collider.transform.root.tag);
            if (condition_tag && condition_shape) { condition = false; Debug.Log("Exit" + this.gameObject.name); }
        }
    }

    public bool GetCondition() { return condition; }
}
