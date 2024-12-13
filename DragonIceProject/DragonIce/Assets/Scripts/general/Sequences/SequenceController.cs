using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Demo Key Structure
public class DemoKey
{
    public KeyCode key { get; private set; }
    public int count;
    public float seconds;
    public bool countingTime;

    public DemoKey(KeyCode key)
    {
        this.key = key;
        count = 0;
        seconds = 0;
        countingTime = false;
    }
}

public class Sequence
{
    protected List<Action<object[]>> sequence_lines;
    protected List<bool> sequence_line_control;

    protected int timer;
    protected int current_line; //This controls the index of the current line of the sequence. For each sequence, we increment this number

    protected SoundController sc;
    protected NewSoundController nwsc;
    protected AnimationController ac;
    protected TimeManager tm;
    protected ScreenFadeController sfc;

    public Sequence()
    {
        sc = GameObject.Find("SoundController").GetComponent<SoundController>();
        nwsc = GameObject.Find("SoundController").GetComponent<NewSoundController>();
        ac = GameObject.Find("AnimationController").GetComponent<AnimationController>();
        tm = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        sfc = GameObject.Find("ScreenFadeController").GetComponent<ScreenFadeController>();

        //variables initialization

        sequence_lines = new List<Action<object[]>>();
        sequence_line_control = new List<bool>();

        timer = tm.CreateTimer(); tm.PauseTimer(timer);
        current_line = 0;
    }

    //method to add a line to the sequence
    public void AddSequenceLine(Action<object[]> sequence_line)
    {
        sequence_lines.Add(sequence_line);
        sequence_line_control.Add(true);
    }

    //method to add a line to the sequence without using control
    public void AddSequenceLineWithoutControl(Action<object[]> sequence_line)
    {
        sequence_lines.Add(sequence_line);
    }

    //method to execute the whole sequence
    public virtual void ExecuteSequence(params object[] args)
    {
        RestartLineIndexControl(); StartTimer(); //in every sequence, we have to Restart the Line Index and Start the timer

        for (int i = 0; i<sequence_lines.Count; i++){ sequence_lines[i](args); }
    }

    //<----------METHODS TO CONTROL THE LINE CONTROL---------->

    protected bool GetLineControl() { return sequence_line_control[current_line];}
    protected void SetLineControl(bool status) { sequence_line_control[current_line] = status; }
    protected void SetToAllLineControl(bool status)
    {
        for (int i = 0; i < sequence_line_control.Count; i++) { sequence_line_control[i] = status; }
    }
    //method to full restart a sequence
    protected void ResetLineControlSequence() { SetToAllLineControl(true); }

    //method to set a sequence as finished
    public void FinishLineControlSequence() { SetToAllLineControl(false); }

    //method to check if a sequence has ended
    public bool CheckEndSequence()
    {
        for (int i = 0; i < sequence_line_control.Count; i++)
        {
            if (sequence_line_control[i]) { return false; }
        }

        return true;
    }

    //<---------METHODS TO MANAGE THE TIMER---------->
    //method to start a timer
    protected void StartTimer() { tm.ResumeTimer(timer); }
    //method to restart a timer
    protected void RestartTimer() { tm.RestartTimer(timer); }

    //<---------METHODS TO MANAGE THE LINE INDEZ
    protected int GetLineIndex() { return current_line; }
    //method to update the line control
    protected void UpdateLineIndexControl() { current_line++; }
    //method to restart the line control
    protected void RestartLineIndexControl() { current_line = 0; }

    //<-------------------------METHODS TO WAIT--------------------------->

