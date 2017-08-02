using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HeadTurnsModule : AnalyticModule {

    public float turnThreshold = 0.3f;

    // time for checking turns
    public float pollTime = 0.1f;

    private Transform headset;
    private float previousTime;

    private bool turning = false;
    private bool tracking = false;

    private float startTime;

    private List<float> turnTimes;
    private List<float> turnDurations;
	
	// Update is called once per frame
	void FixedUpdate () {
        if (tracking) {
            float currTime = Time.time - startTime;
            if (currTime - previousTime > pollTime) {
                previousTime = currTime;
                Vector2 dir = DirectionFromVec3(headset.forward);
                if (!turning && HasTurned(dir)) {
                    turning = true;
                    turnTimes.Add(currTime);
                } else if (turning && !HasTurned(dir)) {
                    AddTurnDuration(currTime);
                    turning = false;
                }
            }
        }
    }

    public void SetHeadset(Transform h){
        headset = h;
    }

    public override void StartTracking(){
        tracking = true;
        turning = false;
        turnTimes = new List<float>();
        turnDurations = new List<float>();
        startTime = Time.time;
        previousTime = Time.time - startTime;
    }

    private void AddTurnDuration(float currentTime) {
        turnDurations.Add(currentTime - turnTimes[turnTimes.Count - 1]);
    }

    public override void StopTracking(){
        tracking = false;
        if(turnTimes.Count != turnDurations.Count) {
            AddTurnDuration(Time.time - startTime);
        }
    }

    public override string AnalyticName(){
        return "Headset Turns";
    }

    public override string AnalyticValue(){
        return turnTimes.Count.ToString();
    }

    public override string AnalyticData(){
        // Outputs when an item was collected
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Head turns over time:");
        sb.AppendLine("ID" + DATA_SEPERATOR + "TIME" + DATA_SEPERATOR + "DURATION");

        for (int i = 0; i < turnTimes.Count; i++)
        {
            sb.AppendLine(i + DATA_SEPERATOR + turnTimes[i] + DATA_SEPERATOR + turnDurations[i]);
        }

        return sb.ToString();
    }


    private Vector2 DirectionFromVec3(Vector3 input){
        return new Vector2(input.x, input.z).normalized;
    }

    private bool HasTurned(Vector2 dir){

        Vector2 parentDir = DirectionFromVec3(headset.parent.forward);

        float dot = Vector2.Dot(dir, parentDir);
        return dot < turnThreshold;
    }
}
