using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private List<float> timers = new List<float>();
    private List<bool> time_count = new List<bool>();

    void Update()
    {
        Update_Time();
    }

    //<----------------------METHODS THAT USES THE TIME CONTROLLER-------------------------->

    //Method to update the timers
    private void Update_Time()
    {
        for(int i = 0; i<timers.Count; i++){
            if (time_count[i] && !GetIfFinishedTimer(i)) { timers[i] += Time.deltaTime; }
        }
    }

    //Method to change the time_count
    private void Change_Time_Count(int idx, bool status)
    {
        time_count[idx] = status;
    }

    //<------------------------METHODS TO USE THE TIME CONTROLLER--------------------------->

    //method to create a timer
    public int CreateTimer() { 
        timers.Add(0.0f); time_count.Add(true);

        return timers.Count - 1;
    }

    //method to get the time value of a timer
    public float GetTime(int idx) { return timers[idx]; }

    //method to set a time value to a timer
    public void SetTime(int idx, float time) { timers[idx] = time; }

    //method to know if the timer has arrived to a certain time
    public bool IsTime(int idx, float seconds) { return timers[idx] >= seconds; }

    public void ResumeTimer(int idx) { Change_Time_Count(idx, true); } //method to pause a timer
    public void PauseTimer(int idx) { Change_Time_Count(idx, false); } //method to pause a timer

    //method to reanude all timers
    public void ResumeAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { ResumeTimer(i); }
    }

    //method to pause all timers
    public void PauseAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { PauseTimer(i); }
    }

    public void RestartTimer(int idx) { timers[idx] = 0.0f; } //method to restart a timer

    //method to restart all timers
    public void RestartAllTimers()
    {
        for (int i = 0; i < timers.Count; i++) { RestartTimer(i); }
    }

    public void ResetTimer(int idx)
    {
        RestartTimer(idx);
        PauseTimer(idx);
    }

    public bool WaitTime(int idx, float time)
    {
        ResumeTimer(idx);

        return IsTime(idx, time);
    }

    public bool WaitTimeWithReset(int idx, float time)
    {
        ResumeTimer(idx);

        bool condition = IsTime(idx, time);

        if (condition) { ResetTimer(idx); }

        return condition;
    }

    //method to delete all timers
    public void DeleteTimers() { timers.Clear(); time_count.Clear(); }

    //method to get if a timer is "finished"
    public bool GetIfFinishedTimer(int idx)
    {
        return GetTime(idx) < 0.0f;
    }

    //method to set a timer as "finished" with a negative time
    public void SetFinishedTimer(int idx)
    {
        SetTime(idx, -1.0f);
    }
}