    //method to wait some time
    public void JustWait(float seconds)
    {
        if (GetLineControl() && tm.IsTime(timer, seconds)) { SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //<-------------------------METHODS TO DO ANIMATIONs TRANSITIONS--------------------------->

    //method to wait some time and set a trigger of the input animator
    public void SetTrigger(int animator_idx, string trigger)
    {
        if (GetLineControl()) { ac.SetTrigger(animator_idx, trigger); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to wait some time and set to true a bool of the input animator
    public void SetBoolTrue(int animator_idx, string bool_name)
    {
        if (GetLineControl()) { ac.SetBoolTrue(animator_idx, bool_name); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to wait some time and set to false a bool of the input animator
    public void SetBoolFalse(int animator_idx, string bool_name)
    {
        if (GetLineControl()) { ac.SetBoolFalse(animator_idx, bool_name); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void SetCrossFade(int animator_idx, AnimationInfo animInfo, float normalizedTransitionDuration, int layer = -1)
    {
        if (GetLineControl()) { ac.CrossFade(animator_idx, animInfo, normalizedTransitionDuration, layer); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to wait some time and set a trigger of the input animator
    public void WaitAndSetTrigger(int animator_idx, string trigger, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { SetTrigger(animator_idx, trigger); } else { UpdateLineIndexControl(); }
    }

    //method to wait some time and set to true a bool of the input animator
    public void WaitAndSetBoolTrue(int animator_idx, string bool_name, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { SetBoolTrue(animator_idx, bool_name); } else { UpdateLineIndexControl(); }
    }

    //method to wait some time and set to false a bool of the input animator
    public void WaitAndSetBoolFalse(int animator_idx, string bool_name, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { SetBoolFalse(animator_idx, bool_name); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndSetCrossFade(int animator_idx, AnimationInfo animInfo, float normalizedTransitionDuration, float seconds, int layer = -1)
    {
        if (tm.IsTime(timer, seconds)) { SetCrossFade(animator_idx, animInfo, normalizedTransitionDuration, layer); } else { UpdateLineIndexControl(); }
    }

    public void ChangeSpeed(int animator_idx, float percentage) { ac.ChangeSpeed(animator_idx, percentage); }

    public bool CheckIfCurrentState(int animator_idx, string anim_name) { return ac.CheckIfCurrentState(animator_idx, anim_name); }

    //method to recalculate time
    public float RecalculateTime(float time, float new_max_time, float previous_max_type)
    {
        return time * new_max_time / previous_max_type;
    }

    //<-------------------------------METHODS TO PLAY SOUNDS--------------------------------->

    //method to play a sound
    public void PlaySound(SoundInfo soundInfo)
    {
        if (GetLineControl()) { sc.ReactiveClip(soundInfo); sc.PlayControlledClipOnce(soundInfo); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to play a sound on loop
    public void PlaySoundOnLoop(SoundInfo soundInfo)
    {
        if (GetLineControl()) { sc.ReactiveClip(soundInfo); sc.PlayControlledClipOnLoop(soundInfo); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to stop playing a sound
    public void StopPlayingSound(int clip_list)
    {
        if (GetLineControl()) { sc.StopPlaying(clip_list); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralPlaySound(SoundInfo soundInfo, AT_IDX_lv1 atIdx)
    {
        if (GetLineControl()) { nwsc.PlayClipOnce(soundInfo, atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralPlaySound(SoundInfo soundInfo, AT_IDX_lv2 atIdx)
    {
        if (GetLineControl()) { nwsc.PlayClipOnce(soundInfo, atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralPlaySoundOnLoop(SoundInfo soundInfo, AT_IDX_lv1 atIdx)
    {
        if (GetLineControl()) { nwsc.PlayClipOnLoop(soundInfo, atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralPlaySoundOnLoop(SoundInfo soundInfo, AT_IDX_lv2 atIdx)
    {
        if (GetLineControl()) { nwsc.PlayClipOnLoop(soundInfo, atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralStopPlayingSound(AT_IDX_lv1 atIdx)
    {
        if (GetLineControl()) { nwsc.StopPlaying(atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void BinauralStopPlayingSound(AT_IDX_lv2 atIdx)
    {
        if (GetLineControl()) { nwsc.StopPlaying(atIdx); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to wait some time and play a sound
    public void WaitAndPlaySound(SoundInfo soundInfo, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { PlaySound(soundInfo); } else { UpdateLineIndexControl(); }
    }

    //method to wait some time and play a sound on loop
    public void WaitAndPlaySoundOnLoop(SoundInfo soundInfo, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { PlaySoundOnLoop(soundInfo); } else { UpdateLineIndexControl(); }
    }

    //method to wait some time and stop playing a sound
    public void WaitAndStopPlayingSound(int clip_list, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { StopPlayingSound(clip_list); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralPlaySound(SoundInfo soundInfo, AT_IDX_lv1 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralPlaySound(soundInfo, atIdx); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralPlaySound(SoundInfo soundInfo, AT_IDX_lv2 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralPlaySound(soundInfo, atIdx); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralPlaySoundOnLoop(SoundInfo soundInfo, AT_IDX_lv1 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralPlaySoundOnLoop(soundInfo, atIdx); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralPlaySoundOnLoop(SoundInfo soundInfo, AT_IDX_lv2 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralPlaySoundOnLoop(soundInfo, atIdx); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralStopPlayingSound(AT_IDX_lv1 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralStopPlayingSound(atIdx); } else { UpdateLineIndexControl(); }
    }

    public void WaitAndBinauralStopPlayingSound(AT_IDX_lv2 atIdx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { BinauralStopPlayingSound(atIdx); } else { UpdateLineIndexControl(); }
    }

    //<-------------------------------METHODS TO CHANGE VOLUME AND PITCH OF SOUNDS--------------------------------->

    //Method to alter volume
    protected void ChangeVolume(int audio_source, float volume) { sc.ChangeVolume(audio_source, volume); }

    protected void IncreaseVolume(int audio_source, float quantity) { sc.IncreaseVolume(audio_source, quantity); } //Method to increase volume
    protected void DecreaseVolume(int audio_source, float quantity) { sc.DecreaseVolume(audio_source, quantity); } //Method to decrease volume

    //Method to alter pitch
    protected void ChangePitch(int audio_source, float pitch) { sc.ChangePitch(audio_source, pitch); }

    protected void IncreasePitch(int audio_source, float quantity) { sc.IncreasePitch(audio_source, quantity); } //Method to increase pitch
    protected void DecreasePitch(int audio_source, float quantity) { sc.DecreasePitch(audio_source, quantity); } //Method to decrease pitch

    //Methods to fade increase volume from seconds to seconds
    protected void FadeIncreaseVolumeFromTo(int audio_source, float quantity, float from, float to)
    {
        if (tm.IsTime(timer, from) && !tm.IsTime(timer, to))
        {
            float delta = quantity * Time.deltaTime / (to - from);

            IncreaseVolume(audio_source, delta);
        }
    }

    //Methods to fade decrease volume from seconds to seconds
    public void FadeDecreaseVolumeFromTo(int audio_source, float quantity, float from, float to)
    {
        if (tm.IsTime(timer, from) && !tm.IsTime(timer, to))
        {
            float delta = quantity*Time.deltaTime/(to - from);

            DecreaseVolume(audio_source, delta);
        }
    }

    //Methods to fade increase volume from seconds to seconds
    protected void FadeIncreasePitchFromTo(int audio_source, float quantity, float from, float to)
    {
        if (tm.IsTime(timer, from) && !tm.IsTime(timer, to))
        {
            float delta = quantity * Time.deltaTime / (to - from);

            IncreasePitch(audio_source, delta);
        }
    }

    //Methods to fade decrease volume from seconds to seconds
    protected void FadeDecreasePitchFromTo(int audio_source, float quantity, float from, float to)
    {
        if (tm.IsTime(timer, from) && !tm.IsTime(timer, to))
        {
            float delta = quantity * Time.deltaTime / (to - from);

            DecreasePitch(audio_source, delta);
        }
    }

    //<-------------------------------METHODS TO SHOW/HIDE GAMEOBJECTS--------------------------------->

    //method to change a GameObject visibility
    public void ChangeObjectVisibility(GameObject obj, bool status)
    {
        obj.SetActive(status);
    }

    //method to show an object
    public void ShowObject(GameObject obj)
    {
        if (GetLineControl()) { ChangeObjectVisibility(obj, true); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to hide an object
    public void HideObject(GameObject obj)
    {
        if (GetLineControl()) { ChangeObjectVisibility(obj, false); SetLineControl(false); }

        UpdateLineIndexControl();
    }

    //method to wait some time and show an object
    public void WaitAndShowObject(GameObject obj, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ShowObject(obj); } else { UpdateLineIndexControl(); }
    }

    //method to wait some time and hide an object
    public void WaitAndHideObject(GameObject obj, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { HideObject(obj); } else { UpdateLineIndexControl(); }
    }

    public void SetPositionObject(GameObject obj, Vector3 position)
    {
        if (GetLineControl()) { obj.transform.position = position; SetLineControl(false); }

        UpdateLineIndexControl();
    }

    public void WaitAndSetPositionObject(GameObject obj, Vector3 position, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { SetPositionObject(obj, position); } else { UpdateLineIndexControl(); }
    }

    //method to move an object
    public void MoveObject(GameObject obj, Vector3 translation)
    {
        obj.transform.position += translation;
    }

    //method to move an object in a certain time to a certain position
    /*
    public void MoveObjectInTime(GameObject obj, float seconds, float seconds_duration, Vector3 initPos, Vector3 lastPos)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                float rate = Time.deltaTime / seconds_duration;

                //POSITION CHANGE
                Vector3 pos_diff = lastPos - initPos;

                Vector3 pos_increment = pos_diff * rate;

                MoveObject(obj, pos_increment);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }
    */

    public void MoveStoneInTime(GameObject obj, DemoKey dK, int countValue, float seconds_duration, Vector3 initPos, Vector3 lastPos, Color color)
    {
        if (GetLineControl())
        {
            bool countValueCondition = dK.count == countValue;

            //If the key is pressed
            if (countValueCondition && Input.GetKeyDown(dK.key) && !dK.countingTime)
            {
                dK.seconds = tm.GetTime(timer);
                dK.countingTime = true;

                obj.transform.position = initPos;
                ChangeObjectVisibility(obj, true);
            }

            if (countValueCondition && dK.countingTime)
            {
                float rate = Time.deltaTime / seconds_duration;

                //POSITION CHANGE
                Vector3 pos_diff = lastPos - initPos;

                Vector3 pos_increment = pos_diff * rate;

                MoveObject(obj, pos_increment);

                if (tm.IsTime(timer, dK.seconds + seconds_duration))
                {
                    SetLineControl(false);

                    sc.PlayClipOnce(SoundInformationLevel1.STONE_SHINE);
                    if (CheckboxManager.light_stone_when_ready_to_kick) { ChangeEmissiveColor(obj, color, CheckboxManager.stone_light_intensity); }

                    dK.countingTime = false;
                    dK.count++;
                }
            }
        }

        UpdateLineIndexControl();
    }

    private void ChangeEmissiveColor(GameObject obj, Color color, float intensity)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        renderer.material.SetColor("_EmissiveColor", color * intensity);
    }

    private float CalculateBlink(float coeff, float seconds_duration)
    {
        if (coeff > seconds_duration / 2)
        {
            return ((seconds_duration - coeff) / seconds_duration) * 2.0f;
        }
        else
        {
            return (coeff / seconds_duration) * 2.0f;
        }
    }

    /*
    public void ChangeEmissiveColorOverTime(GameObject obj, float seconds, float seconds_duration, Color color, float intensity)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                float coeff = tm.GetTime(timer) - seconds;
                float finalIntensity = CalculateBlink(coeff, seconds_duration) * intensity;

                ChangeEmissiveColor(obj, color, finalIntensity);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }
    */

    public void ChangeEmissiveColorOverTime(GameObject obj, DemoKey dK, int countValue, float seconds_duration, Color color, float intensity, bool addParticles = false)
    {
        if (GetLineControl())
        {
            bool countValueCondition = dK.count == countValue;

            //If the key is pressed
            if (countValueCondition && Input.GetKeyDown(dK.key) && !dK.countingTime)
            {
                dK.seconds = tm.GetTime(timer);
                dK.countingTime = true;

                //Sound
                if (addParticles || CheckboxManager.onlyKickPerfectSound) { sc.PlayClipOnce(SoundInformationLevel1.KICK_STONE_PERFECT); }
                else if (!CheckboxManager.muteGoodSound) { sc.PlayClipOnce(SoundInformationLevel1.KICK_STONE_GOOD); }
                sc.PlayClipOnce(SoundInformationLevel1.CRACK);

                //Instantiated ice crack
                GameObject iceCrackPrefab = (GameObject)Resources.Load("Prefabs/Level1/Hits/IceCrack", typeof(GameObject));
                GameObject iceCrack = GameObject.Instantiate(iceCrackPrefab, obj.transform.position + new Vector3(0f,2f,0f), Quaternion.Euler(90f,0f,0f));
                iceCrack.tag = "IceCrackToDestroy";
                iceCrack.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                //In case we add particles
                if (addParticles)
                {
                    Vector3 direction = obj.transform.position - new Vector3(50f, 0f, 50f);
                    direction.y = 0f;

                    GameObject particlesPrefab = (GameObject)Resources.Load("Prefabs/Level1/Particle System", typeof(GameObject));

                    GameObject particles1 = GameObject.Instantiate(particlesPrefab, obj.transform.position, Quaternion.identity);
                    GameObject particles2 = GameObject.Instantiate(particlesPrefab, obj.transform.position, Quaternion.identity);

                    particles1.GetComponent<Particle>().direction = direction.normalized;
                    particles2.GetComponent<Particle>().direction = direction.normalized;
                    particles2.GetComponent<Particle>().sign = -1;
                }
            }

            if (countValueCondition && dK.countingTime)
            {
                float coeff = tm.GetTime(timer) - dK.seconds;
                float finalIntensity = CalculateBlink(coeff, seconds_duration) * intensity;

                ChangeEmissiveColor(obj, color, finalIntensity);

                if (tm.IsTime(timer, dK.seconds + seconds_duration))
                {
                    SetLineControl(false);

                    ChangeEmissiveColor(obj, Color.black, 0f);
                    ChangeObjectVisibility(obj, false);

                    dK.countingTime = false;
                    dK.count++;
                }
            }            
        }

        UpdateLineIndexControl();
    }

    public void DeleteAllIceCracks(bool condition)
    {
        if (condition)
        {
            GameObject[] iceCracksToDestroy = GameObject.FindGameObjectsWithTag("IceCrackToDestroy");

            foreach (GameObject iceCrack in iceCracksToDestroy)
            {
                GameObject.Destroy(iceCrack);
            }
        }
    }

    /*
    public void StopBrightColor(GameObject obj, float seconds)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                ChangeEmissiveColor(obj, Color.black, 0f);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }
    */

    /*
    public void StartParticlesMovement(float seconds, Vector3 initPos)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                

                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }
    */

    public void ConditionalPlaySound(bool condition, SoundInfo soundInfo)
    {
        if (condition) { PlaySound(soundInfo); }
    }

    //<-------------------------------METHODS TO DO FADE SCREEN--------------------------------->

    //method to do screen fade during some time
    public void BlackScreenFade(float seconds, float seconds_duration, bool alpha_descending = false)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                sfc.ChangeScreenTransparencyProgressive(seconds_duration, alpha_descending);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void ScreenshotCamera(float seconds, int idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                sfc.ScreenshotCamera(idx);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    //method to do screen fade during some time
    public void CameraCrossFade(float seconds, float seconds_duration, int idx, bool alpha_descending = false)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                sfc.CrossFadeProgressive(seconds_duration, idx, alpha_descending);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    //<----RESET sound and animations---->

    public void ReturnToIdle()
    {
        sc.StopPlayingAll();
        ac.SetAllToIdle();
    }
}

public class SequenceLevel1 : Sequence
{
    private IceController ice_c;

    public SequenceLevel1() : base()
    {
        ice_c = GameObject.Find("IceController").GetComponent<IceController>();
    }

    //due to its caracteristics, we don't count SPECIAL methods as lines of a sequence
    //<-------------------------------SPECIAL METHODS TO CONTROL THE ICE--------------------------------->

    //SPECIAL method created to freeze the lake, first part, where the humans are in the animator state "KnightWires"
    public void FreezeWater1(float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ice_c.FreezeWater1(); }
    }

    //SPECIAL method created to freeze the lake, second part, where the humans are in the animator state "KnightHole"
    public void FreezeWater2(float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ice_c.FreezeWater2(); }
    }

    //SPECIAL method to show a GameObject from the IceController
    public void WaitAndShowGameObjectIceController(int object_idx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ice_c.ShowObject(object_idx); }
    }

    //SPECIAL method to hide a GameObject from the IceController
    public void WaitAndHideGameObjectIceController(int object_idx, float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ice_c.HideObject(object_idx); }
    }

    //SPECIAL method to disable all ice cracks
    public void WaitAndDisableAllIceCracks(float seconds)
    {
        if (tm.IsTime(timer, seconds)) { ice_c.DisableAllIceCracks(); }
    }
}

public class SequenceLevel1Ingame : Sequence
{
    public SequenceLevel1Ingame() : base() {}

    //method to execute the whole sequence
    public override void ExecuteSequence(params object[] args)
    {
        //this sequence has the following args:
        //(args[0]: float time_between_rounds, args[1]: bool all_players_assigned, args[2]: bool gameplay_finished, args[3]: bool swirl)

        bool gameplay_finished = (bool)args[2];
        bool swirl = (bool)args[3];

        if (!gameplay_finished) //in case it is ready to be played the sequence
        {
            if (swirl)
            {
                ac.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIRL);   //swirl trigger
            }
            else
            {
                RestartLineIndexControl(); StartTimer(); //in every sequence, we have to Restart the Line Index and Start the timer

                for (int i = 0; i < sequence_lines.Count; i++) { sequence_lines[i](args); }

                if (CheckEndSequence()) { ResetLineControlSequence(); RestartTimer(); }
            }
        }
    }
}

public class SequenceLevel2 : Sequence
{
    private MapController mc;
    private SmokeController smoke_contr;
    private DataExtractorCSVLevel2 data_extractor;

    public SequenceLevel2() : base()
    {
        mc = GameObject.Find("MapController").GetComponent<MapController>();
        smoke_contr = GameObject.Find("SmokeController").GetComponent<SmokeController>();
        data_extractor = GameObject.Find("DataExtractor").GetComponent<DataExtractorCSVLevel2>();
    }

    //<-------------------------------METHODS TO CONTROL THE MAP--------------------------------->

    public void ShowMap(float seconds, int map_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                mc.ShowMap(map_idx);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void HideMap(float seconds, int map_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                mc.HideMap(map_idx);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void ShakeMap(float seconds, float seconds_duration, float intensity, int map_idx, int transform_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                float random_offset_x = UnityEngine.Random.Range(-1f, 1f) * intensity * mc.GetCurrentScale(map_idx);
                float random_offset_z = UnityEngine.Random.Range(-1f, 1f) * intensity * mc.GetCurrentScale(map_idx);

                //POSITION CHANGE;
                Vector3 pos_increment = new Vector3(random_offset_x, 0f, random_offset_z);

                mc.Translation(map_idx, pos_increment);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                mc.ChangeMapPosition(map_idx, GameConstants.map_positions[transform_idx]);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void ZoomMap(float seconds, float seconds_duration, int map_idx,
        int init_transform_idx, int final_transform_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                float rate = 1;

                if (seconds_duration > 0)
                {
                    rate = Time.deltaTime / seconds_duration;
                }

                //ZOOM
                float scale_diff = GameConstants.map_scales[final_transform_idx] - GameConstants.map_scales[init_transform_idx];

                float zoom_increment = scale_diff * rate;

                mc.AddZoom(map_idx, zoom_increment);

                //POSITION CHANGE
                Vector3 pos_diff = GameConstants.map_positions[final_transform_idx] - GameConstants.map_positions[init_transform_idx];

                Vector3 pos_increment = pos_diff * rate;

                mc.Translation(map_idx, pos_increment);
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void ShowVisual(float seconds, int visual_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                mc.ShowVisual(visual_idx);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void HideVisual(float seconds, int visual_idx)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                mc.HideVisual(visual_idx);
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    //<-------------------------------METHODS TO CONTROL THE SMOKE--------------------------------->

    public void ShowSmoke(float seconds)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                for (int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
                {
                    smoke_contr.ShowSmoke(i);
                }
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void HideSmoke(float seconds)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                for (int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
                {
                    smoke_contr.HideSmoke(i);
                }
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    //method to do show/hide smoke in a progressive way
    public void ChangeSmokeTransparencyProgressive(float seconds, float seconds_duration, bool alpha_descending = false)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                for(int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
                {
                    smoke_contr.ChangeSmokeTransparencyProgressive(seconds_duration, i, alpha_descending);
                }
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    public void TranslateSmoke(float seconds, float seconds_duration)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                float rate = Time.deltaTime / seconds_duration;

                for(int i = 0; i < GameConstants.SMOKE_INSTANCES; i++)
                {
                    //POSITION CHANGE
                    Vector3 pos_diff = smoke_contr.GetFinalPosition(i) - smoke_contr.GetInitPosition(i);

                    Vector3 pos_increment = pos_diff * rate;

                    smoke_contr.Translation(i, pos_increment);
                }
            }

            if (tm.IsTime(timer, seconds + seconds_duration))
            {
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }

    //<-------------------------------METHODS TO WRITE DATA--------------------------------->

    public void WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
    {
        if (GetLineControl())
        {
            if (tm.IsTime(timer, seconds))
            {
                //code
                if(event_ == DataExtractorCSVLevel2.START)
                {
                    data_extractor.WriteDataLinePhaseStart(phase);
                    data_extractor.SetPhase(phase);
                }
                else { data_extractor.WriteDataLinePhaseEnd(phase); }
                
                SetLineControl(false);
            }
        }

        UpdateLineIndexControl();
    }
}

public class SequenceInformationLevel1: SequenceInformation
{
    //<--------------------SEQUENCE INDEXES------------------------>
    public const int SEQUENCE_1 = 0;
    public const int SEQUENCE_2 = 1;
    public const int SEQUENCE_INGAME = 2;
    public const int SEQUENCE_3 = 3;
    public const int SEQUENCE_4 = 4;
    public const int SEQUENCE_5 = 5;

    //<-----------------------SEQUENCES---------------------------->

    private Sequence Sequence1()
    {
        SequenceLevel1 sequence = new SequenceLevel1();

        GameObject meat = GameObject.Find("Meat"); //Meat GameObject
        meat.SetActive(false);

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndShowObject(meat, 18.866f)); //line 1 of paralel sequence 1: SHOW MEAT
        sequence.AddSequenceLine(args => sequence.WaitAndHideObject(meat, 25.6f)); //line 2 of paralel sequence 1: HIDE MEAT

        //<KNIGHT>
        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.START));                      //line 3 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.SetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK));                      //line 4 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 17.4f));       //line 5 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.THROW, 17.866f));      //line 6 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 22.15f));       //line 7 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 26.983f));     //line 8 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 29.033f));      //line 9 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 29.866f));     //line 10 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.BUTTON, 29.866f));     //line 11 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.ON_KNEES, 29.866f));   //line 12 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 31.85f));       //line 13 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 33.733f));     //line 14 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 34.533f));      //line 15 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 35.808f));     //line 16 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.BUTTON, 35.808f));     //line 17 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.ON_KNEES, 35.808f));   //line 18 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 37.199f));      //line 19 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.KNIGHT, AnimationInformationLevel1.WALK, 43.649f));     //line 20 of paralel sequence 1

        //<SCIENTIST>
        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.START));                   //line 21 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.SetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK));                   //line 22 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 17.4f));    //line 23 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 22.15f));    //line 24 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 27.583f));  //line 25 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 27.6f));     //line 26 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 28.23f));   //line 27 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 30.583f));   //line 28 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 31.4f));    //line 29 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 33.35f));    //line 30 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 34.63f));   //line 31 of paralel sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 35.6f));     //line 32 of paralel sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.SCIENTIST, AnimationInformationLevel1.WALK, 42.55f));   //line 33 of paralel sequence 1

        //<DRAGON>>
        /*
        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FIRST_SEQUENCE));                 //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 0.65f));             //line 2 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 12.316f));          //line 8 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 20.016f));           //line 9 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 23.183f));          //line 10 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.ENTER_WATER, 23.183f));       //line 12 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SECOND_SEQUENCE, 24.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FORWARD_TO_DOWN, 24.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_DOWN, 28.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_DOWN, 35.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.DOWN_TO_FORWARD, 35.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FORWARD_TO_UP, 37.1f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 38.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 43.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.UP_TO_FORWARD, 43.6f));
        */


        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FIRST_SEQUENCE));                 //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 0.65f));             //line 2 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 12.316f));          //line 8 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 20.016f));           //line 9 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 23.183f));          //line 10 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.ENTER_WATER, 23.183f));       //line 12 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SECOND_SEQUENCE, 24.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FORWARD_TO_DOWN, 24.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM_DOWN, 0.1f, 28.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_DOWN, 28.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_DOWN, 35.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.DOWN_TO_FORWARD, 0.1f, 35.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FORWARD_TO_UP, 0.1f, 36.1f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM_UP, 0.1f, 37.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 37.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 42.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.UP_TO_FORWARD, 0.1f, 42.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.UP_TO_FORWARD, 42.6f));

        //<<SOUND>>

        //<NARRATIVE>
        if(CheckboxManager.play_dragon_Introduction_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_1_INTRO, 1.15f));          //line 34 of paralel sequence 1

        if(CheckboxManager.play_dragon_Fly_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_2_FLY, 4.15f));            //line 35 of paralel sequence 1
        
        if(CheckboxManager.play_character_introduction_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_3_CHARACTERS, 10.9f));     //line 36 of paralel sequence 1
        
        if(CheckboxManager.play_freeze_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_4_FREEZE, 28.0f));         //line 37 of paralel sequence 1

        //<AMBIENT>
        sequence.AddSequenceLine(args => sequence.PlaySound(SoundInformationLevel1.AMBIENT_START));                      //line 55 of paralel sequence 1

        //<ICE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WATER_FREEZING, 31.183f));         //line 56 of paralel sequence 1

        //<WATER>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WATER_DROP, 19.716f));               //line 18 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.INTO_WATER, 24.566f));               //line 19 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.WATER, 25.683f));                                    //line 20 of sequence 1
        
        if (Settings.binauralSound)
        {
            //<HUMANS>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.SNOW_WALK, AT_IDX_lv1.HUMANS, 6.016f));            //line 38 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.WATER_WALK, AT_IDX_lv1.HUMANS, 13.878f));          //line 39 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.SNOW_WALK, AT_IDX_lv1.HUMANS, 17.309f));           //line 40 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.HUMANS, 18.0f));                                 //line 41 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.THROW_MOVEMENT, AT_IDX_lv1.HUMANS, 18.1f));        //line 42 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.SNOW_WALK, AT_IDX_lv1.HUMANS, 20.362f));     //line 43 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.BUTTON_CLICK, AT_IDX_lv1.HUMANS, 31.05f));         //line 44 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.SNOW_WALK, AT_IDX_lv1.HUMANS, 31.866f));           //line 45 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.HUMANS, 33.716f));                               //line 46 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.WALK, AT_IDX_lv1.HUMANS, 33.97f));                 //line 47 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.WALK, AT_IDX_lv1.HUMANS, 34.55f));                 //line 48 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.HUMANS, 35.791f));                               //line 49 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.BUTTON_CLICK, AT_IDX_lv1.HUMANS, 36.408f));        //line 50 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.WALK, AT_IDX_lv1.HUMANS, 37.216f));                //line 51 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.HUMANS, 43.632f));                               //line 52 of paralel sequence 1

            //<KNIGHT/SCIENTIST>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.KNIGHT_LAUGHING, AT_IDX_lv1.KNIGHT, 36.866f));     //line 53 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.MAN_LAUGHING, AT_IDX_lv1.SCIENTIST, 39.85f));      //line 54 of paralel sequence 1

            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 1.25f));                 //line 14 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.DRAGON, 11.133f));                                   //line 15 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 20.016f));               //line 16 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.DRAGON, 24.166f));                                   //line 17 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.SWIM, AT_IDX_lv1.DRAGON, 24.6f));
        }
        else
        {
            //<HUMANS>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.SNOW_WALK, 6.016f));            //line 38 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WATER_WALK, 13.878f));          //line 39 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.SNOW_WALK, 17.309f));           //line 40 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.HUMANS, 18.0f));                                 //line 41 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.THROW_MOVEMENT, 18.1f));        //line 42 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.SNOW_WALK, 20.362f));     //line 43 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.BUTTON_CLICK, 31.05f));         //line 44 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.SNOW_WALK, 31.866f));           //line 45 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.HUMANS, 33.716f));                               //line 46 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WALK, 33.97f));                 //line 47 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WALK, 34.55f));                 //line 48 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.HUMANS, 35.791f));                               //line 49 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.BUTTON_CLICK, 36.408f));        //line 50 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.WALK, 37.216f));                //line 51 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.HUMANS, 43.632f));                               //line 52 of paralel sequence 1

            //<KNIGHT/SCIENTIST>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.KNIGHT_LAUGHING, 36.866f));     //line 53 of paralel sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.MAN_LAUGHING, 39.85f));      //line 54 of paralel sequence 1

            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.FLY, 1.25f));                 //line 14 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 11.133f));                                   //line 15 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.FLY, 20.016f));               //line 16 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 24.166f));                                   //line 17 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.SWIM, 24.6f));
        }

        //<<SPECIAL METHODS>>

        //<ICE>
        sequence.AddSequenceLineWithoutControl(args => sequence.FreezeWater1(31.183f));
        sequence.AddSequenceLineWithoutControl(args => sequence.FreezeWater2(36.683f));

        return sequence;
    }

    private Sequence Sequence2()
    {
        SequenceLevel1 sequence = new SequenceLevel1();

        //<<ANIMATIONS>>

        //<DRAGON>>
        //sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.THIRD_SEQUENCE));                     //line 1 of sequence 3
        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.THIRD_SEQUENCE, 0.05f));                     //line 1 of sequence 3
        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM, 0.1f));
        sequence.AddSequenceLine(args => sequence.SetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM));     //line 2 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM, 9.533f));     //line 11 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.HIT, 0.2f, 9.333f));

        if(CheckboxManager.ingame_stay_dragon)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.STAY_IN_THE_MIDDLE, 0.1f, 10.766f));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY_UP, 0.1f, 10.766f));
            sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_UP, 10.766f));            //line 13 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_UP, 12.216f));           //line 14 of sequence 3
        }

        /*
        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.THIRD_SEQUENCE));                     //line 1 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM, 0.1f, 0f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM, 0f));     //line 2 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM_LEFT, 0.1f, 1f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM, 0.1f, 2.183f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM_LEFT, 0.1f, 3.75f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM, 0.1f, 4.85f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM_LEFT, 0.0f, 6.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SWIM, 0.1f, 7.7f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM, 9.533f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.HIT, 0.1f, 9.533f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY_UP, 0.1f, 10.766f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_UP, 10.766f));            //line 13 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_UP, 12.216f));           //line 14 of sequence 3
        */

        //<<SOUND>>

        //<NARRATIVE>
        if(CheckboxManager.play_help_dragon_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_5_HELP_DRAGON, 1.15f));            //line 15 of sequence 3
        
        if (!CheckboxManager.demoTutorialSequence && CheckboxManager.play_docks_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_6_DOCKS, 14.0f));             //line 16 of sequence 3

        if (Settings.binauralSound)
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.HITTING_ICE, AT_IDX_lv1.DRAGON, 10.533f));                     //line 17 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel1.ANGRY, AT_IDX_lv1.DRAGON, 11.933f));                       //line 18 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.SWIM, AT_IDX_lv1.DRAGON, 14.0f));                  //line 19 of sequence 3
        }
        else
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.HITTING_ICE, 10.533f));                     //line 17 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.ANGRY, 11.933f));                       //line 18 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.SWIM, 14.0f));                  //line 19 of sequence 3
        }

        //<AMBIENT>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.AMBIENT_MUSIC, 14.0f));        //line 20 of sequence 3

        //<<DEMO>>
        GameObject demoStone = GameObject.Find("DemoStone"); //DemoStone GameObject
        demoStone.SetActive(false);

        DemoKey moveKey = new DemoKey(KeyCode.Alpha9);
        DemoKey shineKey = new DemoKey(KeyCode.Alpha0);

        if (CheckboxManager.demoTutorialSequence)
        {
            //<<DRAGON LOOP ANIMATION>>
            sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.INGAME_STAY));

            //<<STONES ANIMATIONS>>
            /*
            sequence.AddSequenceLine(args => sequence.WaitAndSetPositionObject(demoStone, new Vector3(50f, -1f, 50f), 14f));
            sequence.AddSequenceLine(args => sequence.WaitAndShowObject(demoStone, 14f));
            sequence.AddSequenceLine(args => sequence.MoveObjectInTime(demoStone, 14f, 5f, new Vector3(50f, -1f, 50f), new Vector3(38.8f, -1f, 22f)));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.INTERACTION, SoundInformationLevel1.KICK_STONE_PERFECT, 21f));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, 21f, 1f, Color.white, CheckboxManager.stone_light_intensity));
            sequence.AddSequenceLine(args => sequence.StopBrightColor(demoStone, 22f));
            sequence.AddSequenceLine(args => sequence.WaitAndHideObject(demoStone, 22f));

            sequence.AddSequenceLine(args => sequence.WaitAndSetPositionObject(demoStone, new Vector3(50f, -1f, 50f), 24f));
            sequence.AddSequenceLine(args => sequence.WaitAndShowObject(demoStone, 24f));
            sequence.AddSequenceLine(args => sequence.MoveObjectInTime(demoStone, 24f, 5f, new Vector3(50f, -1f, 50f), new Vector3(50f, -1f, 20f)));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.INTERACTION, SoundInformationLevel1.KICK_STONE_PERFECT, 29.5f));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, 29.5f, 1f, Color.white, CheckboxManager.stone_light_intensity));
            sequence.AddSequenceLine(args => sequence.StartParticlesMovement(29.5f, demoStone.transform.position));
            sequence.AddSequenceLine(args => sequence.StopBrightColor(demoStone, 30.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndHideObject(demoStone, 30.5f));

            sequence.AddSequenceLine(args => sequence.WaitAndSetPositionObject(demoStone, new Vector3(50f, -1f, 50f), 32f));
            sequence.AddSequenceLine(args => sequence.WaitAndShowObject(demoStone, 32f));
            sequence.AddSequenceLine(args => sequence.MoveObjectInTime(demoStone, 32f, 5f, new Vector3(50f, -1f, 50f), new Vector3(62.2f, -1f, 22f)));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.INTERACTION, SoundInformationLevel1.KICK_STONE_PERFECT, 37f));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, 37f, 1f, Color.white, CheckboxManager.stone_light_intensity));
            sequence.AddSequenceLine(args => sequence.StopBrightColor(demoStone, 38f));
            sequence.AddSequenceLine(args => sequence.WaitAndHideObject(demoStone, 38f));
            */

            sequence.AddSequenceLine(args => sequence.MoveStoneInTime(demoStone, moveKey, 0, 5f, new Vector3(50f, -1f, 50f), new Vector3(22f, -1f, 62.2f), Color.grey));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, shineKey, 0, 1f, Color.grey, CheckboxManager.stone_explosion_intensity));

            sequence.AddSequenceLine(args => sequence.MoveStoneInTime(demoStone, moveKey, 1, 5f, new Vector3(50f, -1f, 50f), new Vector3(22f, -1f, 50f), Color.grey));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, shineKey, 1, 1f, Color.grey, CheckboxManager.stone_explosion_intensity, true));

            sequence.AddSequenceLine(args => sequence.MoveStoneInTime(demoStone, moveKey, 2, 5f, new Vector3(50f, -1f, 50f), new Vector3(22f, -1f, 38.8f), Color.grey));
            sequence.AddSequenceLine(args => sequence.ChangeEmissiveColorOverTime(demoStone, shineKey, 2, 1f, Color.grey, CheckboxManager.stone_explosion_intensity));

            sequence.AddSequenceLineWithoutControl(args => sequence.DeleteAllIceCracks(shineKey.count>2));

            if(CheckboxManager.play_docks_Narrative)
                sequence.AddSequenceLine(args => sequence.ConditionalPlaySound(shineKey.count>2, SoundInformationLevel1.N_6_DOCKS));
        }

        return sequence;
    }

    private Sequence SequenceIngame()
    {
        SequenceLevel1Ingame sequence = new SequenceLevel1Ingame();

        //this sequence has the following args:
        //(args[0]: float time_between_rounds, args[1]: bool all_players_assigned, args[2]: bool gameplay_finished, args[3]: bool swirl)

        //float time_between_rounds = (float)args[0];
        //bool all_players_assigned = (bool)args[1];

        //<<ANIMATIONS>>

        //CHANGE ANIMATION SPEED

        sequence.AddSequenceLineWithoutControl(args =>
        {
            if (sequence.CheckIfCurrentState(AnimationInformationLevel1.DRAGON, "Swim") && (bool)args[1])
            {
                float speed = 7.50f / (float)args[0];
                sequence.ChangeSpeed(AnimationInformationLevel1.DRAGON, speed);
            }
        }
        );

        //CHANGE BETWEEN ANIMATIONS
        if (CheckboxManager.ingame_stay_dragon)
        {
            sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.INGAME_STAY));   //line 1 of sequence ingame_stay
        }
        else
        {
            //<DRAGON>>
            sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.INGAME));   //line 1 of sequence ingame

            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, sequence.RecalculateTime(1.016f, (float)args[0], 7.50f)));     //line 2 of sequence ingame
            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, sequence.RecalculateTime(3.4f, (float)args[0], 7.50f)));    //line 3 of sequence ingame

            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, sequence.RecalculateTime(3.4f, (float)args[0], 7.50f)));      //line 4 of sequence ingame
            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, sequence.RecalculateTime(4.3f, (float)args[0], 7.50f)));     //line 5 of sequence ingame

            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, sequence.RecalculateTime(4.3f, (float)args[0], 7.50f)));       //line 6 of sequence ingame
            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, sequence.RecalculateTime(6.183f, (float)args[0], 7.50f)));      //line 7 of sequence ingame

            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, sequence.RecalculateTime(6.183f, (float)args[0], 7.50f)));    //line 8 of sequence ingame
            //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, sequence.RecalculateTime(7.033f, (float)args[0], 7.50f)));   //line 9 of sequence ingame

            sequence.AddSequenceLine(args => sequence.JustWait(sequence.RecalculateTime(7.5f, (float)args[0], 7.50f)));         //line 10 of sequence ingame
        }

        return sequence;
    }

    private Sequence Sequence3()
    {
        SequenceLevel1 sequence = new SequenceLevel1();

        //<<ANIMATIONS>>

        //CHANGE ANIMATION SPEED
        sequence.AddSequenceLineWithoutControl(args => sequence.ChangeSpeed(AnimationInformationLevel1.DRAGON, 1.0f));

        //<DRAGON>>
        sequence.AddSequenceLine(args => sequence.SetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.OUTGAME));                              //line 1 of sequence 4
        sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FOURTH_SEQUENCE));                       //line 2 of sequence 4

        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.UP_TO_FORWARD, 0.0f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.HIT, 0.0f, 1.166f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FINAL_HIT_TRIGGER, 1.166f));      //line 3 of sequence 4

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM, 2.5f));               //line 4 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 6.266f));            //line 5 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 6.883f));            //line 6 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 7.783f));           //line 7 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 7.783f));             //line 8 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 9.283f));            //line 9 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 9.666f));            //line 10 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 10.516f));          //line 11 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 10.516f));            //line 12 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 11.833f));           //line 13 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 12.033f));           //line 14 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_RIGHT, 13.25f));           //line 15 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 13.25f));             //line 16 of sequence 4
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 15.05f));            //line 17 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 17.033f));            //line 18 of sequence 4
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM, 18.433f));           //line 19 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.BREAKICE, 18.5f));             //line 20 of sequence 4
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.BREAK_ICE_PATH_IMPROVED, 0.0f, 18.3f));             //line 20 of sequence 4

        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FORWARD_TO_DOWN, 18.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FORWARD_TO_DOWN, 0.1f, 18.5f));

        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.DOWN_TO_FORWARD, 19.5f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.DOWN_TO_FORWARD, 20.5f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FORWARD_TO_UP, 19.5f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FORWARD_TO_UP, 0.1f, 19.5f));

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 21.0f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_UP, 23.0f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SHELL_POSITION, 21.0f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SHELL_POSITION, 0f, 21f));

        //<<SOUND>>

        //<NARRATIVE>
        if(CheckboxManager.play_Final_Narrative)
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.N_9_FINAL, 4f));                    //line 21 of sequence 4

        if (Settings.binauralSound)
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.SWIM, AT_IDX_lv1.DRAGON, 18.50f));                      //line 22 of sequence 4
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.DRAGON, 22.333f));                                                 //line 23 of sequence 4                  //line 26 of sequence 4
        }
        else
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.SWIM, 18.50f));                      //line 22 of sequence 4
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 22.333f));                                          //line 23 of sequence 4
        }

        //<ICE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.HITTING_ICE_FINAL, 1.566f));                  //line 24 of sequence 4
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.ICE_CRACKING_2, 3.333f));                     //line 25 of sequence 4
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel1.FINAL_CRACK, 22.433f));                       //line 26 of sequence 4

        //<<WAITS>> //in order to define the seconds of the sequence
        sequence.AddSequenceLine(args => sequence.JustWait(24.3f));                                                                                   //line 28 of sequence 4

        //<<SPECIAL METHODS>>

        //<ICE>
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndShowGameObjectIceController(IceController.FINAL_CRACKS, 1.833f));

        /*
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndHideGameObjectIceController(IceController.ICE_PLANE, 22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndHideGameObjectIceController(IceController.FINAL_CRACKS, 22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndDisableAllIceCracks(22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndShowGameObjectIceController(IceController.ICE_CRACKS, 22.433f));
        */

        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndHideGameObjectIceController(IceController.ICE_PLANE, 22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndHideGameObjectIceController(IceController.FINAL_CRACKS, 22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndDisableAllIceCracks(22.433f));
        sequence.AddSequenceLineWithoutControl(args => sequence.WaitAndShowGameObjectIceController(IceController.ICE_CRACKS, 22.433f));

        return sequence;
    }

    private Sequence Sequence4()
    {
        SequenceLevel1 sequence = new SequenceLevel1();

        //<<ANIMATIONS>>

        //<DRAGON>>
        //sequence.AddSequenceLine(args => sequence.SetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FIFTH_SEQUENCE));                     //line 1 of sequence 5
        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FIFTH_SEQUENCE, 0.0f));                     //line 1 of sequence 5

        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY, 0.1f));                  //line 2 of sequence 5
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 0f));                  //line 2 of sequence 5

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 0.866f));          //line 3 of sequence 5
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 4.3f));           //line 4 of sequence 5

        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 5.133f));          //line 5 of sequence 5
        //sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.SWIM_LEFT, 9.066f));         //line 6 of sequence 5

        /*
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 9.2f));                 //line 7 of sequence 5

        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY_SKILL, 0.1f, 9.2f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_SKILL, 9.2f));

        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY, 0.1f, 11.3f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 11.3f));                 //line 8 of sequence 5
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 14.0f));                //line 9 of sequence 5

        //<<SOUND>>

        //<DRAGON>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.DRAGON, SoundInformationLevel1.FLY, 1.083f));                    //line 10 of sequence 5
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 9.2f));                                          //line 11 of sequence 5

        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel1.DRAGON, SoundInformationLevel1.FLY, 11.6f));                     //line 12 of sequence 5
        //sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 14.0f));                                         //line 13 of sequence 5
        */

        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 12.53f));                 //line 7 of sequence 5

        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY_SKILL, 0.1f, 13.5f));
        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY_SKILL, 9.2f));

        sequence.AddSequenceLine(args => sequence.WaitAndSetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.FLY, 0.1f, 16.4f));
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 16.4f));                 //line 8 of sequence 5
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel1.DRAGON, AnimationInformationLevel1.FLY, 19f));                //line 9 of sequence 5

        //<<SOUND>>

        if (Settings.binauralSound)
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 0.6f));                    //line 10 of sequence 5
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.DRAGON, 13.53f));                                          //line 11 of sequence 5

            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 16.4f));                     //line 12 of sequence 5
        }
        else
        {
            //<DRAGON>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 0.6f));                    //line 10 of sequence 5
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv1.DRAGON, 13.53f));                                          //line 11 of sequence 5

            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel1.FLY, AT_IDX_lv1.DRAGON, 16.4f));                     //line 12 of sequence 5
        }

        //<ICE>
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.ICE, 2f));

        return sequence;
    }

    public Sequence Sequence5()
    {
        SequenceLevel1 sequence = new SequenceLevel1();

        //<<ANIMATIONS>>

        //<DRAGON>>
        sequence.AddSequenceLine(args => sequence.SetCrossFade(AnimationInformationLevel1.DRAGON, AnimationStatesLevel1.SIXTH_SEQUENCE, 0f));  //line 1 of sequence 6

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(2.0f, 3.0f));  //line 2 of sequence 6
        sequence.AddSequenceLine(args => sequence.JustWait(5.0f));  //line 3 of sequence 6

        //<<SOUND>>

        //<DRAGON>
        //sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel1.DRAGON, 4.0f));

        //<<SPECIAL METHODS>>
        sequence.AddSequenceLineWithoutControl(args => sequence.FadeDecreaseVolumeFromTo(SoundInformationLevel1.AMBIENT, 0.115f, 2f, 5f));

        return sequence;
    }

    public List<Sequence> GetLevelSequences()
    {
        List<Sequence> sequences = new List<Sequence>();

        sequences.Add(Sequence1());
        sequences.Add(Sequence2());
        sequences.Add(SequenceIngame());
        sequences.Add(Sequence3());
        sequences.Add(Sequence4());
        sequences.Add(Sequence5());

        return sequences;
    }
}

