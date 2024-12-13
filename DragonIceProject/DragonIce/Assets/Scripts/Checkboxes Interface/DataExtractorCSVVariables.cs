using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class DataExtractorCSVVariables
{
    private static DataExtractorCSVVariables instance = null;
    private DataExtractorCSVVariables() { InitFilenameAndDateValues(); }

    public static DataExtractorCSVVariables Instance
    {
        get
        {
            if (instance == null)
                instance = new DataExtractorCSVVariables();
            return instance;
        }
    }

    private string filename_settings;

    //<---------------------DATA TYPES------------------------->
    [System.Serializable]
    public class DataRowVariables
    {
        public string variableName;
        public string value;

        public DataRowVariables(string varName, string value)
        {
            this.variableName = varName; this.value = value;
        }
    }

    private List<DataRowVariables> variables_data_rows = new List<DataRowVariables>();

    //method to init the filenames and date
    private void InitFilenameAndDateValues()
    {
        string gamemode = "Async";

        if (Settings.sync) { gamemode = "Sync"; }

        if (!Settings.play_level_1) { gamemode = "OnlyLevel2"; }

        string date = DateTime.Now.ToString("yyyy_MM_dd_HH;mm");

        // Get the parent directory of the application path
        string parentDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

        // Adjusted the folder path to use the parent directory and Exported_Data
        GameConstants.folder = Path.Combine(parentDirectory, "Exported_Data", gamemode);

        // Check if the folder exists, create it if not
        if (!Directory.Exists(GameConstants.folder))
        {
            Directory.CreateDirectory(GameConstants.folder);
        }

        GameConstants.filename_settings = "Dragonice_Settings_[" + date + "]";

        filename_settings = Path.Combine(GameConstants.folder, GameConstants.filename_settings + ".csv");
    }

    private void AddRowToVariablesDataRows(string variableName, string value)
    {
        variables_data_rows.Add(new DataRowVariables(variableName, value));
    }

    //method to write the position trackings csv
    private void WritePositionTrackingsCSV()
    {
        if (Settings.enableDataExtraction)
        {
            TextWriter tw = new StreamWriter(filename_settings, false);
            tw.WriteLine("Variable_Name, Value");      //Here we put the headers
            tw.Close();

            tw = new StreamWriter(filename_settings, true);

            for (int i = 0; i < variables_data_rows.Count; i++)
            {
                tw.WriteLine(variables_data_rows[i].variableName +
                    "," + variables_data_rows[i].value);
            }

            tw.Close();
        }
    }

    //Method to get all the checkboxes and settings and write them into the csv
    public void GetSettingsAndSaveCsv()
    {
        // Clear existing data rows
        variables_data_rows.Clear();

        // Get and store settings
        foreach (GroupOfCheckboxes group in GlobalGroupsOfCheckboxes.Instance.groupsOfCheckboxes)
        {
            bool storeCondition = (Settings.sync && group.id == (int)checkboxesId.LEVEL1_ASYNC) 
                || (!Settings.sync && group.id == (int)checkboxesId.LEVEL1_SYNC);

            if (!storeCondition)
            {
                AddRowToVariablesDataRows(group.GetTitle(language.ENGLISH), "");
                foreach (GroupOfProperties gProp in group.gProperties)
                {
                    AddRowToVariablesDataRows(gProp.GetTitle(language.ENGLISH), "");
                    foreach (PropertyExtended prop in gProp.properties)
                    {
                        object value = prop.property.GetValue(null);
                        string valueString = value != null ? GetValueAsString(value).Replace(",", ".") : "!ERROR";
                        AddRowToVariablesDataRows(prop.property.Name, valueString);
                    }
                }
            }
        }

        // Write the data to CSV
        WritePositionTrackingsCSV();
    }

    private string GetValueAsString(object value)
    {
        if (value.GetType().IsArray)
        {
            // Handle arrays separately
            Array array = (Array)value;
            if (array.Length > 0)
            {
                string[] elements = new string[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    elements[i] = array.GetValue(i).ToString();
                }
                return "[" + string.Join("_", elements) + "]";
            }
            else
            {
                return "empty_array";
            }
        }
        else
        {
            return value.ToString();
        }
    }
}
