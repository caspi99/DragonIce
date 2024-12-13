using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class DataExtractorCSV : MonoBehaviour
{
    
    [SerializeField] protected bool enable_data_extraction = true;

    //<--------------------PLAYERS INDEXES------------------------->
    public const int RED = 0;
    public const int GREEN = 1;
    public const int PURPLE = 2;
    public const int YELLOW = 3;

    protected static string[] players_tags = new string[] { "Red", "Green", "Purple", "Yellow" };
    protected static string[] phase_tags = new string[] {};
    protected static string[] event_tags = new string[] {};

    //<--------------------STATES INDEXES------------------------->
    public const int ACTIVE = 0;
    public const int INACTIVE = 1;

    protected static string[] state_tags = new string[] { "Active", "Inactive" };

    //<---------------------TIMER VARIABLE------------------------->
    protected int timer = -1;

    //<---------------------EXTERNAL CONTROLLERS------------------------->
    public TimeManager tm;

    //<---------------------TEXT WRITER------------------------->
    protected string date;
    protected string filename_mechanics;
    protected string filename_positions;

    protected bool continuous_data_saving; //only initializated if data stored contuniously
    protected bool data_flow; //only initializated if data stored contuniously

    protected int curr_phase;

    protected string mechanics_head;

    //<---------------------DATA TYPES------------------------->
    [System.Serializable]
    public class DataRowPositionsTracking
    {
        public string player;
        public string x_position;
        public string y_position;
        public string time;
        public string phase;

        public DataRowPositionsTracking(int p, float x, float y, float t, int ph)
        {
            if (p < 0) { player = ""; }
            else { player = players_tags[p]; }

            x_position = x.ToString().Replace(',', '.'); y_position = y.ToString().Replace(',', '.');

            time = t.ToString().Replace(',', '.');
            phase = phase_tags[ph];
        }
    }

    protected List<DataRowPositionsTracking> positions_tracking_data_rows = new List<DataRowPositionsTracking>();

    // Start is called before the first frame update
    protected void Start()
    {
        enable_data_extraction = Settings.enableDataExtraction;

        InitFilenameAndDateValues();
        InitIndexes();

        continuous_data_saving = Settings.continuous_data_saving;

        if (continuous_data_saving) { InitContinuousWriters(); }

        InitTimer();
        ResumeTimer();
    }

    //<----------------------TIMERS METHODS-------------------------->

    //method to init the timer
    protected void InitTimer()
    {
        if (timer < 0) { timer = tm.CreateTimer(); tm.PauseTimer(timer); }
    }

    //method to reanude timer
    protected void ResumeTimer() { tm.ResumeTimer(timer); }

    //method to reset the timer
    protected void ResetTimer() { tm.ResetTimer(timer); }

    //method to get the time of a timer
    protected float GetTime() { return tm.GetTime(timer); }

    //method to count the seconds of the stones without moving
    protected bool WaitTime(float time) { return tm.WaitTimeWithReset(timer, time); }

    //<----------------------ADD DATA LINES METHODS (POSITIONS)-------------------------->
    public void WriteDataLinePositionPlayer(int player, float x, float y)
    {
        if (continuous_data_saving)
        {
            if (data_flow) { WritePositionTrackingsLine(player, x, y); }
        }
        else
        {
            AddPositionTrackingsLine(player, x, y);
        }
    }

    //<----------------------METHODS THAT USES THE DATA EXTRACTOR CSV-------------------------->

    //method to add a mechanic line
    private void AddPositionTrackingsLine(int p, float x, float y)
    {
        positions_tracking_data_rows.Add(new DataRowPositionsTracking(p, x, y, GetTime(), curr_phase));
    }

    //method to write the position trackings csv
    public void WritePositionTrackingsCSV()
    {
        if (enable_data_extraction)
        {
            TextWriter tw = new StreamWriter(filename_positions, false);
            tw.WriteLine("Player, X_Position, Y_Position, Time, Phase");      //Here we put the headers
            tw.Close();

            tw = new StreamWriter(filename_positions, true);

            for (int i = 0; i < positions_tracking_data_rows.Count; i++)
            {
                tw.WriteLine(positions_tracking_data_rows[i].player +
                    "," + positions_tracking_data_rows[i].x_position +
                    "," + positions_tracking_data_rows[i].y_position +
                    "," + positions_tracking_data_rows[i].time +
                    "," + positions_tracking_data_rows[i].phase);
            }

            tw.Close();
        }
    }

    //<----------------------METHODS OF CONTINUOUS DATA EXTRACTION-------------------------->

    //method to init the filenames and date
    protected abstract void InitFilenameAndDateValues();

    protected abstract void InitIndexes();

    //method to init the text writers
    protected void InitContinuousWriters()
    {
        if (enable_data_extraction)
        {
            data_flow = true;

            //WRITING HEADERS
            TextWriter tw = new StreamWriter(filename_mechanics, false);
            tw.WriteLine(mechanics_head);
            tw.Close();

            tw = new StreamWriter(filename_positions, false);
            tw.WriteLine("Player, X_Position, Y_Position, Time, Phase");
            tw.Close();
        }
    }

    //method to write a position tracking line
    private void WritePositionTrackingsLine(int p, float x, float y)
    {
        if (enable_data_extraction)
        {
            DataRowPositionsTracking position_tracking_data_line = new DataRowPositionsTracking(p, x, y, GetTime(), curr_phase);

            TextWriter tw = new StreamWriter(filename_positions, true);

            tw.WriteLine(position_tracking_data_line.player +
                    "," + position_tracking_data_line.x_position +
                    "," + position_tracking_data_line.y_position +
                    "," + position_tracking_data_line.time +
                    "," + position_tracking_data_line.phase);

            tw.Close();
        }
    }

    //<----------------------METHODS OF DATA FLOW CONTROL-------------------------->

    private void ChangeDataFlow(bool status) { data_flow = status; }

    public void SetTrueDataFlow() { ChangeDataFlow(true); }
    public void SetFalseDataFlow() { ChangeDataFlow(false); }

    public void SetPhase(int value) { curr_phase = value; }
}
