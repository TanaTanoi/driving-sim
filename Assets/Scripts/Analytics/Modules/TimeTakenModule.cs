using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTakenModule : AnalyticModule {

    private float finalTime;

    protected override void EnableTracking() {} // No requirements

    public float CurrentTime() {
        if(tracking) {
            return Time.time - StartTime;
        } else {
            return finalTime;
        }
    }

    public override void DisableTracking() {
        finalTime = Time.time - StartTime;
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

    protected override void Track() {}// Requires no active tracking
}