public class SequenceInformationLevel2 : SequenceInformation
{
    //<--------------------SEQUENCE INDEXES------------------------>
    public const int SEQUENCE_1 = 0;
    public const int SEQUENCE_2 = 1;
    public const int SEQUENCE_3 = 2;

    //<-----------------------SEQUENCES---------------------------->

    /*
    private Sequence Sequence1()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(0f, 0.0f));  //line 36 of sequence 1
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(3.0f, 10.0f, GameConstants.ACTIVE));  //line 13 of sequence 1

        //<<MAPS>>
        sequence.AddSequenceLine(args => sequence.ShowMap(0.0f, GameConstants.MAP_BEFORE_EXPLOSION));  //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideMap(0.0f, GameConstants.MAP_AFTER_EXPLOSION)); //line 2 of sequence 1

        sequence.AddSequenceLine(args => sequence.ShowMap(45.0f, GameConstants.MAP_AFTER_EXPLOSION));  //line 3 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideMap(45.0f, GameConstants.MAP_BEFORE_EXPLOSION));  //line 4 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(40.0f, GameConstants.STONES_VISUAL));  //line 5 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(40.0f, GameConstants.TREES_VISUAL));   //line 6 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(40.0f, GameConstants.BRIDGE_EMPTY_VISUAL));  //line 7 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(40.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 8 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(40.0f, GameConstants.BRIDGE_FULL_VISUAL));  //line 9 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(40.0f, GameConstants.TABLA_INDIVIDUAL));  //line 10 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(40.0f, GameConstants.TABLA_COLECTIVA));  //line 11 of sequence 1

        sequence.AddSequenceLine(args => sequence.HideVisual(116f, GameConstants.STONES_VISUAL));  //line 12 of sequence 1

        //<<VOLCANO SMOKE>>
        sequence.AddSequenceLine(args => sequence.HideSmoke(0.0f));   //line 14 of sequence 1

        sequence.AddSequenceLine(args => sequence.ChangeSmokeTransparencyProgressive(30.0f, 3.0f));   //line 15 of sequence 1
        sequence.AddSequenceLine(args => sequence.TranslateSmoke(31.0f, 14.0f));   //line 16 of sequence 1

        sequence.AddSequenceLine(args => sequence.ChangeSmokeTransparencyProgressive(46.0f, 3.0f, true)); //line 17 of sequence 1

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ShakeMap(30f, 15f, 1f, GameConstants.MAP_BEFORE_EXPLOSION, GameConstants.GLOBAL_TRANSFORM)); //line 18 of sequence 1
        sequence.AddSequenceLine(args => sequence.ZoomMap(62.0f, 24.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.GLOBAL_TRANSFORM, GameConstants.PHASE_1_TRANSFORM));     //line 19 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_INDIVIDUAL));  //line 37 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_COLECTIVA));  //line 43 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(95.05f, GameConstants.PIEDRA_INDIVIDUAL));  //line 38 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(113.13f, GameConstants.PIEDRA_COLECTIVA));  //line 41 of sequence 1

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.AMBIENCE, SoundInformationLevel2.FOREST_1_SOUND, 3.0f));    //line 20 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.DRAGON, SoundInformationLevel2.FLY, 0.4f));    //line 21 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.AMBIENCE, 30.0f));     //line 22 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON, SoundInformationLevel2.DRAGON_GROWL, 30.5f));    //line 23 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.VOLCANO, SoundInformationLevel2.VOLCANO_2_SOUND, 30.0f));    //line 24 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.VOLCANO, 46.0f));     //line 25 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.AMBIENCE, SoundInformationLevel2.FOREST_2_SOUND, 49.0f));    //line 26 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR, SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 53.5f));    //line 27 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR, SoundInformationLevel2.CAR_STOPS, 69.1f));           //line 28 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR, SoundInformationLevel2.CAR_CLAXON_SOUND, 70.5f));    //line 29 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR, SoundInformationLevel2.CAR_WAITING, 71.5f));    //line 30 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.HUMAN, SoundInformationLevel2.HELPING_CHARACTER, 79f));

        //<<NARRATIVE>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE, SoundInformationLevel2.NARRATIVE_1, 13.0f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE, SoundInformationLevel2.NARRATIVE_2, 54.5f));

        //<SQUIRREL INDIVIDUAL STONE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 86f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 88.26f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 90.26f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.INTERACTION, SoundInformationLevel2.STONE_DRAGGING, 90.26f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 95.06f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.INTERACTION, SoundInformationLevel2.POP_SOUND, 95.05f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 96.0f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 98f));

        //<SQUIRREL COLECTIVE STONE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 98f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 101f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 102.3f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 113.05f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.INTERACTION, SoundInformationLevel2.POP_SOUND, 113.13f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL, SoundInformationLevel2.SQUIRREL_WALKING, 113.23f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 116f));

        //<NARRATIVE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE, SoundInformationLevel2.NARRATIVE_3, 82f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE, SoundInformationLevel2.NARRATIVE_4, 86f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE, SoundInformationLevel2.NARRATIVE_5, 98f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.DRAGON_PATH_START, 0.0f)); //line 31 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.FLY, 0.0f)); //line 32 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.FLY, 32.066f)); //line 33 of sequence 3

        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_1_TRIGGER, 53.5f));    //line 34 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_1_TRIGGER, 51.5f));    //line 34 of sequence 1
        
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_IND, AnimationInformationLevel2.ARDILLA_PIEDRA_INDIVIDUAL, 86f));  //line 39 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_COL, AnimationInformationLevel2.ARDILLA_PIEDRA_COLECTIVO, 98f));  //line 40 of sequence 1

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION, DataExtractorCSVLevel2.START, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION_STONES, DataExtractorCSVLevel2.START, 86.0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.STONES, DataExtractorCSVLevel2.START, 116f));

        return sequence;
    }
    */

