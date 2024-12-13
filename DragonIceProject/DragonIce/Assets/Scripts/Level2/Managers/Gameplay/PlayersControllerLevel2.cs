using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersControllerLevel2 : PlayersController
{
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!CheckboxManagerLevel2.pullTreeMechanic) { ChangeMassOfPlayers(); }
    }

    private void ChangeMassOfPlayers()
    {
        //GETTING THE PUSHED GRABBABLES
        int[] players_pushing_idx = new int[4] {0,0,0,0};

        for (int i = 0; i < Players.Count; i++)
        {
            players_pushing_idx[i] = Players[i].GetComponent<PlayerLevel2>().GetCurrentInteractingGrabbable();
        }

        //MASS CHANGING
        for (int i = 0; i < players_pushing_idx.Length; i++)
        {
            //We count the num of players pushing one grabbable
            int count_of_grabbable_i = 0;

            for (int j = 0; j < players_pushing_idx.Length; j++)
            {
                if (players_pushing_idx[i] == players_pushing_idx[j])
                {
                    count_of_grabbable_i++;
                }
            }

            //Then, we obtain the final mass to assign to player i
            float final_mass = CheckboxManagerLevel2.player_mass;

            if ((count_of_grabbable_i>0) && (players_pushing_idx[i] > -1))
            {
                final_mass = final_mass / count_of_grabbable_i;
            }

            Players[i].GetComponent<PlayerLevel2>().ChangeMass(final_mass);
        }
    }

    //<----------------------EXTERNAL METHODS OF THE PLAYERS CONTROLLER-------------------------->

    //method to fix bugs with players not being able to pick grabbables when changing to the next phase
    public void ResetGrabbingGripVariablesInPlayers()
    {
        for(int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<PlayerLevel2>().ResetGrabbingGrip();
        }
    }

    public void ChangePlayersInteractionLevel(bool status)
    {
        for(int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<PlayerLevel2>().ChangeInteractionLevel(status);
        }
    }

    public void ChangePlayerRopeVisibility(bool status)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerLevel2 curr_player = Players[i].GetComponent<PlayerLevel2>();

            bool condition = status && curr_player.GetIfGrabbingGrip(-1, -1, -1);

            curr_player.ChangeRopeVisibility(condition);
        }
    }

    public void ChangePlayerBranchVisibility(bool status)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<PlayerLevel2>().ChangeBranchVisibility(status);
        }
    }

    public void CalculateDirectionBranchOfPlayer()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].GetComponent<PlayerLevel2>().CalculateDirectionOfBranch();
        }
    }

    public int GetNumberOfPlayersInteractingGrabbable(int grabbable_internal_id)
    {
        //GETTING THE PUSHED GRABBABLES
        int[] players_grabbable_idx = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < Players.Count; i++)
        {
            players_grabbable_idx[i] = Players[i].GetComponent<PlayerLevel2>().GetCurrentInteractingGrabbable();
        }

        int result = 0;

        //COUNT OF THE SAME GRABBABLE
        for (int i = 0; i < players_grabbable_idx.Length; i++)
        {
            if (players_grabbable_idx[i] == grabbable_internal_id) { result++; }
        }

        return result;
    }

    public List<int> GetListOfPlayersInteractingGrabbable(int grabbable_internal_id)
    {
        //GETTING THE PUSHED GRABBABLES
        int[] players_grabbable_idx = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < Players.Count; i++)
        {
            players_grabbable_idx[i] = Players[i].GetComponent<PlayerLevel2>().GetCurrentInteractingGrabbable();
        }

        List<int> result = new List<int>();

        //COUNT OF THE SAME GRABBABLE
        for (int i = 0; i < players_grabbable_idx.Length; i++)
        {
            if (players_grabbable_idx[i] == grabbable_internal_id) { result.Add(i); }
        }

        return result;
    }
}
