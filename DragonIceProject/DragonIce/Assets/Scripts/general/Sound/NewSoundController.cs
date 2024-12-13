using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AT_IDX_lv1
{
    HUMANS,
    KNIGHT,
    SCIENTIST,
    DRAGON
}

public enum AT_IDX_lv2
{
    VOLCANO,
    CAR,
    RIVER,
    DRAGON_BEFORE_EXP,
    DRAGON,
    HUMANS,
    SQUIRREL_IND_PH1,
    SQUIRREL_COL_PH1,
    SQUIRREL_IND_PH2,
    SQUIRREL_COL_PH2,
    STONE_IND_PH1,
    STONE_COL_PH1,
    TREE_IND_PH2,
    TREE_COL_PH2
}

public sealed class AT_Instances
{
    private List<StaticAudioTransform> audioTransforms;
    private List<DynamicAudioTransform> dynamicAudioTransforms;

    private static AT_Instances instance = null;
    private AT_Instances()
    {
        audioTransforms = new List<StaticAudioTransform>();
        dynamicAudioTransforms = new List<DynamicAudioTransform>();
    }

    public static AT_Instances Instance
    {
        get
        {
            if (instance == null)
                instance = new AT_Instances();
            return instance;
        }
    }

    public void GetSceneInstances()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            // Level 1
            Transform knightTransform = GameObject.Find("KnightAPoseFinal").GetComponent<Transform>();
            Transform scientistTransform = GameObject.Find("PepeTPose").GetComponent<Transform>();
            Transform dragonTransform = GameObject.Find("DragonBase").GetComponent<Transform>();

            audioTransforms.Clear();
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv1.HUMANS, knightTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv1.KNIGHT, knightTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv1.SCIENTIST, scientistTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv1.DRAGON, dragonTransform));
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            // Level 2
            Transform volcanoTransform = GameObject.Find("Planes/Mapas/Map Before Explosion/volcan").GetComponent<Transform>();
            Transform carTransform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Tramo1@Anim/Jeep_GEO").GetComponent<Transform>();
            Transform riverTransform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Visuals (sequences)/Bridge_empty (only visual)").GetComponent<Transform>();
            Transform dragonBeforeExpTransform = GameObject.Find("Planes/Mapas/Map Before Explosion/OBJECTS/DragonBase").GetComponent<Transform>();
            Transform dragonTransform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/DragonBase").GetComponent<Transform>();
            Transform squirrelIndPh1Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaPiedraIndividual@Anim/Ardilla_GEO").GetComponent<Transform>();
            Transform squirrelColPh1Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaPiedraColectiva@Anim/Ardilla_GEO").GetComponent<Transform>();
            Transform squirrelIndPh2Transform;
            Transform squirrelColPh2Transform;

            if (CheckboxManagerLevel2.pullTreeMechanic)
            {
                squirrelIndPh2Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaTroncoIndividual@Anim/Ardilla_GEO").GetComponent<Transform>();
                squirrelColPh2Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaTroncoColectiva@Anim/Ardilla1_GEO").GetComponent<Transform>();
            }
            else
            {
                squirrelIndPh2Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaTroncoIndividual2/Ardilla_GEO").GetComponent<Transform>();
                squirrelColPh2Transform = GameObject.Find("Planes/Mapas/Map After Explosion/OBJECTS/Ardillas/ArdillaTroncoColectiva2/Ardilla1_GEO").GetComponent<Transform>();
            }

            audioTransforms.Clear();
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.VOLCANO, volcanoTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.CAR, carTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.RIVER, riverTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.DRAGON_BEFORE_EXP, dragonBeforeExpTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.DRAGON, dragonTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.HUMANS, carTransform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.SQUIRREL_IND_PH1, squirrelIndPh1Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.SQUIRREL_COL_PH1, squirrelColPh1Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.SQUIRREL_IND_PH2, squirrelIndPh2Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.SQUIRREL_COL_PH2, squirrelColPh2Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.STONE_IND_PH1, squirrelIndPh1Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.STONE_COL_PH1, squirrelColPh1Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.TREE_IND_PH2, squirrelIndPh2Transform));
            audioTransforms.Add(new StaticAudioTransform((int)AT_IDX_lv2.TREE_COL_PH2, squirrelColPh2Transform));
        }
    }

    public StaticAudioTransform GetAT(AT_IDX_lv1 idx) { return audioTransforms[(int)idx]; }
    public StaticAudioTransform GetAT(AT_IDX_lv2 idx) { return audioTransforms[(int)idx]; }
    public void AddDynamicAudioTransform(DynamicAudioTransform dynamicAudio) { dynamicAudioTransforms.Add(dynamicAudio); }
    public void UpdateAudioTransforms()
    {
        //AudioTransforms
        for (int i = 0; i < audioTransforms.Count; i++)
        {
            audioTransforms[i].UpdatePosition();
        }

        //DynamicAudioTransforms
        for (int i = dynamicAudioTransforms.Count - 1; i >= 0; i--)
        {
            dynamicAudioTransforms[i].UpdatePosition();
            if (dynamicAudioTransforms[i].CheckIfFinished())
            {
                dynamicAudioTransforms[i].DestroyInstance();
                dynamicAudioTransforms.RemoveAt(i);
            }
        }
    }
}

