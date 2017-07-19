using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DisplacementModule))]
[RequireComponent(typeof(TimeTakenModule))]
public class DisplacementRenderModule : AnalyticsViewModule {
    
    public float lineWidth = 0.725f;
    public Material lineMaterial;
    [Tooltip("Each colour represents the percentage along the time limit this is.")]
    public Color[] colors;
    [Tooltip("This uses the TimeLimitModule if <= 0")]
    public float timeLimit = 0;

    private DisplacementModule displacementMod;
    private TimeTakenModule timeMod;
    private GameObject lineSegments; // Used to hold all the line segment meshes
    private int currentSegments = 0;

    private bool trackSegments = false;

	// Use this for initialization
	void Start () {
        displacementMod = GetComponent<DisplacementModule>();
        timeMod = GetComponent<TimeTakenModule>();
        Setup();

        TimeLimitModule timeLimitMod = GetComponent<TimeLimitModule>();
        if(timeLimitMod != null && timeLimit <= 0) {
            timeLimit = timeLimitMod.timelimit;
        }
	}
	
	void FixedUpdate () {
        int posCount = displacementMod.PositionCount();

        // If we have fewer segments than there are points, add a line segment
        if(trackSegments && currentSegments <= posCount && posCount > 2) {
            List<Vector3> positions = displacementMod.GetPositions();

            // Add the last two points as a segment
            AddLineSegment(positions[posCount - 1], positions[posCount - 2]);
            currentSegments++;
        }
		
	}

    private Color ColorAtTime() {

        float progress = timeMod.CurrentTime() / timeLimit;
        int lowerColour = Mathf.FloorToInt(progress * colors.Length);
        Color a = colors[lowerColour  % colors.Length];
        Color b = colors[Mathf.CeilToInt(progress * colors.Length) % colors.Length];

        return Color.Lerp(a, b, (progress * colors.Length) - lowerColour);
    }

    private void AddLineSegment(Vector3 from, Vector3 to) {
        Mesh lineMesh = new Mesh();
        Vector3 bisec = (to - from).normalized;
        bisec = new Vector3(bisec.z, 0, -bisec.x);

        Vector3 a = to + bisec * lineWidth;
        Vector3 b = to - bisec * lineWidth;
        Vector3 c = from + bisec * lineWidth;
        Vector3 d = from - bisec * lineWidth;

        Vector3[] verts = new Vector3[] { a, b, c, d };
        int[] tris = new int[] {
            3, 1, 0,
            3, 0, 2
        };

        lineMesh.vertices = verts;
        lineMesh.triangles = tris;

        GameObject lineSegment = new GameObject("LineSegment");
        lineSegment.AddComponent<MeshFilter>().mesh = lineMesh;
        MeshRenderer mr = lineSegment.AddComponent<MeshRenderer>();
        mr.material = lineMaterial;

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

        mr.GetPropertyBlock(propBlock);

        propBlock.SetColor("_Color", ColorAtTime());

        mr.SetPropertyBlock(propBlock);
        lineSegment.transform.parent = lineSegments.transform;
        lineSegment.layer = AnalyticsController.OBSERVER_LAYER;
    }

    public override void StartDisplay() {
        trackSegments = true;
    }

    public override void StopDisplay() {
        trackSegments = false;
    }

    public override void ClearDisplay() {
        Destroy(lineSegments);
        Setup();
    }

    // Clears the information between displays
    private void Setup() {
        lineSegments = new GameObject("Line Segments");
        lineSegments.transform.parent = transform;
        currentSegments = 0;
    }
}
