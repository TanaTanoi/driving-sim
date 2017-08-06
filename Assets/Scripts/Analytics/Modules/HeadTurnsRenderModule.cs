using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeadTurnsModule))]
public class HeadTurnsRenderModule : AnalyticsViewModule
{

    public float arrowSize = 1;

    private GameObject arrowSegments;
    private int currentSegments;
    private HeadTurnsModule headturnmod;

    private static Vector3 OFFSET = Vector3.up * 0.5f;

    void FixedUpdate(){
        if (headturnmod.Tracking && headturnmod.TurnDirections.Count > currentSegments){
            // add latest segment
            bool latestDir = headturnmod.TurnDirections[headturnmod.TurnDirections.Count - 1];
            currentSegments++;
        }
    }


    private void AddArrowSegment(){

    }

    public override void StartDisplay() {
        headturnmod = GetComponent<HeadTurnsModule>();
        currentSegments = 0;
    }

    public override void StopDisplay(){
    }

    public override void ClearDisplay(){
        Destroy(arrowSegments);
        Setup();
    }

    // Clears the information between displays
    private void Setup(){
        arrowSegments = new GameObject("Line Segments");
        arrowSegments.transform.parent = transform;
        currentSegments = 0;
    }
}