    private Sequence Sequence1()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        GameObject ardillaTroncoIndividual1 = GameObject.Find("ArdillaTroncoIndividual@Anim");
        GameObject ardillaTroncoColectivo1 = GameObject.Find("ArdillaTroncoColectiva@Anim");
        GameObject ardillaTroncoIndividual2 = GameObject.Find("ArdillaTroncoIndividual2");
        GameObject ardillaTroncoColectivo2 = GameObject.Find("ArdillaTroncoColectiva2");

        if (CheckboxManagerLevel2.pullTreeMechanic)
        {
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoIndividual1));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoColectivo1));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoIndividual2));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoColectivo2));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoIndividual1));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoColectivo1));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoIndividual2));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoColectivo2));
        }

        //<<CAMERA>>
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(0f, 0f, 0, GameConstants.ACTIVE));
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(0f, 0f, 1, GameConstants.ACTIVE));

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(0f, 0.0f));  //line 36 of sequence 1
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(3.0f, 10.0f, GameConstants.ACTIVE));  //line 13 of sequence 1

        //<<MAPS>>
        sequence.AddSequenceLine(args => sequence.ShowMap(0.0f, GameConstants.MAP_BEFORE_EXPLOSION));  //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideMap(0.0f, GameConstants.MAP_AFTER_EXPLOSION)); //line 2 of sequence 1

        sequence.AddSequenceLine(args => sequence.ShowMap(32.0f, GameConstants.MAP_AFTER_EXPLOSION));  //line 3 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideMap(32.0f, GameConstants.MAP_BEFORE_EXPLOSION));  //line 4 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(35.0f, GameConstants.STONES_VISUAL));  //line 5 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(35.0f, GameConstants.TREES_VISUAL));   //line 6 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(35.0f, GameConstants.BRIDGE_EMPTY_VISUAL));  //line 7 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(35.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 8 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(35.0f, GameConstants.BRIDGE_FULL_VISUAL));  //line 9 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(35.0f, GameConstants.TABLA_INDIVIDUAL));  //line 10 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(35.0f, GameConstants.TABLA_COLECTIVA));  //line 11 of sequence 1

        sequence.AddSequenceLine(args => sequence.HideVisual(103f, GameConstants.STONES_VISUAL));  //line 12 of sequence 1

        //<<VOLCANO SMOKE>>
        sequence.AddSequenceLine(args => sequence.HideSmoke(0.0f));   //line 14 of sequence 1

        sequence.AddSequenceLine(args => sequence.ChangeSmokeTransparencyProgressive(20.0f, 3.0f));   //line 15 of sequence 1
        sequence.AddSequenceLine(args => sequence.TranslateSmoke(21.0f, 14.0f));   //line 16 of sequence 1

        sequence.AddSequenceLine(args => sequence.ChangeSmokeTransparencyProgressive(36.0f, 3.0f, true)); //line 17 of sequence 1

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ShakeMap(20f, 15f, 1f, GameConstants.MAP_BEFORE_EXPLOSION, GameConstants.GLOBAL_TRANSFORM)); //line 18 of sequence 1
        sequence.AddSequenceLine(args => sequence.ZoomMap(47.0f, 24.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.GLOBAL_TRANSFORM, GameConstants.PHASE_1_TRANSFORM));     //line 19 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_INDIVIDUAL));  //line 37 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_COLECTIVA));  //line 43 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(82.05f, GameConstants.PIEDRA_INDIVIDUAL));  //line 38 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(100.13f, GameConstants.PIEDRA_COLECTIVA));  //line 41 of sequence 1

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FOREST_1_SOUND, 3.0f));    //line 20 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.AMBIENCE, 20.0f));     //line 22 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FOREST_2_SOUND, 39.0f));    //line 26 of sequence 1

        if (Settings.binauralSound)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.FLY, AT_IDX_lv2.DRAGON_BEFORE_EXP, 0.4f));    //line 21 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_GROWL, AT_IDX_lv2.DRAGON_BEFORE_EXP, 20.5f));    //line 23 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.VOLCANO_2_SOUND, AT_IDX_lv2.VOLCANO, 20.0f));    //line 24 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.VOLCANO, 36.0f));     //line 25 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, AT_IDX_lv2.CAR, 40.5f));    //line 27 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_STOPS, AT_IDX_lv2.CAR, 56.1f));           //line 28 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, AT_IDX_lv2.CAR, 57.5f));    //line 29 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, AT_IDX_lv2.CAR, 58.5f));    //line 30 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.HELPING_CHARACTER, AT_IDX_lv2.HUMANS, 66f));

            //<SQUIRREL INDIVIDUAL STONE>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH1, 73f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH1, 75.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH1, 77.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.STONE_DRAGGING, AT_IDX_lv2.STONE_IND_PH1, 77.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH1, 82.06f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.POP_SOUND, AT_IDX_lv2.STONE_IND_PH1, 82.05f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH1, 83.0f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH1, 85f));

            //<SQUIRREL COLECTIVE STONE>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH1, 85f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH1, 88f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH1, 89.3f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH1, 100.05f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.POP_SOUND, AT_IDX_lv2.STONE_COL_PH1, 100.13f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH1, 100.23f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH1, 103f));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 0.4f));    //line 21 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_GROWL, 20.5f));    //line 23 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.VOLCANO_2_SOUND, 20.0f));    //line 24 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.VOLCANO, 36.0f));     //line 25 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 40.5f));    //line 27 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_STOPS, 56.1f));           //line 28 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, 57.5f));    //line 29 of sequence 1
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, 58.5f));    //line 30 of sequence 1

            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.HELPING_CHARACTER, 66f));

            //<SQUIRREL INDIVIDUAL STONE>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 73f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 75.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 77.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.STONE_DRAGGING, 77.26f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 82.06f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.POP_SOUND, 82.05f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 83.0f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 85f));

            //<SQUIRREL COLECTIVE STONE>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 85f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 88f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 89.3f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 100.05f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.POP_SOUND, 100.13f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 100.23f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 103f));
        }

        //<<NARRATIVE>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_1, 4.0f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_2, 41.5f));

        //<NARRATIVE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_3, 69f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_4, 73f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_5, 85f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.DRAGON_PATH_START, 0.0f)); //line 31 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.FLY, 0.0f)); //line 32 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_1, AnimationInformationLevel2.FLY, 22.066f)); //line 33 of sequence 3

        //sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_1_TRIGGER, 53.5f));    //line 34 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_1_TRIGGER, 38.5f));    //line 34 of sequence 1

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_IND, AnimationInformationLevel2.ARDILLA_PIEDRA_INDIVIDUAL, 73f));  //line 39 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_COL, AnimationInformationLevel2.ARDILLA_PIEDRA_COLECTIVO, 85f));  //line 40 of sequence 1

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION, DataExtractorCSVLevel2.START, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION_STONES, DataExtractorCSVLevel2.START, 73.0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.STONES, DataExtractorCSVLevel2.START, 103f));

        return sequence;
    }

    private Sequence Sequence2()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ZoomMap(5.5f, 36.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_1_TRANSFORM, GameConstants.PHASE_2_TRANSFORM));  //line 1 of sequence 2

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 2 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(56.75f, GameConstants.TABLA_INDIVIDUAL));  //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(72.96f, GameConstants.TABLA_COLECTIVA));  //line 4 of sequence 2

        sequence.AddSequenceLine(args => sequence.HideVisual(78.15f, GameConstants.TREES_VISUAL));  //line 5 of sequence 2

        //<<SOUND>>
        if (Settings.binauralSound)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_TURNS_ON, AT_IDX_lv2.CAR, 2.2f));    //line 6 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, AT_IDX_lv2.CAR, 3.2f));    //line 7 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_STOPS, AT_IDX_lv2.CAR, 44.1f));   //line 8 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, AT_IDX_lv2.CAR, 45.6f));    //line 9 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, AT_IDX_lv2.CAR, 46.6f));    //line 10 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.HELPING_CHARACTER, AT_IDX_lv2.HUMANS, 47.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.RIVER_SOUND, AT_IDX_lv2.RIVER, 34.5f));   //line 11 of sequence 2

            //<SQUIRREL INDIVIDUAL LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH2, 46.4f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH2, 47.4f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH2, 50.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, AT_IDX_lv2.TREE_IND_PH2, 50.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, AT_IDX_lv2.TREE_IND_PH2, 56.75f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH2, 61.2f));

            //<SQUIRREL COLECTIVE LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH2, 62.85f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, AT_IDX_lv2.TREE_COL_PH2, 68.85f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, AT_IDX_lv2.TREE_COL_PH2, 72.96f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH2, 73.15f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH2, 74.15f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH2, 78.15f));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_TURNS_ON, 2.2f));    //line 6 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 3.2f));    //line 7 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_STOPS, 44.1f));   //line 8 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, 45.6f));    //line 9 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, 46.6f));    //line 10 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.HELPING_CHARACTER, 47.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.RIVER_SOUND, 34.5f));   //line 11 of sequence 2

            //<SQUIRREL INDIVIDUAL LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 46.4f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 47.4f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 48.9f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, 52.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 56.75f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 61.2f));

            //<SQUIRREL COLECTIVE LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 62.85f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, 68.85f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 72.96f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 73.15f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 74.15f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 78.15f));
        }

        //<NARRATIVE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_3, 42.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_4, 46.6f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_5, 62.85f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_2_TRIGGER, 0.0f));  //line 14 of sequence 2

        if (CheckboxManagerLevel2.pullTreeMechanic)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND2, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL_2, 46.6f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL2, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO_2, 62.85f));  //line 16 of sequence 2
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL, 46.6f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO, 62.85f));  //line 16 of sequence 2
        }

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.STONES, DataExtractorCSVLevel2.END, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.TRANSITION_1, DataExtractorCSVLevel2.START, 3.2f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION_LOGS, DataExtractorCSVLevel2.START, 46.6f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.LOGS, DataExtractorCSVLevel2.START, 78.15f));

        return sequence;
    }

    private Sequence Sequence3()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ZoomMap(5.5f, 24.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_2_TRANSFORM, GameConstants.PHASE_3_1_TRANSFORM));   //line 1 of sequence 3

        sequence.AddSequenceLine(args => sequence.ZoomMap(29.5f, 36.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_3_1_TRANSFORM, GameConstants.PHASE_3_2_TRANSFORM));  //line 2 of sequence 3

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.BRIDGE_EMPTY_VISUAL));   //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.BRIDGE_FULL_VISUAL));   //line 4 of sequence 3

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.END_SECOND_LEVEL, 93.2f));

        if (Settings.binauralSound)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_TURNS_ON, AT_IDX_lv2.CAR, 2.2f));    //line 5 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, AT_IDX_lv2.CAR, 3.2f));    //line 6 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_STOPS, AT_IDX_lv2.CAR, 68.183f));   //line 7 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, AT_IDX_lv2.CAR, 69.2f));    //line 8 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_GROWL, AT_IDX_lv2.DRAGON, 69.2f));    //line 9 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 70.2f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.FLY, AT_IDX_lv2.DRAGON, 71.6f));    //line 10 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.RIVER, 24.0f));   //line 11 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, AT_IDX_lv2.DRAGON, 81.2f));    //line 12 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, AT_IDX_lv2.DRAGON, 82.7f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.FLY, AT_IDX_lv2.DRAGON, 84.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.DRAGON, 93.2f));   //line 20 of sequence 3
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_TURNS_ON, 2.2f));    //line 5 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 3.2f));    //line 6 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_STOPS, 68.183f));   //line 7 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, 69.2f));    //line 8 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_GROWL, 69.2f));    //line 9 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 70.2f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 71.6f));    //line 10 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.RIVER, 24.0f));   //line 11 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 81.2f));    //line 12 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 82.7f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 84.6f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.DRAGON, 93.2f));   //line 20 of sequence 3
        }

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_3_TRIGGER, 0.0f));    //line 13 of sequence 3

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.DRAGON_PATH_END, 71.2f)); //line 14 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 71.2f)); //line 15 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 81.2f)); //line 16 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 81.2f)); //line 17 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 84.2f)); //line 18 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 84.2f)); //line 19 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 93.2f)); //line 21 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.CLOSED_WINGS, 93.2f)); //line 22 of sequence 3

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.LOGS, DataExtractorCSVLevel2.END, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.TRANSITION_2, DataExtractorCSVLevel2.START, 3.2f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.CAVE, DataExtractorCSVLevel2.START, 71.2f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.CAVE, DataExtractorCSVLevel2.END, 96.2f));

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(96.2f, 5.0f));

        return sequence;
    }

    private Sequence Sequence2Shorter()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(17f, 0f, 0));
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(18f, 3f, 0, GameConstants.ACTIVE));
        sequence.AddSequenceLine(args => sequence.ZoomMap(17.5f, 0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_1_TRANSFORM, GameConstants.PHASE_2_TRANSFORM));  //line 1 of sequence 2

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 2 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(56.75f - 19f, GameConstants.TABLA_INDIVIDUAL));  //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(72.96f - 19f, GameConstants.TABLA_COLECTIVA));  //line 4 of sequence 2

        sequence.AddSequenceLine(args => sequence.HideVisual(78.15f - 19f, GameConstants.TREES_VISUAL));  //line 5 of sequence 2

        //<<SOUND>>
        if (Settings.binauralSound)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_TURNS_ON, AT_IDX_lv2.CAR, 2.2f));    //line 6 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, AT_IDX_lv2.CAR, 3.2f));    //line 7 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_STOPS, AT_IDX_lv2.CAR, 44.1f - 19f));   //line 8 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, AT_IDX_lv2.CAR, 45.6f - 19f));    //line 9 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, AT_IDX_lv2.CAR, 46.6f - 19f));    //line 10 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.HELPING_CHARACTER, AT_IDX_lv2.HUMANS, 47.6f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.RIVER_SOUND, AT_IDX_lv2.RIVER, 34.5f - 19f));   //line 11 of sequence 2

            //<SQUIRREL INDIVIDUAL LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH2, 46.4f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH2, 47.4f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_IND_PH2, 50.6f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, AT_IDX_lv2.TREE_IND_PH2, 50.6f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, AT_IDX_lv2.TREE_IND_PH2, 56.75f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_IND_PH2, 61.2f - 19f));

            //<SQUIRREL COLECTIVE LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH2, 62.85f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, AT_IDX_lv2.TREE_COL_PH2, 68.85f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, AT_IDX_lv2.TREE_COL_PH2, 72.96f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH2, 73.15f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, AT_IDX_lv2.SQUIRREL_COL_PH2, 74.15f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.SQUIRREL_COL_PH2, 78.15f - 19f));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_TURNS_ON, 2.2f));    //line 6 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 3.2f));    //line 7 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_STOPS, 44.1f - 19f));   //line 8 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_CLAXON_SOUND, 45.6f - 19f));    //line 9 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, 46.6f - 19f));    //line 10 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.HELPING_CHARACTER, 47.6f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.RIVER_SOUND, 34.5f - 19f));   //line 11 of sequence 2

            //<SQUIRREL INDIVIDUAL LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 46.4f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 47.4f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 48.9f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, 52.6f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 56.75f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 61.2f - 19f));

            //<SQUIRREL COLECTIVE LOG>
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 62.85f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.MOVING_LOG, 68.85f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 72.96f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 73.15f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.SQUIRREL_WALKING, 74.15f - 19f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.SQUIRREL, 78.15f - 19f));
        }

        //<NARRATIVE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_3, 42.6f - 19f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_4, 46.6f - 19f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.NARRATIVE_5, 62.85f - 19f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_2_SHORTER_TRIGGER, 0.0f));  //line 14 of sequence 2

        if (CheckboxManagerLevel2.pullTreeMechanic)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND2, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL_2, 46.6f - 19f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL2, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO_2, 62.85f - 19f));  //line 16 of sequence 2
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL, 46.6f - 19f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO, 62.85f - 19f));  //line 16 of sequence 2
        }

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.STONES, DataExtractorCSVLevel2.END, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.TRANSITION_1, DataExtractorCSVLevel2.START, 3.2f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.INTRODUCTION_LOGS, DataExtractorCSVLevel2.START, 46.6f - 19f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.LOGS, DataExtractorCSVLevel2.START, 78.15f - 19f));

        return sequence;
    }

    private Sequence Sequence3Shorter()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(23f, 0f, 1));
        sequence.AddSequenceLine(args => sequence.CameraCrossFade(24f, 3f, 1, GameConstants.ACTIVE));
        sequence.AddSequenceLine(args => sequence.ZoomMap(23.5f, 0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_2_TRANSFORM, GameConstants.PHASE_3_2_TRANSFORM));  //line 2 of sequence 3

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.BRIDGE_EMPTY_VISUAL));   //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.BRIDGE_FULL_VISUAL));   //line 4 of sequence 3

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.END_SECOND_LEVEL, 93.2f - 28.5f));

        if (Settings.binauralSound)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_TURNS_ON, AT_IDX_lv2.CAR, 2.2f));    //line 5 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, AT_IDX_lv2.CAR, 3.2f));    //line 6 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CAR_STOPS, AT_IDX_lv2.CAR, 39.683f));   //line 7 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, AT_IDX_lv2.CAR, 69.2f - 28.5f));    //line 8 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_GROWL, AT_IDX_lv2.DRAGON, 69.2f - 28.5f));    //line 9 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, AT_IDX_lv2.HUMANS, 70.2f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.FLY, AT_IDX_lv2.DRAGON, 71.6f - 28.5f));    //line 10 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.RIVER, 27.0f));   //line 11 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, AT_IDX_lv2.DRAGON, 81.2f - 28.5f));    //line 12 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, AT_IDX_lv2.DRAGON, 82.7f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralPlaySoundOnLoop(SoundInformationLevel2.FLY, AT_IDX_lv2.DRAGON, 84.6f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndBinauralStopPlayingSound(AT_IDX_lv2.DRAGON, 93.2f - 28.5f));   //line 20 of sequence 3
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 0f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_TURNS_ON, 2.2f));    //line 5 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.JEEP_IN_MOUNTAIN_1_SOUND, 3.2f));    //line 6 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CAR_STOPS, 39.683f));   //line 7 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.CAR_WAITING, 69.2f - 28.5f));    //line 8 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_GROWL, 69.2f - 28.5f));    //line 9 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.CELEBRATION_CHARACTERS, 70.2f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 71.6f - 28.5f));    //line 10 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.RIVER, 27.0f));   //line 11 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 81.2f - 28.5f));    //line 12 of sequence 3
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 82.7f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 84.6f - 28.5f));
            sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.DRAGON, 93.2f - 28.5f));   //line 20 of sequence 3
        }

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.JEEP, AnimationInformationLevel2.PHASE_3_SHORTER_TRIGGER, 0.0f));    //line 13 of sequence 3

        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.DRAGON_PATH_END, 71.2f-28.5f)); //line 14 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 71.2f - 28.5f)); //line 15 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 81.2f - 28.5f)); //line 16 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 81.2f - 28.5f)); //line 17 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 84.2f - 28.5f)); //line 18 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 84.2f - 28.5f)); //line 19 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 93.2f - 28.5f)); //line 21 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.CLOSED_WINGS, 93.2f - 28.5f)); //line 22 of sequence 3

        //<<DATA EXTRACTION>> WritePhaseLineToDataExtractor(int phase, int event_, float seconds)
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.LOGS, DataExtractorCSVLevel2.END, 0f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.TRANSITION_2, DataExtractorCSVLevel2.START, 3.2f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.CAVE, DataExtractorCSVLevel2.START, 71.2f - 28.5f));
        sequence.AddSequenceLine(args => sequence.WritePhaseLineToDataExtractor(DataExtractorCSVLevel2.CAVE, DataExtractorCSVLevel2.END, 96.2f - 28.5f));
        
        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(96.2f - 28.5f, 5.0f));
        
        return sequence;
    }

    private Sequence Sequence1Skip()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        GameObject ardillaTroncoIndividual1 = GameObject.Find("ArdillaTroncoIndividual@Anim");
        GameObject ardillaTroncoColectivo1 = GameObject.Find("ArdillaTroncoColectiva@Anim");
        GameObject ardillaTroncoIndividual2 = GameObject.Find("ArdillaTroncoIndividual2");
        GameObject ardillaTroncoColectivo2 = GameObject.Find("ArdillaTroncoColectiva2");

        if (CheckboxManagerLevel2.pullTreeMechanic)
        {
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoIndividual1));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoColectivo1));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoIndividual2));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoColectivo2));
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoIndividual1));
            sequence.AddSequenceLine(args => sequence.ShowObject(ardillaTroncoColectivo1));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoIndividual2));
            sequence.AddSequenceLine(args => sequence.HideObject(ardillaTroncoColectivo2));
        }

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(0f, 0.0f));  //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(3.0f, 1.0f, GameConstants.ACTIVE));  //line 2 of sequence 1

        //<<MAPS>>
        sequence.AddSequenceLine(args => sequence.ShowMap(0.0f, GameConstants.MAP_AFTER_EXPLOSION));  //line 3 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideMap(0.0f, GameConstants.MAP_BEFORE_EXPLOSION));  //line 4 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.STONES_VISUAL));  //line 5 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.TREES_VISUAL));   //line 6 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.BRIDGE_EMPTY_VISUAL));  //line 7 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 8 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.BRIDGE_FULL_VISUAL));  //line 9 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.TABLA_INDIVIDUAL));  //line 10 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.TABLA_COLECTIVA));  //line 11 of sequence 1

        sequence.AddSequenceLine(args => sequence.HideVisual(15.13f, GameConstants.STONES_VISUAL));  //line 12 of sequence 1

        //<<VOLCANO SMOKE>>
        sequence.AddSequenceLine(args => sequence.HideSmoke(0.0f));   //line 14 of sequence 1

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ZoomMap(0.0f, 0.0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.GLOBAL_TRANSFORM, GameConstants.PHASE_1_TRANSFORM));     //line 19 of sequence 1

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_INDIVIDUAL));  //line 37 of sequence 1
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.PIEDRA_COLECTIVA));  //line 43 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(9.05f, GameConstants.PIEDRA_INDIVIDUAL));  //line 38 of sequence 1
        sequence.AddSequenceLine(args => sequence.ShowVisual(15.13f, GameConstants.PIEDRA_COLECTIVA));  //line 41 of sequence 1

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FOREST_2_SOUND, 3.0f));    //line 26 of sequence 1

        //<SQUIRREL INDIVIDUAL STONE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.POP_SOUND, 9.05f));

        //<SQUIRREL COLECTIVE STONE>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.POP_SOUND, 15.13f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_IND, AnimationInformationLevel2.ARDILLA_PIEDRA_INDIVIDUAL, 0f));  //line 39 of sequence 1
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE1_COL, AnimationInformationLevel2.ARDILLA_PIEDRA_COLECTIVO, 0f));  //line 40 of sequence 1

        return sequence;
    }

    private Sequence Sequence2Skip()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(0f, 1.0f));  //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(2.1f, 1.0f, GameConstants.ACTIVE));  //line 2 of sequence 1

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ZoomMap(2f, 0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_1_TRANSFORM, GameConstants.PHASE_2_TRANSFORM));  //line 1 of sequence 2

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.STONES_PLACEMENTS_VISUAL));  //line 2 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(10.15f, GameConstants.TABLA_INDIVIDUAL));  //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(10.13f, GameConstants.TABLA_COLECTIVA));  //line 4 of sequence 2

        sequence.AddSequenceLine(args => sequence.HideVisual(12.11f, GameConstants.TREES_VISUAL));  //line 5 of sequence 2

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.RIVER_SOUND, 2f));   //line 11 of sequence 2

        //<SQUIRREL INDIVIDUAL LOG>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 10.15f));

        //<SQUIRREL COLECTIVE LOG>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.WOOD_PLANK_SOUND, 10.13f));

        //<<ANIMATIONS>>
        if (CheckboxManagerLevel2.pullTreeMechanic)
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND2, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL_2, 0f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL2, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO_2, 0f));  //line 16 of sequence 2
        }
        else
        {
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_IND, AnimationInformationLevel2.ARDILLA_TRONCO_INDIVIDUAL, 0f));  //line 15 of sequence 2
            sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.ARDILLA_PHASE2_COL, AnimationInformationLevel2.ARDILLA_TRONCO_COLECTIVO, 0f));  //line 16 of sequence 2
        }

        return sequence;
    }

    private Sequence Sequence3Skip()
    {
        SequenceLevel2 sequence = new SequenceLevel2();

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(0f, 1.0f));  //line 1 of sequence 1
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(2.1f, 1.0f, GameConstants.ACTIVE));  //line 2 of sequence 1

        //<<MAP ZOOM>>
        sequence.AddSequenceLine(args => sequence.ZoomMap(2f, 0f, GameConstants.MAP_AFTER_EXPLOSION,
            GameConstants.PHASE_2_TRANSFORM, GameConstants.PHASE_3_2_TRANSFORM));   //line 1 of sequence 3

        //<<MAPS VISUALS>>
        sequence.AddSequenceLine(args => sequence.HideVisual(0.0f, GameConstants.BRIDGE_EMPTY_VISUAL));   //line 3 of sequence 2
        sequence.AddSequenceLine(args => sequence.ShowVisual(0.0f, GameConstants.BRIDGE_FULL_VISUAL));   //line 4 of sequence 3

        //<<SOUND>>
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 2.4f));    //line 10 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.RIVER, 2.0f));   //line 11 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 12.4f));    //line 12 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.DRAGON_LOOP_END, 12.9f));
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySoundOnLoop(SoundInformationLevel2.FLY, 14.4f));
        sequence.AddSequenceLine(args => sequence.WaitAndStopPlayingSound(SoundInformationLevel2.DRAGON, 24.4f));   //line 20 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndPlaySound(SoundInformationLevel2.END_SECOND_LEVEL, 24.4f));

        //<<ANIMATIONS>>
        sequence.AddSequenceLine(args => sequence.WaitAndSetTrigger(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.DRAGON_PATH_END, 2.4f)); //line 14 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 2.4f)); //line 15 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 12.4f)); //line 16 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 12.4f)); //line 17 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.BACKFLIP, 15.4f)); //line 18 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 15.4f)); //line 19 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolFalse(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.FLY, 24.4f)); //line 21 of sequence 3
        sequence.AddSequenceLine(args => sequence.WaitAndSetBoolTrue(AnimationInformationLevel2.DRAGON_2, AnimationInformationLevel2.CLOSED_WINGS, 24.4f)); //line 22 of sequence 3

        //<<SCREEN FADE>>
        sequence.AddSequenceLine(args => sequence.BlackScreenFade(24.4f, 5.0f));

        return sequence;
    }

    public List<Sequence> GetLevelSequences()
    {
        List<Sequence> sequences = new List<Sequence>();

        if (CheckboxManagerLevel2.skip_story_sequences)
        {
            sequences.Add(Sequence1Skip());
            sequences.Add(Sequence2Skip());
            sequences.Add(Sequence3Skip());
        }
        else
        {
            sequences.Add(Sequence1());
            if (CheckboxManagerLevel2.shorterStorySequences)
            {
                sequences.Add(Sequence2Shorter());
                sequences.Add(Sequence3Shorter());
            }
            else
            {
                sequences.Add(Sequence2());
                sequences.Add(Sequence3());
            }
        }

        return sequences;
    }
}

