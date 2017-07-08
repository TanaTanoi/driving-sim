using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsController : MonoBehaviour {

    AnalyticModule[] modules;

    void Start() {
        modules = gameObject.GetComponents<AnalyticModule>();
        StartTracking();
    }

    public void StartTracking() {
        foreach(AnalyticModule mod in modules) {
            mod.StartTracking();
        }
    }

     public void StopTracking() {
        foreach(AnalyticModule mod in modules) {
            mod.StopTracking();
        }
    }

    public string[] Names() {
        string[] names = new string[modules.Length];
        for(int i = 0; i < modules.Length; i++) {
            names[i] = modules[i].AnalyticName();
        }
        return names;
    }

    public string[] Values() {
        string[] values = new string[modules.Length];
        for(int i = 0; i < modules.Length; i++) {
            values[i] = modules[i].AnalyticValue();
        }
        return values;
    }

    void Update() {
        foreach(AnalyticModule mod in modules) {
            Debug.Log(mod.AnalyticName() + " " + mod.AnalyticValue());
        }
    }
}
