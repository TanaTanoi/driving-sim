using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResultWriter {

    private const string RESULTS_PATH = "results\\";

    private string foldername;
    // name : data
    private Dictionary<string, string> datasets;


    public ResultWriter(string name, string context, string layout) {
        foldername = FilenameFor(name, context, layout);
        datasets = new Dictionary<string, string>();
    }

    public void AddData(string name, string dataset) {
        datasets[name] = dataset;
    }

    // Returns the full filename for a given name in the format:
    //  results_name_yyyy_MM_dd_HH_mm.txt 
    private string FilenameFor(string name, string context = "", string layout = "") {
        context += "_";
        layout += "_";
        if(name.Length == 0) {
            name = "unlabelled";
        }
        name = name.Replace(' ', '_');
        return name + "_" + context + layout + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
    }

    public bool WriteData() {
        string filepath = RESULTS_PATH + foldername;
        Directory.CreateDirectory(filepath);
        foreach(KeyValuePair<string, string> data in datasets) {
            string filename = filepath + "\\" + data.Key + ".csv";
            if(!WriteToFile(filename, data.Value)){
                return false;
            }
        }

        GameObject.FindObjectOfType<TestManager>().TakeScreenshotOfMap(filepath + "\\map.png");

        return true;
    }


    
    // Writes contents to a file. Returns false if it failed
    private bool WriteToFile(string filename, string content) {
        try {
            System.IO.File.WriteAllText(filename, content);
        } catch( System.IO.IOException e) {
            Debug.Log("Error - Could not write: " + e);
            return false;
        }
        return true;
    }

}
