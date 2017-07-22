using System;
using System.Collections;
using System.Collections.Generic;
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
            ui.SetConsoleText(controller.Data()[0]);
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
