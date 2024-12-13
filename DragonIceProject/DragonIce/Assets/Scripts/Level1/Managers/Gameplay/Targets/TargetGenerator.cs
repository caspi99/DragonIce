using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerator : GeneratorLevel1
{
    public void InitTargetGenerator(List<string> players)
    {
        InitCurrentRoundsPrefabs();
        SetTargetAssignedPlayers(players);
        InitTargetsDefaultIdx();

        InitPrefabs();
    }

    //<----------------------METHODS THAT USES THE STONE GENERATOR-------------------------->

    //method to init the current rounds stones
    private void InitTargetsDefaultIdx()
    {
        for (int i = 0; i < prefabs_assigned_players.Count; i++)
        {
            string player = prefabs_assigned_players[i];
            int idx = int.Parse(player.Substring(player.Length - 1)) - 1;

            prefab_default_idx.Add(idx.ToString() + "_");
        }
    }

    //method to init the stones assigned players list
    public void SetTargetAssignedPlayers(List<string> players) { prefabs_assigned_players = players; }
}
