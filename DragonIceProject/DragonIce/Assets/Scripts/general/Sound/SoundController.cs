using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundInfo
{
    public int clipIdx { get; private set; }
    public float volume { get; private set; }
    public int audioSourceIdx { get; private set; }
    public SoundInfo(int clipIdx, float volume, int audioSourceIdx)
    {
        this.clipIdx = clipIdx;
        this.volume = volume;
        this.audioSourceIdx = audioSourceIdx;
    }
}

public class ClipList
{
    public List<AudioClip> clips;
    public List<bool> control_clips;

    public ClipList(List<AudioClip> clips)
    {
        this.clips = clips;

        control_clips = new List<bool>();

        for (int i = 0; i < this.clips.Count; i++)
        {
            control_clips.Add(GameConstants.ACTIVE);
        }
    }
}

public class SoundController : MonoBehaviour
{
    private List<ClipList> clip_lists;

    [SerializeField]
    private List<AudioSource> audio_sources;

    /*
    [SerializeField] private List<Transform> soundPositions;
    private List<int> soundPositionsIdxLevel1 = new List<int>() { SoundInformationLevel1.DRAGON};
    private List<int> soundPositionsIdxLevel2 = new List<int>() { SoundInformationLevel2.DRAGON};
    */

    // Start is called before the first frame update
    void Start()
    {
        SoundInformation sound_info = this.gameObject.GetComponent<SoundInformation>();

        clip_lists = sound_info.GetLevelClips();
    }

    /*
    private void Update()
    {
        if((SceneManager.GetActiveScene().name == "Level1")&&CheckboxManager.binauralSound)
        {
            BinauralSoundCalculus(0);
        }
    }
    */

    //<----------------------BINAURAL SOUND-------------------------->>
    /*
    private void BinauralSoundCalculus(int levelIdx)
    {
        List<int> levelList = soundPositionsIdxLevel1;
        if(levelIdx == 1) { levelList = soundPositionsIdxLevel2; }

        for (int i = 0; i < levelList.Count; i++)
        {
            int idx = levelList[i];
            audio_sources[idx].panStereo = Mathf.Clamp((soundPositions[i].position.z - 50f) / 50f, -1f, 1f);
        }
    }
    */

    //<----------------------METHODS TO INITIALIZE THE SOUND CONTROLLER-------------------------->

    //Method to get the size of a clip list
    private int GetClipListSize(int idx)
    {
        return clip_lists[idx].clips.Count;
    }

    //<----------------------METHODS THAT USES THE SOUND CONTROLLER-------------------------->

    //Method to change the clip from the input audio source
    private void ChangeClip(AudioClip clip, int audio_source)
    {
        audio_sources[audio_source].Stop();
        audio_sources[audio_source].clip = clip;
    }

    //Method to reproduce the input audio source
    private void ReproduceAudioSource(int audio_source, bool loop)
    {
        audio_sources[audio_source].loop = loop;
        audio_sources[audio_source].Play();
    }

    //Method to reproduce a clip in the audio source with PlayOneShot
    private void ReproduceClipOneShot(AudioClip clip, int audio_source, float volume)
    {
        audio_sources[audio_source].loop = false;
        audio_sources[audio_source].PlayOneShot(clip, volume);
    }

    //Method to change a clip and reproduce it
    private void ReproduceClip(AudioClip clip, int audio_source, bool loop, float volume)
    {
        ChangeVolume(audio_source, volume);
        ChangeClip(clip, audio_source);
        ReproduceAudioSource(audio_source, loop);
    }

    //Method to get a clip
    private AudioClip GetClipFromList(int clip_list, int clip_idx)
    {
        return clip_lists[clip_list].clips[clip_idx];
    }

    //Method to get a control bool
    private bool GetControlBoolOfClip(int clip_list, int clip_idx)
    {
        return clip_lists[clip_list].control_clips[clip_idx];
    }

    //Method to set a control bool
    private void SetControlBoolOfClip(int clip_list, int clip_idx, bool status = GameConstants.FINISHED)
    {
        clip_lists[clip_list].control_clips[clip_idx] = status;
    }

    //<----------------------METHODS TO USE THE SOUND CONTROLLER-------------------------->