public class AudioTransform
{
    public AudioSource audioSource { get; private set; }
    protected Transform parent;

    protected AudioTransform(Transform parent)
    {
        GameObject audioGameObject = new GameObject();
        audioGameObject.AddComponent<AudioSource>();
        audioSource = audioGameObject.GetComponent<AudioSource>();

        // Enable spatialization
        audioSource.spatialize = true;

        this.parent = parent;
    }

    public void UpdatePosition()
    {
        if (parent.gameObject.activeSelf)
        {
            this.audioSource.transform.position = this.parent.position;

            if (Settings.binauralSound) { BinauralSoundCalculus(); }
        }
    }

    protected void BinauralSoundCalculus() { audioSource.panStereo = Mathf.Clamp((50f - audioSource.transform.position.z) / 50f, -1f, 1f); }
}

public class StaticAudioTransform : AudioTransform
{
    public int idx { get; private set; }

    public StaticAudioTransform(int idx, Transform parent) : base(parent)
    {
        this.idx = idx;
        audioSource.gameObject.name = "AudioSource" + this.idx.ToString();
    }
}

public class DynamicAudioTransform : AudioTransform
{
    private int timer;
    private TimeManager tm;
    private float secondsDuration;

    public DynamicAudioTransform(Transform parent, float secondsDuration) : base(parent)
    {
        audioSource.gameObject.name = "DynamicAudioSource";

        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        timer = tm.CreateTimer();
        this.secondsDuration = secondsDuration;
    }
    public bool CheckIfFinished() { return tm.WaitTime(timer, secondsDuration); }
    // Method to release resources and perform cleanup
    public void DestroyInstance()
    {
        // Stop the audio source if it's playing
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Destroy the GameObject associated with the audio source
        GameObject.Destroy(audioSource.gameObject);

        // Perform any other cleanup tasks

        // Optionally, unregister from events or other actions

        // Set fields to null or default values to indicate the object is no longer in a valid state
        parent = null;
        secondsDuration = 0;

        // Ensure the timer is released
        tm.PauseTimer(timer);
        tm = null;
    }
}

public class NewSoundController : MonoBehaviour
{
    private List<ClipList> clip_lists;

    [SerializeField] private Transform audioListener;

    // Start is called before the first frame update
    void Start()
    {
        if (Settings.binauralSound) { audioListener.position = new Vector3(50f, 0f, 50f); }
        else { audioListener.position = new Vector3(50f, 125f, 50f); }

        SoundInformation sound_info = this.gameObject.GetComponent<SoundInformation>();

        clip_lists = sound_info.GetLevelClips();

        AT_Instances.Instance.GetSceneInstances();
    }

