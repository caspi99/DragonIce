using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel2 : MonoBehaviour
{
    private int grabbing_grabbable_phase;
    private int grabbing_grabbable;
    private int grabbing_grip;

    private CapsuleCollider col;
    //private BoxCollider col;
    private Rigidbody rb;

    private GameObject rope;
    private GameObject branch;

    private Vector3 previous_position;  //to calculate the direction of the branch in phase 2

    // Start is called before the first frame update
    void Start()
    {
        previous_position = this.transform.position;

        ResetGrabbingGrip();

        col = this.gameObject.GetComponent<CapsuleCollider>();
        //col = this.gameObject.GetComponent<BoxCollider>();
        rb = this.gameObject.GetComponent<Rigidbody>();

        rope = this.transform.Find("Cuerda").gameObject;
        branch = this.transform.Find("RamaPivot").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //<-------METHODS TO USE EXTERNALLY------>

    //method to get if a player is grabbing a grip
    public bool GetIfGrabbingGrip(int phase_id, int grab_id, int grip_id)
    {
        bool condition = (grabbing_grabbable_phase == phase_id) && (grabbing_grabbable == grab_id) && (grabbing_grip == grip_id);
        return condition;
    }

    public void SetGrabbingGrip(int phase_id, int grab_id, int grip_id)
    { (grabbing_grabbable_phase, grabbing_grabbable, grabbing_grip) = (phase_id, grab_id, grip_id); }

    //method to reset a player's grabbing grip variables
    public void ResetGrabbingGrip() { (grabbing_grabbable_phase, grabbing_grabbable, grabbing_grip) = (-1, -1, -1); }

    //method to change players interaction to TRIGGER or COLLISION
    public void ChangeInteractionLevel(bool status)
    {
        col.isTrigger = status; rb.isKinematic = status;

        if (status)
        {
            col.radius = 1.5f; col.height = 10.0f;
        }
        else
        {
            col.radius = 3.5f; col.height = 7.0f;
        }

        /*
        Vector3 size = col.size;
        size.x = 5f;
        size.z = 5f;

        col.size = size;
        */
    }

    public void ChangeMass(float mass) { rb.mass = mass; }

    //method to change the visibility of the branch
    public void ChangeRopeVisibility(bool status) { rope.SetActive(status); }

    //method to change the visibility of the branch
    public void ChangeBranchVisibility(bool status) { branch.SetActive(status); }

    //method to calculate the direction of the branch
    public void CalculateDirectionOfBranch()
    {
        Vector3 curr_pos = this.transform.position;
        Vector3 direction = (curr_pos - previous_position);

        if (direction.magnitude > 0.05f)
        {
            Vector3 start_vector = new Vector3(-1.0f, 0.0f, 0.0f);

            float dot_vector = Vector3.Dot(start_vector, direction.normalized);
            float cos_angle = dot_vector / Vector3.Magnitude(start_vector) * Vector3.Magnitude(direction.normalized);
            float angle = Mathf.Acos(cos_angle) * Mathf.Rad2Deg;

            if (direction.z < 0.0f) { angle = -angle; }

            branch.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        previous_position = curr_pos;
    }

    //method to get the grabbed grabbable of the player
    public int GetCurrentInteractingGrabbable() { return grabbing_grabbable; }
}
