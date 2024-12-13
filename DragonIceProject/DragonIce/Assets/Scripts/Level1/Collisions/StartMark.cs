using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMark : MonoBehaviour
{
    private string assigned_player;     //String with the name of the assigned player to this start mark

    // Start is called before the first frame update
    void Start()
    {
        assigned_player = "";
    }

    //<----------------------METHODS THAT USES THE STARTMARKS SCRIPT-------------------------->

    //<----------------------COLLISIONS-------------------------->

    void OnTriggerStay(Collider collider)
    {
        bool condition = (collider.gameObject.tag == "Player") && (assigned_player == "");

        if (condition) { assigned_player = collider.gameObject.name; }
    }

    void OnTriggerExit(Collider collider)
    {
        bool condition = (collider.gameObject.tag == "Player") && (collider.gameObject.name == assigned_player);

        if (condition) { assigned_player = ""; }
    }

    //<----------------------METHODS TO USE THE STARTMARKS SCRIPT-------------------------->
    public string GetAssignedPlayer() { return assigned_player; }
}
