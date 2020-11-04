using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using CsvHelper;

public class DataManager : MonoBehaviour
{
    //public static DataManager Instance { set; get; }
    //
    //private void Awake()
    //{
    //    Instance = this;
    //}

    public void WriteData(string dir, string csvPath, Dictionary<string, string> data)
    {
        // check if directory exist
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
           
        }
        // check if file exists, if not create with header
        if (!File.Exists(dir))
        {
            
            // create new csv file
            using (var ftw = new FileStream(csvPath, FileMode.Append))
            using (var sw = new StreamWriter(ftw))
            using (var wt = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture))
            {
                foreach (string key in data.Keys)
                {
                    wt.WriteField(key);
                }

                wt.NextRecord();

                // close all
                wt.Dispose();
                sw.Close();
                ftw.Close();
            }

            Debug.Log("File at path '" + csvPath + "' is created with headers.");
        }

        using (var ftw = new FileStream(csvPath, FileMode.Append))
        using (var sw = new StreamWriter(ftw))
        using (var wt = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture))
        {
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
}
