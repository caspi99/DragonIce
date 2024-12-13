using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataExtractorCSVLevel2 : DataExtractorCSV
{
    //<--------------------PHASE INDEXES------------------------->
    public const int INTRODUCTION = 0;
    public const int INTRODUCTION_STONES = 1;
    public const int STONES = 2;
    public const int TRANSITION_1 = 3;
    public const int INTRODUCTION_LOGS = 4;
    public const int LOGS = 5;
    public const int TRANSITION_2 = 6;
    public const int CAVE = 7;

    //<--------------------EVENT INDEXES------------------------->
    public const int START = 0;
    public const int MOVING = 1;
    public const int INCORPORATING = 2;
    public const int END = 3;

    //<--------------------MODALITY INDEXES------------------------->
    public const int INDIVIDUAL = 0;
    public const int SHARED = 1;

    private static string[] modality_tags = new string[] {};

    //<--------------------CONTROLLERS------------------------->
    private PlayersControllerLevel2 player_controller;

    //<---------------------DATA TYPES------------------------->
    [System.Serializable]
    public class DataRowMechanics
    {
        public string player;
        public string phase;
        public string event_;
        public string state;
        public string id;
        public string type;
        public string modality;
        public string number_of_players;
        public string time;

        public DataRowMechanics(int p, int ph, int e, int s, int i, int ty, int m, int n, float t)
        {
            if (p < 0) { player = "NaN"; }
            else { player = players_tags[p]; }

            if (ph < 0) { phase = ""; }
            else { phase = phase_tags[ph]; }

            if (e < 0) { event_ = ""; }
            else { event_ = event_tags[e]; }

            if (s < 0) { state = ""; }
            else { state = state_tags[s]; }

            if (i < 0) { id = "NaN"; }
            else { id = i.ToString(); }

            if (ty < 0) { type = "NaN"; }
            else { type = ty.ToString(); }

            if (m < 0) { modality = "NaN"; }
            else { modality = modality_tags[m]; }

            if (n < 0) { number_of_players = "NaN"; }
            else { number_of_players = n.ToString(); }

            time = t.ToString().Replace(',', '.');
        }
    }

    private List<DataRowMechanics> mechanics_data_rows = new List<DataRowMechanics>();

    //<----------------------ADD DATA LINES METHODS (MECHANIC)-------------------------->

    //method called from NewGameController
    public void WriteDataLinePhaseStart(int phase)
    {
        if (continuous_data_saving)
        {
            WriteMechanicsLine(-1, phase, START, -1, -1, -1, -1, -1);
        }
        else
        {
            AddMechanicsLine(-1, phase, START, -1, -1, -1, -1, -1);
        }
    }

    public void WriteDataLinePhaseEnd(int phase)
    {
        if (continuous_data_saving)
        {
            WriteMechanicsLine(-1, phase, END, -1, -1, -1, -1, -1);
        }
        else
        {
            AddMechanicsLine(-1, phase, END, -1, -1, -1, -1, -1);
        }
    }

    //method called from RoundController
    public void WriteDataLineMovingGrabbable(int p, int s, int id, int ty)
    {
        int modality = INDIVIDUAL;
        int number_of_players = player_controller.GetNumberOfPlayersInteractingGrabbable(id);
        List<int> all_players = player_controller.GetListOfPlayersInteractingGrabbable(id);

        if (number_of_players > 1) { modality = SHARED; }

        if (continuous_data_saving)
        {
            WriteMechanicsLine(p, curr_phase, MOVING, s, id, ty, modality, number_of_players);

            //Refresh other players
            for (int i = 0; i < all_players.Count; i++)
            {
                if (all_players[i] != p) { WriteMechanicsLine(all_players[i], curr_phase, MOVING, ACTIVE, id, ty, modality, number_of_players); }
            }
        }
        else
        {
            AddMechanicsLine(p, curr_phase, MOVING, s, id, ty, modality, number_of_players);

            //Refresh other players
            for (int i = 0; i < all_players.Count; i++)
            {
                if (all_players[i] != p) { AddMechanicsLine(all_players[i], curr_phase, MOVING, ACTIVE, id, ty, modality, number_of_players); }
            }
        }
    }

    public void WriteDataLinePlacingGrabbable(int id, int ty)
    {
        int modality = INDIVIDUAL;
        int number_of_players = player_controller.GetNumberOfPlayersInteractingGrabbable(id);
        List<int> all_players = player_controller.GetListOfPlayersInteractingGrabbable(id);

        if (number_of_players > 1) { modality = SHARED; }

        if (continuous_data_saving)
        {
            if(number_of_players == 0) { WriteMechanicsLine(-1, curr_phase, INCORPORATING, -1, id, ty, modality, number_of_players); }

            for(int i = 0; i < all_players.Count; i++)
            { WriteMechanicsLine(all_players[i], curr_phase, INCORPORATING, -1, id, ty, modality, number_of_players); }
        }
        else
        {
            if(number_of_players == 0) { AddMechanicsLine(-1, curr_phase, INCORPORATING, -1, id, ty, modality, number_of_players); }

            for (int i = 0; i < all_players.Count; i++)
            { AddMechanicsLine(all_players[i], curr_phase, INCORPORATING, -1, id, ty, modality, number_of_players); }
        }
    }

    //<----------------------METHODS THAT USES THE DATA EXTRACTOR CSV-------------------------->

    //method to add a mechanic line
    private void AddMechanicsLine(int p, int ph, int e, int s, int i, int ty, int m, int n)
    {
        mechanics_data_rows.Add(new DataRowMechanics(p, ph, e, s, i, ty, m, n, GetTime()));
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
                    "," + mechanics_data_rows[i].phase +
                    "," + mechanics_data_rows[i].event_ +
                    "," + mechanics_data_rows[i].state +
                    "," + mechanics_data_rows[i].id +
                    "," + mechanics_data_rows[i].type +
                    "," + mechanics_data_rows[i].modality +
                    "," + mechanics_data_rows[i].number_of_players +
                    "," + mechanics_data_rows[i].time);
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
            // Get the parent directory of the application path
            string parentDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

            // Adjusted the folder path to use the parent directory, Exported_Data, and OnlyLevel2
            GameConstants.folder = Path.Combine(parentDirectory, "Exported_Data", "OnlyLevel2");

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

        filename_mechanics = Path.Combine(GameConstants.folder, "Level2_" + GameConstants.filename_mechanics + ".csv");
        filename_positions = Path.Combine(GameConstants.folder, "Level2_" + GameConstants.filename_positions + ".csv");
    }
    protected override void InitIndexes()
    {
        phase_tags = new string[] { "Introduction", "Introduction_stones", "Stones", "Transition_1",
        "Introduction_logs", "Logs", "Transition_2", "Cave" };
        event_tags = new string[] { "Start", "Moving", "Incorporating", "End" };
        modality_tags = new string[] { "Individual", "Shared" };
        mechanics_head = "Player, Phase, Event, State, Id, Type, Modality, Number of players, Time";

        player_controller = GameObject.Find("PlayersController").GetComponent<PlayersControllerLevel2>();
    }

    //method to write a mechanic line
    private void WriteMechanicsLine(int p, int ph, int e, int s, int i, int ty, int m, int n)
    {
        if (enable_data_extraction)
        {
            DataRowMechanics mechanic_data_line = new DataRowMechanics(p, ph, e, s, i, ty, m, n, GetTime());

            TextWriter tw = new StreamWriter(filename_mechanics, true);

            tw.WriteLine(mechanic_data_line.player +
                    "," + mechanic_data_line.phase +
                    "," + mechanic_data_line.event_ +
                    "," + mechanic_data_line.state +
                    "," + mechanic_data_line.id +
                    "," + mechanic_data_line.type +
                    "," + mechanic_data_line.modality +
                    "," + mechanic_data_line.number_of_players +
                    "," + mechanic_data_line.time);

            tw.Close();
        }
    }
}
