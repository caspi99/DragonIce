using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGenerator : GeneratorLevel1
{
    //<--------------------STONES COLOR------------------------->
    public List<Material> stones_material = new List<Material>();
    [SerializeField] private List<Material> particleMaterials = new List<Material>();

    public void InitStoneGenerator()
    {
        InitCurrentRoundsPrefabs();
        InitStonesDefaultIdx();
        InitStonesAssignedPlayers();

        InitPrefabs();
    }

    //<----------------------METHODS THAT USES THE STONE GENERATOR-------------------------->

    //method to init the current rounds stones
    private void InitStonesDefaultIdx()
    {
        for (int i = 0; i < 4; i++) { prefab_default_idx.Add(i.ToString()+"_"); }
    }

    //method to init the stones assigned players list
    private void InitStonesAssignedPlayers()
    {
        prefabs_assigned_players.Add("Player1");
        prefabs_assigned_players.Add("Player2");
        prefabs_assigned_players.Add("Player3");
        prefabs_assigned_players.Add("Player4");
    }

    //method to generate a clone
    protected override GameObject GenerateClone(string identifier)
    {
        GameObject clone = base.GenerateClone(identifier);

        (int stone_idx, _) = ProcessPrefabIdentifier(identifier);

        clone.GetComponent<Stone>().SetAssignedPlayer(prefabs_assigned_players[stone_idx]);   //we set the assigned player
        clone.GetComponent<Renderer>().material = stones_material[stone_idx];  //we set the corresponding material to the clone
        clone.GetComponent<Stone>().SetStoneColor(stones_material[stone_idx].color);
        clone.GetComponent<Stone>().SetParticleMaterial(particleMaterials[stone_idx]);

        return clone;
    }
}
