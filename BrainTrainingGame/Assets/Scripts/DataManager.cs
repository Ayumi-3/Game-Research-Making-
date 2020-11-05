using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using CsvHelper;

public class DataManager : MonoBehaviour
{
    private FileMode mode;

    public void WriteData(string dir, string csvPath, Dictionary<string, string> data, bool overWrite, bool writeHeader)
    {
        // check if directory exist
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
           
        }
        
        if (overWrite)
        {
            mode = FileMode.Create;
        }
        else
        {
            mode = FileMode.Append;
        }
        
        // create new csv file
        using (var ftw = new FileStream(csvPath, mode))
        using (var sw = new StreamWriter(ftw))
        using (var wt = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture))
        {
            if (writeHeader)
            {
                foreach (string key in data.Keys)
                {
                    wt.WriteField(key);
                }

                wt.NextRecord();
            }

            foreach (string val in data.Values)
            {
                wt.WriteField(val);
            }

            wt.NextRecord();

            // close all
            wt.Dispose();
            sw.Close();
            ftw.Close();
        }
    }

    public List<string> ReadPlayerName(string csvPath)
    {
        List<string> playerName = new List<string>();
        string value;

        using (TextReader ftr = File.OpenText(csvPath))
        {
            var rt = new CsvReader(ftr, System.Globalization.CultureInfo.CurrentCulture);
            rt.Configuration.HasHeaderRecord = false;
            while (rt.Read())
            {
                for(int i = 0; rt.TryGetField<string>(i, out value); i++)
                {
                    playerName.Add(value);
                }
            }
        }

        return playerName;
    }
}
