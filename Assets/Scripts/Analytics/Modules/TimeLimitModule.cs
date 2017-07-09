using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimitModule : TerminationModule {

    public TimeTakenModule timetaken;
    public float timelimit = 200;

    // Use this for initialization
    public new void Start () {
        base.Start();

        if(timetaken == null) {
            timetaken = GetComponent<TimeTakenModule>();
            if(timetaken == null) {
                throw new Exception("TimeLimitModule requires a TimeTakenModule component!");
            }
        }
	}
	
    public override bool TestOver() {
        return float.Parse(timetaken.AnalyticValue()) > timelimit;
    }
}
