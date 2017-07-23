using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsController : MonoBehaviour {

    public const int OBSERVER_LAYER = 8;

    AnalyticModule[] modules;
    AnalyticsViewModule[] viewModules;
    TerminationModule[] termModules;

    private bool tracking = false;

    void Start() {
        modules = gameObject.GetComponents<AnalyticModule>();
        viewModules = gameObject.GetComponents<AnalyticsViewModule>();
        termModules = gameObject.GetComponents<TerminationModule>();
    }

    public void StartTracking() {
        tracking = true;
        foreach(AnalyticModule mod in modules) {
            mod.StartTracking();
        }
        foreach(AnalyticsViewModule vmod in viewModules) {
            vmod.StartDisplay();
        }
    }

    public void SetupTracking() {
         foreach (TerminationModule tmod in termModules) {
            tmod.Setup();
        }
    }

     public void StopTracking() {
        tracking = false;
        foreach(AnalyticModule mod in modules) {
            mod.StopTracking();
        }
        foreach(AnalyticsViewModule vmod in viewModules) {
            vmod.StopDisplay();
        }
    }

    public void ClearTracking() {
        foreach(AnalyticsViewModule vmod in viewModules) {
            vmod.ClearDisplay();
        }
    }

    public bool Tracking() {
        return tracking;
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

    public string[] Data() {
        string[] data = new string[modules.Length];
        for(int i = 0; i < modules.Length; i++) {
            data[i] = modules[i].AnalyticData();
        }
        return data;
    }

    void Update() {
        //foreach(AnalyticModule mod in modules) {
        //    Debug.Log(mod.AnalyticName() + " " + mod.AnalyticValue());
        //}
    }
}
