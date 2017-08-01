using System;
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
        positions = new List<Vector3>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(tracking) {
            if(target == null) {
                throw new System.Exception("This module requires a target!");
            }
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
        if(tracking) {
            for(int i = 0; i < positions.Count - 1; i++) {
                Debug.DrawLine(positions[i], positions[i + 1], Color.red);
            }
        }
    }

    public void SetTarget(GameObject target) {
        this.target = target;
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
        return "Distance Covered";
    }

    public override string AnalyticValue() {
        return Mathf.Round(distanceTravelled).ToString();
    }

    public override string AnalyticData() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Displacement Over Time:");
        sb.AppendLine("ID" + DATA_SEPERATOR + "TIME" + DATA_SEPERATOR + "DISPLACEMENT" +DATA_SEPERATOR +"X" + DATA_SEPERATOR+ "Z");
        float currentTotalDistance = 0;
        Vector3 previous = positions[0];
        for(int i = 0; i < positions.Count; i++) {
            Vector3 current = positions[i];
            float dist = (current - previous).magnitude;
            currentTotalDistance += dist;
            previous = current;

            // Entry : Time : Distance : X : Z : 
            sb.AppendLine(
                i + DATA_SEPERATOR + (i * TIME_DELTA) + DATA_SEPERATOR + Mathf.Round(currentTotalDistance) + 
                DATA_SEPERATOR + Mathf.Round(current.x) + DATA_SEPERATOR + Mathf.Round(current.z)
                );
        }
        return sb.ToString();
    }

    public List<Vector3> GetPositions() {
        return positions;
    }                        
    public int PositionCount() {
        return positions.Count;
    }           
}
