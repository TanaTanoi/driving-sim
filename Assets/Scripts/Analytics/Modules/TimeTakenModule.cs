using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTakenModule : AnalyticModule {

    private float startTime = 0;

    public override void StartTracking() {
        startTime = Time.unscaledTime;
    }

    public float CurrentTime() {
        return Time.unscaledTime - startTime;
    }

    public override void StopTracking() {
        // Nothing to stop tracking
    }

    public override string AnalyticName() {
        return "Time Taken";
    }

    public override string AnalyticValue() {
        return Mathf.Round(CurrentTime()).ToString();
    }

    public override string AnalyticData() {
        return AnalyticName() + " : " + AnalyticValue();
    }
}
