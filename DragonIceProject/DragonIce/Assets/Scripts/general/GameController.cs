using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController : MonoBehaviour
{
    //bool to know if the players are ingame
    protected bool ingame;

    //bool to know if the game is finished
    protected bool gameplay_finished;

    //<------------------------------------------------------------->
    //<------------------------CONTROLLERS-------------------------->
    //<------------------------------------------------------------->

    protected SequenceController sequence_controller;
    protected UnityEngine.Rendering.Volume volume;

    //<------------------------------------------------------------->
    //<--------------------------METHODS---------------------------->
    //<------------------------------------------------------------->

    protected virtual void FindControllers()
    {
        sequence_controller = GameObject.Find("SequenceController").GetComponent<SequenceController>();
        volume = GameObject.Find("PostProcessing").GetComponent<UnityEngine.Rendering.Volume>();
    }

    protected void InitGameplayControlValues(bool ingame, bool gameplay_finished) { this.ingame = ingame; this.gameplay_finished = gameplay_finished; }       //method to init the gameplay control values
    protected void ChangeBloomIntensity(float value)
    {
        UnityEngine.Rendering.HighDefinition.Bloom bloom;
        volume.profile.TryGet(out bloom);
        bloom.intensity.value = value;
    }

    protected abstract void DataExtractorCSVUpdater();
    protected abstract void RunStorySequences();
    protected abstract void RunGameplay();
    protected abstract void GetIfGameplayFinished();
}
