using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrabbablePlacement : InteractiveObject
{
    //<-------GRABBABLE PLACEMENT ATRIBUTES------>

    protected bool[] corners_colliding = new bool[] {};

    protected bool grabbable_placed = false;

    protected int grabbable_placement_type;

    [SerializeField]
    protected int shape;

    [SerializeField]
    protected GameObject empty_object;

    [SerializeField]
    protected GameObject filled_object;

    protected SoundController sound_controller;
    protected NewSoundController nwsc;

    //DATA EXTRACTION
    protected DataExtractorCSVLevel2 data_extractor;

    //<-------GRABBABLE METHODS------>

    //<-------START METHODS------>

    protected virtual void ChangeGrabbablePlacementName()
    {
        string grabbable_placement_name = grabbable_placement_type.ToString() + "_" + shape.ToString();

        this.gameObject.name = grabbable_placement_name;
    }

    //<-------METHODS TO USE INTERNALLY------>

    //stay trigger
    protected abstract void OnTriggerStay(Collider collider);

    //method to get if all the corners collide
    protected virtual bool GetIfAllCornersCollided()
    {
        bool condition = true;

        for (int i = 0; i < corners_colliding.Length; i++)
        {
            condition = condition && corners_colliding[i];
            
        }

        return condition;
    }

    //<-------METHODS TO USE EXTERNALLY------>

    //method to get the state of grabbable_placed
    public virtual bool GetIfGrabbablePlaced() { return grabbable_placed; }

    //method to place a grabbable placement
    public virtual void SetAsPlaced()
    {
        //We set this GrabbablePlacement as placed
        grabbable_placed = true;

        //We change the aspect of the placement
        empty_object.SetActive(false);
        filled_object.SetActive(true);
    }

    //method to init the grabbable placement
    public virtual void InitGrabbablePlacement()
    {
        ChangeGrabbablePlacementName();

        sound_controller = GameObject.Find("SoundController").GetComponent<SoundController>();    //to init the audio
        nwsc = GameObject.Find("SoundController").GetComponent<NewSoundController>();    //to init the audio

        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel2>();    //to init the data extraction

        empty_object.SetActive(true);

        if (grabbable_placement_type == GameConstants.STONE_PHASE_1)
        { empty_object.SetActive(CheckboxManagerLevel2.show_stone1_placement); }

        if(grabbable_placement_type == GameConstants.TREE_PHASE_2) { empty_object.SetActive(false); }

        filled_object.SetActive(false);
    }

    protected void DataExtractionGrabbablePlaced(Collider collider)
    {
        Grabbable curr_grabbable = collider.gameObject.GetComponent<Grabbable>();
        int grabbable_id = curr_grabbable.GetInternalId();
        int grabbable_shape = curr_grabbable.GetShape();

        data_extractor.WriteDataLinePlacingGrabbable(grabbable_id, grabbable_shape);
    }
}
