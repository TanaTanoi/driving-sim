using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTakenModule : AnalyticModule {

    protected override void EnableTracking() {
    }

    public float CurrentTime() {
        return Time.time - StartTime;
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
