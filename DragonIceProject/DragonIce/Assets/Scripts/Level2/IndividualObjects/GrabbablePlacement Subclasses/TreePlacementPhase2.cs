using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlacementPhase2 : RectangularGrabbablePlacement
{
    private List<GameObject> tablas = new List<GameObject>();

    public void InitTreePlacementPhase2(int shape)
    {
        this.shape = shape; //we set the shape

        grabbable_placement_type = GameConstants.TREE_PHASE_2;
        corners_colliding = new bool[] { false, false, false };

        InitGrabbablePlacement();

        GetAllTablasFromScene();    //to get all the tablas
    }

    // Update is called once per frame
    protected override void Update()
    {
        GetSubCollidersInfo();

        UpdateGrabbablePlacedStatus();
    }

    //stay trigger
    protected override void OnTriggerStay(Collider collider)
    {
        bool condition_tag = (GameConstants.grabbable_tags[GameConstants.TREE_PHASE_2] == collider.tag);
        bool condition_shape = (collider.gameObject.name == this.gameObject.name) && (!grabbable_placed);

        if (condition_shape && condition_tag)
        {
            DataExtractionGrabbablePlaced(collider); //DATA EXTRACTION

            UpdateBridgeTablas(collider.gameObject.GetComponent<TreePhase2>().GetShape());

            //we change the name of TreeMiddle to disabled
            collider.gameObject.name = "disabled";

            //SOUND OF THE PLACEMENT
            if (Settings.binauralSound)
            {
                nwsc.DynamicPlayClip(SoundInformationLevel2.WOOD_PLANK_SOUND, collider.transform);
            }
            else
            {
                sound_controller.PlayClipOneShot(SoundInformationLevel2.WOOD_PLANK_SOUND);
            }
        }
    }

    //<-------METHODS TO USE INIT THE GRABBABLE PLACEMENTS------>

    //Method to get all the grabbables of the scene
    private void GetAllTablasFromScene()
    {
        List<GameObject> found_tablas = new List<GameObject>();
        found_tablas.AddRange(GameObject.FindGameObjectsWithTag("Tabla"));

        for(int i = 0; i < found_tablas.Count; i++)
        {
            bool shapeCondition = found_tablas[i].name == this.shape.ToString();

            if (shapeCondition)
            {
                found_tablas[i].transform.SetParent(this.gameObject.transform);

                (bool empty_status, bool filled_status) = (false, true);

                if (i < CheckboxManagerLevel2.num_of_trees_phase2)
                { (empty_status, filled_status) = (CheckboxManagerLevel2.show_table_placement, false); }

                (GameObject empty, GameObject filled) = GetChildren(found_tablas[i]);

                empty.SetActive(empty_status);
                filled.SetActive(filled_status);

                tablas.Add(found_tablas[i]);
            }
        }
    }

    private void UpdateGrabbablePlacedStatus()
    {
        bool condition = true;

        for(int i = 0; i < tablas.Count; i++)
        {
            condition &= GetIfObjectPlaced(tablas[i]);
        }

        //We set this GrabbablePlacement as placed
        if (condition) { grabbable_placed = true; }
    }

    private void UpdateBridgeTablas(int shape)
    {
        for(int i = 0; i < tablas.Count; i++)
        {
            if (!GetIfObjectPlaced(tablas[i]))
            {
                (GameObject empty, GameObject filled) = GetChildren(tablas[i]);

                //to show the object
                empty.SetActive(false);
                filled.SetActive(true);

                i = tablas.Count;
            }
        }
    }

    private bool GetIfObjectPlaced(GameObject tabla)
    {
        (_, GameObject filled) = GetChildren(tabla);

        return filled.activeSelf;
    }

    private (GameObject empty, GameObject filled) GetChildren(GameObject tabla)
    {
        int numChildren = tabla.transform.childCount;

        List<GameObject> children = new List<GameObject>();
        int[] indexes = new int[] { 0, 1 };

        for (int i = 0; i < numChildren; i++)
        {
            GameObject childObject = tabla.transform.GetChild(i).gameObject;

            if (childObject.name == "Empty") { indexes[0] = i; }
            if (childObject.name == "Filled") { indexes[1] = i; }

            children.Add(childObject);
        }

        return (children[indexes[0]], children[indexes[1]]);
    }
}
