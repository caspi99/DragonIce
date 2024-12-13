using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMarksController : MonoBehaviour
{
    //<--------------------CONTROL INDEXES------------------------->
    const bool ACTIVE = true;
    const bool DISABLED = false;

    //<----------------START POSITIONS INDEXES------------------->
    const int START_POS_1 = 0;
    const int START_POS_2 = 1;
    const int START_POS_3 = 2;
    const int START_POS_4 = 3;

    //<--------------------START POSITIONS----------------------->
    public List<StartMark> start_marks;
    public List<GameObject> start_marks_colors;

    //<-----------ASSIGNED PLAYERS TO EACH POSITION-------------->
    private List<string> start_marks_assigned_players = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        InitStartMarksAssignedPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        GetStartMarksAssignedPlayers();
    }

    //<----------------------METHODS THAT USES THE STARTMARKS CONTROLLER-------------------------->
    //method to init the start marks assigned players list
    private void InitStartMarksAssignedPlayers()
    {
        for(int i = 0; i<start_marks.Count; i++) { start_marks_assigned_players.Add(""); }
    }

    //method to update the start marks assigned players list
    private void GetStartMarksAssignedPlayers()
    {
        if (CheckStartMarksStatus())
        {
            for(int i = 0; i < start_marks_assigned_players.Count; i++) { start_marks_assigned_players[i] = start_marks[i].GetAssignedPlayer(); }
        }
    }

    //method to check if all the start marks have been assigned
    private bool CheckStartMarksAssignedPlayers()
    {
        for (int i = 0; i < start_marks_assigned_players.Count; i++)
        {
            if(start_marks_assigned_players[i] == "") { return false; }     //not ready to start
        }

        return true;                                                        //ready to start
    }

    //method to check if the first start mark is active. If one is active, the rest are active
    private bool CheckStartMarksStatus() { return start_marks[0].gameObject.activeSelf; }


    //<----------------------METHODS TO USE THE STARTMARKS CONTROLLER-------------------------->

    //method to change the start marks to active or disabled
    private void ChangeStartMarksStatus(bool status)
    {
        for (int i = 0; i < start_marks.Count; i++)
        {
            start_marks[i].gameObject.SetActive(status);
        }
    }

    public void ActivateStartMarks() { ChangeStartMarksStatus(ACTIVE); }                //activate the start marks
    public void DeactivateStartMarks() { ChangeStartMarksStatus(DISABLED); }            //deactivate the start marks

    public bool ReadyToStart() { return CheckStartMarksAssignedPlayers(); }                    //method to check if the game can start

    public List<string> GetPlayersStartPos()    //method to get the players assigned to each start mark
    {
        if (Settings.tracking_player_reorder)
        {
            start_marks_assigned_players[0] = "Player1";
            start_marks_assigned_players[1] = "Player2";
            start_marks_assigned_players[2] = "Player3";
            start_marks_assigned_players[3] = "Player4";
        }

        return start_marks_assigned_players;
    }

    public List<int> GetPlayersStartOrder()
    {
        List<int> order = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            order.Add(int.Parse(start_marks_assigned_players[i][start_marks_assigned_players[i].Length - 1].ToString()) - 1);
        }

        return order;
    }

    //method to show the startmarks colors
    public void ShowStartMarksColors(bool condition)
    {
        for (int i = 0; i < start_marks_colors.Count; i++)
        {
            start_marks_colors[i].SetActive(Settings.light_docks && condition);
        }
    }
}