    //Method to play a full clip
    private void PlayClip(SoundInfo soundInfo, bool loop)
    {
        AudioClip clip = GetClipFromList(soundInfo.audioSourceIdx, soundInfo.clipIdx);

        ReproduceClip(clip, soundInfo.audioSourceIdx, loop, soundInfo.volume);
    }

    public void PlayClipOnce(SoundInfo soundInfo) { PlayClip(soundInfo, false); } //Method to play a full clip once
    public void PlayClipOnLoop(SoundInfo soundInfo) { PlayClip(soundInfo, true); } //Method to play a full clip on loop

    public void PlayClipOneShot(SoundInfo soundInfo, float volume = 0.5f)
    {

        AudioClip clip = GetClipFromList(soundInfo.audioSourceIdx, soundInfo.clipIdx);

        ReproduceClipOneShot(clip, soundInfo.audioSourceIdx, volume);
    }

    //Method to play a full clip just once in the game
    private void PlayControlledClip(SoundInfo soundInfo, bool loop)
    {
        if (GetControlBoolOfClip(soundInfo.audioSourceIdx, soundInfo.clipIdx)) { SetControlBoolOfClip(soundInfo.audioSourceIdx, soundInfo.clipIdx); PlayClip(soundInfo, loop); }
    }

    public void PlayControlledClipOnce(SoundInfo soundInfo) { PlayControlledClip(soundInfo, false); } //Method to play a full clip once just once in the game
    public void PlayControlledClipOnLoop(SoundInfo soundInfo) { PlayControlledClip(soundInfo, true); } //Method to play a full clip on loop just once in the game

    //Method to play a full clip once just once in the game
    public void PlayControlledClipOneShot(SoundInfo soundInfo, float volume = 0.5f)
    {
        if (GetControlBoolOfClip(soundInfo.audioSourceIdx, soundInfo.clipIdx)) { SetControlBoolOfClip(soundInfo.audioSourceIdx, soundInfo.clipIdx); PlayClipOneShot(soundInfo); }
    }

    //Method to know if any audiosource is playing
    public bool IsPlaying(int audio_source)
    {
        return audio_sources[audio_source].isPlaying;
    }

    //Method to stop playing the sound of the input audio source
    public void StopPlaying(int audio_source)
    {
        audio_sources[audio_source].Stop();
    }

    //Method to stop playing the sound of the input audio source
    public void StopPlayingAll()
    {
        for (int i = 0; i < audio_sources.Count; i++)
        {
            audio_sources[i].Stop();
        }
    }

    //Method to reactive a clip
    public void ReactiveClip(SoundInfo soundInfo) { SetControlBoolOfClip(soundInfo.audioSourceIdx, soundInfo.clipIdx, GameConstants.ACTIVE); }

    //Method to alter volume
    public void ChangeVolume(int audio_source, float volume)
    {
        audio_sources[audio_source].volume = volume;
    }

    public void IncreaseVolume(int audio_source, float quantity) { ChangeVolume(audio_source, audio_sources[audio_source].volume + quantity); } //Method to increase volume
    public void DecreaseVolume(int audio_source, float quantity) { ChangeVolume(audio_source, audio_sources[audio_source].volume - quantity); } //Method to decrease volume

    //Method to alter pitch
    public void ChangePitch(int audio_source, float pitch)
    {
        audio_sources[audio_source].pitch = pitch;
    }

    public void IncreasePitch(int audio_source, float quantity) { ChangePitch(audio_source, audio_sources[audio_source].pitch + quantity); } //Method to increase pitch
    public void DecreasePitch(int audio_source, float quantity) { ChangePitch(audio_source, audio_sources[audio_source].pitch - quantity); } //Method to decrease pitch

    //Method to get pitch
    public float GetPitch(int audio_source)
    {
        return audio_sources[audio_source].pitch;
    }

    //Method to get if a clip is ready to be reproduced
    public bool IsClipReady(int clip_list, int clip_idx) { return GetControlBoolOfClip(clip_list, clip_idx); }

    //Method to get the length of a clip
    public float ClipLength(int clip_list, int clip_idx) { return GetClipFromList(clip_list, clip_idx).length; }
}
