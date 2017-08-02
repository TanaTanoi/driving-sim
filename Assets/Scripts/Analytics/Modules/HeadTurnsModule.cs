using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class HeadTurnsModule : AnalyticModule {

    public float turnThreshold = 0.3f;

    // time for checking turns
    public float pollTime = 0.1f;

    private Transform headset;
    private float previousTime;

    private bool turning = false;

    private List<float> turnTimes;
    private List<float> turnDurations;
    private List<bool> turnDirection; // false for left, true for right

    protected override void Track() {
        float currTime = Time.time - StartTime;
        if (currTime - previousTime > pollTime) {
            previousTime = currTime;
            Vector2 dir = DirectionFromVec3(headset.forward);
            if (!turning && HasTurned(dir)) {
                turning = true;
                turnTimes.Add(currTime);
                turnDirection.Add(TurnDirection(dir));
            } else if (turning && !HasTurned(dir)) {
                AddTurnDuration(currTime);
                turning = false;
            }
        }
    }

    public void SetHeadset(Transform h){
        headset = h;
    }

    protected override void EnableTracking(){
        tracking = true;
        turning = false;
        turnTimes = new List<float>();
        turnDurations = new List<float>();
        turnDirection = new List<bool>();
        previousTime = Time.time - StartTime;
    }

    private void AddTurnDuration(float currentTime) {
        turnDurations.Add(currentTime - turnTimes[turnTimes.Count - 1]);
    }

    public override void StopTracking(){
        tracking = false;
        if(turnTimes.Count != turnDurations.Count) {
            AddTurnDuration(Time.time - StartTime);
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


        sb.AppendLine("ID" + DATA_SEPERATOR + "TIME" + DATA_SEPERATOR + "DURATION" + DATA_SEPERATOR + "DIRECTION");

        for (int i = 0; i < turnTimes.Count; i++) {
            string direction = turnDirection[i] ? "RIGHT" : "LEFT";
            sb.AppendLine(i + DATA_SEPERATOR + turnTimes[i] + DATA_SEPERATOR + turnDurations[i] + DATA_SEPERATOR + direction);
        }

        return sb.ToString();
    }


    private Vector2 DirectionFromVec3(Vector3 input){
        return new Vector2(input.x, input.z).normalized;
    }

    // Returns false for left, true for right
    private bool TurnDirection(Vector2 dir) {
        Vector2 parentRight = DirectionFromVec3(headset.parent.right);
        float dot = Vector2.Dot(dir, parentRight);
        return dot > 0;
    }

    private bool HasTurned(Vector2 dir){

        Vector2 parentDir = DirectionFromVec3(headset.parent.forward);

        float dot = Vector2.Dot(dir, parentDir);
        return dot < turnThreshold;
    }
}
