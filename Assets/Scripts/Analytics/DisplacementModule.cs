﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DisplacementModule : AnalyticModule {
    public GameObject target;

    private List<Vector3> positions;

    private bool tracking = false;
    // Time between taking positions
    private const float TIME_DELTA = 0.2f;
    private float distanceTravelled = 0;
    private float time = 0;

	// Use this for initialization
	void Start () {
		if(target == null) {
            throw new System.Exception("This module requires a target!");
        }
        positions = new List<Vector3>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(tracking) {
            Vector3 current = target.transform.position;
            Vector3 last = positions[positions.Count - 1];
            float distance = (current - last).magnitude;
            if(Time.time - time > TIME_DELTA) {
                distanceTravelled += distance;
                positions.Add(target.transform.position);
                time += TIME_DELTA;
            }
        }
	}
    
    void Update() {
        for(int i = 0; i < positions.Count - 1; i++) {
            Debug.DrawLine(positions[i], positions[i + 1], Color.red);
        }
    }

    public override void StartTracking() {
        tracking = true;
        positions.Clear();
        positions.Add(target.transform.position);
        time = Time.time;
    }

    public override void StopTracking() {
        tracking = false;
    }

    public override string AnalyticName() {
        return "Distance Travelled";
    }

    public override string AnalyticValue() {
        return distanceTravelled.ToString();
    }

    public override string AnalyticData() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Displacment Over Time");
        float currentTotalDistance = 0;
        Vector3 previous = positions[0];
        for(int i = 0; i < positions.Count; i++) {
            Vector3 current = positions[i];
            float dist = (current - previous).magnitude;
            currentTotalDistance += dist;
            previous = current;

            // Entry : Time : Distance : x : Y : Z : 
            sb.AppendLine(i + " " + (i * TIME_DELTA) + " " + currentTotalDistance + " " + current.x + " " + current.y + " " + current.z);
        }
        return sb.ToString();
    }
}
