using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Streams data to the observer menu
[RequireComponent(typeof(AnalyticsController))]
public class DataStreamModule : AnalyticsViewModule {

    // If this is null, it will not stream
    private ObserverUI ui;
    // Where data is sourced
    private AnalyticsController controller;

    void Start() {
        controller = GetComponent<AnalyticsController>();
    }

    void FixedUpdate() {
        if(ui != null) {
            StringBuilder sb = new StringBuilder();
            string[] names = controller.Names();
            string[] values = controller.Values();
            for(int i = 0; i < names.Length; i++) {
                sb.AppendLine(String.Format("{0, -20}{1, 10}", names[i], values[i]));
            }
            ui.SetConsoleText(sb.ToString());
        }
    }

    public override void ClearDisplay() {
        if(ui != null) {
            ui.SetConsoleText("");
        }
    }

    public override void StartDisplay() {
        ClearDisplay();
        ui = FindObjectOfType<ObserverUI>();
    }

    public override void StopDisplay() {
        ui = null;
    }
}
