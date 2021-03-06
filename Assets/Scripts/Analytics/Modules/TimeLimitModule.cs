﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TimeTakenModule))]
public class TimeLimitModule : TerminationModule {

    public TimeTakenModule timetaken;
    public float timelimit = 200;

    public override string TerminationReason {
        get {
            return "Time limit (" + timelimit + " seconds) exceeded.";
        }
    }

    // Use this for initialization
    public new void Start () {
        base.Start();

        timetaken = GetComponent<TimeTakenModule>();
	}
	
    public override bool TestOver() {
        return float.Parse(timetaken.AnalyticValue()) > timelimit;
    }

    public override void Setup() {
        // No setup requried
    }
}
