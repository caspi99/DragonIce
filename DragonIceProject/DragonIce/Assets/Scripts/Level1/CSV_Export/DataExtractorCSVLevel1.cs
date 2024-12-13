 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataExtractorCSVLevel1 : DataExtractorCSV
{
    //<--------------------PHASE INDEXES------------------------->
    public const int TRAINING = 0;
    public const int FAMILIARIZATION = 1;
    public const int GAME = 2;
    public const int INTRODUCTION = 3;
    public const int END = 4;

    //<--------------------EVENT INDEXES------------------------->
    public const int START = 0;
    public const int STONE_THROWING = 1;
    public const int STONE_FINAL_POSITION = 2;
    public const int STONE_STEPPING_THROWING = 3;
    public const int STONE_STEPPING_FINAL_POSITION = 4;

    //<---------------------EXTERNAL CONTROLLERS------------------------->
    public RoundController rc;


    //<---------------------DATA TYPES------------------------->
    [System.Serializable]
    public class DataRowMechanics
    {
        public string player;
        public string order;
        public string phase;
        public string event_;
        public string iteration;
        public string state;
        public string time;
        public string speed;
        public string angle;
        public string distance;
        public string x_position;
        public string y_position;
        public string perfectTiming;

        public DataRowMechanics(int p, int o, int ph, int e, int i, int s, float t, float sp, float a, float d, float x, float y, int pT)
        {
            if (p < 0) { player = ""; }
            else { player = players_tags[p]; }

            if (o < 0) { order = "NaN"; }
            else { order = o.ToString(); }

            if (ph < 0) { phase = ""; }
            else { phase = phase_tags[ph]; }

            if (e < 0) { event_ = ""; }
            else { event_ = event_tags[e]; }

            if (i < 0) { iteration = "NaN"; }
            else { iteration = i.ToString(); }

            if (s < 0) { state = "NaN"; }
            else { state = state_tags[s]; }

            if(pT < 0) { perfectTiming = "NaN"; }
            else { if (pT > 0) { perfectTiming = "True"; } else { perfectTiming = "False"; } }

            time = t.ToString().Replace(',', '.');
            speed = sp.ToString().Replace(',', '.');
            angle = a.ToString().Replace(',', '.');
            distance = d.ToString().Replace(',', '.');
            x_position = x.ToString().Replace(',', '.'); y_position = y.ToString().Replace(',', '.');
        }
    }

    private List<DataRowMechanics> mechanics_data_rows = new List<DataRowMechanics>();

    //<----------------------ADD DATA LINES METHODS (MECHANIC)-------------------------->

    //method called from NewGameController
    public void WriteDataLinePhaseStart(int phase)
    {
        if (continuous_data_saving)
        {
            WriteMechanicsLine(-1, -1, phase, START, -1, -1, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, -1);
        }
        else
        {
            AddMechanicsLine(-1, -1, phase, START, -1, -1, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, -1);
        }
    }

    //method called from RoundController
    public void WriteDataLineStoneThrowing(int stone_idx, int round_idx)
    {
        int order = rc.GetStoneOrder(stone_idx, round_idx);

        (float x, float y) = rc.GetCurrentStonePosition(stone_idx, round_idx);

        if (Settings.sync) { order = 1; }

        if (continuous_data_saving)
        {
            WriteMechanicsLine(stone_idx, order, rc.CheckRoundType(round_idx), STONE_THROWING,
                round_idx, -1, rc.GetStoneSpeed(stone_idx, round_idx), float.NaN, float.NaN, x, y, -1);
        }
        else
        {
            AddMechanicsLine(stone_idx, order, rc.CheckRoundType(round_idx), STONE_THROWING,
                round_idx, -1, rc.GetStoneSpeed(stone_idx, round_idx), float.NaN, float.NaN, x, y, -1);
        }
    }

    //Method called from a Stone
    public void WriteDataLineStoneFinalPosition(string stone_name)
    {
        (int stone_idx, int round_idx) = rc.GetAssignedPlayerAndCurrentRound(stone_name);

        (float angle, float distance, float x, float y) = rc.GetAngleDistanceXYTuple(stone_name);

        if (continuous_data_saving)
        {
            WriteMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_FINAL_POSITION,
                round_idx, -1, float.NaN, angle, distance, x, y, -1);
        }
        else
        {
            AddMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_FINAL_POSITION,
                round_idx, -1, float.NaN, angle, distance, x, y, -1);
        }
    }

    //Method called from a Stone
    public void WriteDataLineStoneSteppingThrowing(string stone_name)
    {
        (int stone_idx, int round_idx) = rc.GetAssignedPlayerAndCurrentRound(stone_name);

        (float x, float y) = rc.GetCurrentStonePosition(stone_idx, round_idx);

        if (continuous_data_saving)
        {
            WriteMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_STEPPING_THROWING,
                round_idx, ACTIVE, float.NaN, float.NaN, float.NaN, x, y, -1);
        }
        else
        {
            AddMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_STEPPING_THROWING,
                round_idx, ACTIVE, float.NaN, float.NaN, float.NaN, x, y, -1);
        }
    }

    //Method called from a Stone
    public void WriteDataLineStoneSteppingFinalPosition(string stone_name, int state, int pT)
    {
        (int stone_idx, int round_idx) = rc.GetAssignedPlayerAndCurrentRound(stone_name);

        if (continuous_data_saving)
        {
            WriteMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_STEPPING_FINAL_POSITION,
                round_idx, state, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, pT);
        }
        else
        {
            AddMechanicsLine(stone_idx, -1, rc.CheckRoundType(round_idx), STONE_STEPPING_FINAL_POSITION,
                round_idx, state, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, pT);
        }
    }

    //<----------------------METHODS THAT USES THE DATA EXTRACTOR CSV-------------------------->

    //method to add a mechanic line
    private void AddMechanicsLine(int p, int o, int ph, int e, int i, int s, float sp, float a, float d, float x, float y, int pT)
    {
        mechanics_data_rows.Add(new DataRowMechanics(p, o, ph, e, i, s, GetTime(), sp, a, d, x, y, pT));
    }

    //method to write the mechanics csv
    public void WriteMechanicsCSV()
    {
        if (enable_data_extraction)
        {
            TextWriter tw = new StreamWriter(filename_mechanics, false);
            tw.WriteLine(mechanics_head);      //Here we put the headers
            tw.Close();

            tw = new StreamWriter(filename_mechanics, true);

            for (int i = 0; i < mechanics_data_rows.Count; i++)
            {
                tw.WriteLine(mechanics_data_rows[i].player +
                    "," + mechanics_data_rows[i].order +
                    "," + mechanics_data_rows[i].phase +
                    "," + mechanics_data_rows[i].event_ +
                    "," + mechanics_data_rows[i].iteration +
                    "," + mechanics_data_rows[i].state +
                    "," + mechanics_data_rows[i].time +
                    "," + mechanics_data_rows[i].speed +
                    "," + mechanics_data_rows[i].angle +
                    "," + mechanics_data_rows[i].distance +
                    "," + mechanics_data_rows[i].x_position +
                    "," + mechanics_data_rows[i].y_position +
                    "," + mechanics_data_rows[i].perfectTiming);
            }

            tw.Close();
        }
    }

    //<----------------------METHODS OF CONTINUOUS DATA EXTRACTION-------------------------->

    //method to init the filenames and date
    protected override void InitFilenameAndDateValues()
    {
        if (GameConstants.folder.Equals("None"))
        {
            string gamemode = "Async";

            if (Settings.sync) { gamemode = "Sync"; }

            // Get the parent directory of the application path
            string parentDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

            // Adjusted the folder path to use the parent directory and Exported_Data
            GameConstants.folder = Path.Combine(parentDirectory, "Exported_Data", gamemode);

            // Check if the folder exists, create it if not
            if (!Directory.Exists(GameConstants.folder))
            {
                Directory.CreateDirectory(GameConstants.folder);
            }
        }

        if (GameConstants.filename_mechanics.Equals("None") && GameConstants.filename_positions.Equals("None"))
        {
            date = DateTime.Now.ToString("yyyy_MM_dd_HH;mm");
            GameConstants.filename_mechanics = "Dragonice_Mechanics_[" + date + "]";
            GameConstants.filename_positions = "Dragonice_PositionsUsers_[" + date + "]";
        }

        filename_mechanics = Path.Combine(GameConstants.folder, GameConstants.filename_mechanics + ".csv");
        filename_positions = Path.Combine(GameConstants.folder, GameConstants.filename_positions + ".csv");
    }
    protected override void InitIndexes()
    {
        phase_tags = new string[] { "Training", "Familiarization", "Game", "Introduction", "End" };
        event_tags = new string[] { "Start", "Stone_Throwing", "Stone_FinalPosition", "Stone_Stepping_Throwing", "Stone_Stepping_FinalPosition" };
        mechanics_head = "Player, Order, Phase, Event, Iteration, State, Time, Speed, Angle, Distance, X_Position, Y_Position, PerfectTiming";
    }

    //method to write a mechanic line
    private void WriteMechanicsLine(int p, int o, int ph, int e, int i, int s, float sp, float a, float d, float x, float y, int pT)
    {
        if (enable_data_extraction)
        {
            DataRowMechanics mechanic_data_line = new DataRowMechanics(p, o, ph, e, i, s, GetTime(), sp, a, d, x, y, pT);

            TextWriter tw = new StreamWriter(filename_mechanics, true);

            tw.WriteLine(mechanic_data_line.player +
                    "," + mechanic_data_line.order +
                    "," + mechanic_data_line.phase +
                    "," + mechanic_data_line.event_ +
                    "," + mechanic_data_line.iteration +
                    "," + mechanic_data_line.state +
                    "," + mechanic_data_line.time +
                    "," + mechanic_data_line.speed +
                    "," + mechanic_data_line.angle +
                    "," + mechanic_data_line.distance +
                    "," + mechanic_data_line.x_position +
                    "," + mechanic_data_line.y_position +
                    "," + mechanic_data_line.perfectTiming);

            tw.Close();
        }
    }
}