    private void Update()
    {
        AT_Instances.Instance.UpdateAudioTransforms();
    }

    //<----------------------METHODS THAT USES THE SOUND CONTROLLER-------------------------->

    //Method to change the clip
    private void ChangeClip(AudioSource audioSource, AudioClip clip) { audioSource.clip = clip; }

    //Method to reproduce the input audio source
    private void ReproduceAudio(AudioSource audioSource, bool loop)
    {
        audioSource.loop = loop;
        audioSource.Play();
    }

    //Method to change a clip and reproduce it
    private void ReproduceClip(AudioClip clip, AudioSource audioSource, bool loop, float volume)
    {
        ChangeClip(audioSource, clip);
        ChangeVolume(audioSource, volume);
        ReproduceAudio(audioSource, loop);
    }

    //Method to get a clip
    private AudioClip GetClipFromList(int clip_list, int clip_idx)
    {
        return clip_lists[clip_list].clips[clip_idx];
    }

    //<----------------------METHODS TO USE THE SOUND CONTROLLER-------------------------->

    //Method to play a full clip
    private void PlayClip(SoundInfo soundInfo, AT_IDX_lv1 atIdx, bool loop)
    {
        AudioClip clip = GetClipFromList(soundInfo.audioSourceIdx, soundInfo.clipIdx);

        StaticAudioTransform audioTransform = AT_Instances.Instance.GetAT(atIdx);

        ReproduceClip(clip, audioTransform.audioSource, loop, soundInfo.volume);
    }

    public void PlayClipOnce(SoundInfo soundInfo, AT_IDX_lv1 atIdx) { PlayClip(soundInfo, atIdx, false); } //Method to play a full clip on loop
    public void PlayClipOnLoop(SoundInfo soundInfo, AT_IDX_lv1 atIdx) { PlayClip(soundInfo, atIdx, true); } //Method to play a full clip on loop

    public void StopPlaying(AT_IDX_lv1 atIdx) { AT_Instances.Instance.GetAT(atIdx).audioSource.Stop(); }

    //Method to play a full clip level2
    private void PlayClip(SoundInfo soundInfo, AT_IDX_lv2 atIdx, bool loop)
    {
        AudioClip clip = GetClipFromList(soundInfo.audioSourceIdx, soundInfo.clipIdx);

        StaticAudioTransform audioTransform = AT_Instances.Instance.GetAT(atIdx);

        ReproduceClip(clip, audioTransform.audioSource, loop, soundInfo.volume);
    }

    public void PlayClipOnce(SoundInfo soundInfo, AT_IDX_lv2 atIdx) { PlayClip(soundInfo, atIdx, false); } //Method to play a full clip on loop
    public void PlayClipOnLoop(SoundInfo soundInfo, AT_IDX_lv2 atIdx) { PlayClip(soundInfo, atIdx, true); } //Method to play a full clip on loop

    public void StopPlaying(AT_IDX_lv2 atIdx) { AT_Instances.Instance.GetAT(atIdx).audioSource.Stop(); }

    //Method to play a full clip
    public void DynamicPlayClip(SoundInfo soundInfo, Transform parent)
    {
        AudioClip clip = GetClipFromList(soundInfo.audioSourceIdx, soundInfo.clipIdx);

        float secondsDuration = clip.length;

        DynamicAudioTransform dynamicAudio = new DynamicAudioTransform(parent, secondsDuration);

        AT_Instances.Instance.AddDynamicAudioTransform(dynamicAudio);

        ReproduceClip(clip, dynamicAudio.audioSource, false, soundInfo.volume);
    }

    //Method to alter volume
    private void ChangeVolume(AudioSource audioSource, float volume)
    {
        audioSource.volume = volume;
    }
}