public class SequenceController : MonoBehaviour
{
    private List<Sequence> sequences;

    void Start()
    {
        sequences = new List<Sequence>();

        string scene_name = SceneManager.GetActiveScene().name;

        if (scene_name.Equals("Level1"))
        {
            SequenceInformationLevel1 sequenceInformation = new SequenceInformationLevel1();
            sequences = sequenceInformation.GetLevelSequences();
        }

        if (scene_name.Equals("Level2"))
        {
            SequenceInformationLevel2 sequenceInformation = new SequenceInformationLevel2();
            sequences = sequenceInformation.GetLevelSequences();
        }
    }

    //<-----------------------METHODS THAT USES THE STAGE CONTROLLER------------------------->

    //method to check if a sequence has ended pt2
    private bool GetSequenceEnd(int sequence_idx)
    {
        return sequences[sequence_idx].CheckEndSequence();
    }

    //method to get if the input sequence is ready to be played
    public bool ReadyToPlaySequence(int sequence_idx)
    {
        if (sequence_idx > 0)
        {
            return (!GetSequenceEnd(sequence_idx) && GetSequenceEnd(sequence_idx - 1));
        }
        else
        {
            return !GetSequenceEnd(sequence_idx);
        }
    }

    //<------------------------METHODS TO USE THE STAGE CONTROLLER--------------------------->

    //<------------------------GET IF SEQUENCES ENDED--------------------------->
    public bool GetIfSequenceFinished(int sequence_idx) { return GetSequenceEnd(sequence_idx); }
    public bool GetIfSequencesFinished() { return GetSequenceEnd(sequences.Count-1); }

    //<------------------------FINISH SEQUENCES--------------------------->
    public void FinishSequence(int sequence_idx) { sequences[sequence_idx].FinishLineControlSequence(); }


    //<------------------------SEQUENCES--------------------------->

    public void ExecuteSequence(int sequence_idx, params object[] args)
    {
        if (ReadyToPlaySequence(sequence_idx)) //in case it is ready to be played the sequence
        {
            sequences[sequence_idx].ExecuteSequence(args);
        }
    }

    public void ExecuteSequences()
    {
        for(int i = 0; i < sequences.Count; i++) { ExecuteSequence(i); }
    }
}


//<-------------------------------SPECIAL METHOD TO DO A SPECIAL AUDIO FADE--------------------------------->
/*
//SPECIAL method created to fade the audio swim in the second sequence
private void SwimFade(int audio_source, float quantity, float from, float change, float to)
{
    float pitch_min = 0.7f; float pitch_max = 1.0f; //pitch range

    FadeIncreaseVolumeFromTo(audio_source, quantity, from, change);
    FadeDecreaseVolumeFromTo(audio_source, quantity, change, to);

    if (sc.GetPitch(audio_source) > pitch_min) { FadeDecreasePitchFromTo(audio_source, quantity, from, change); }
    if (sc.GetPitch(audio_source) < pitch_max) { FadeIncreasePitchFromTo(audio_source, quantity, change, to); }
}
*/