﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorplanDebug : MonoBehaviour {
    
    public Material debugStructMat;
    public Material debugDetailMat;
    public Material debugDoorMat;

    // Use this for initialization
    void Start() {
        List<Vector2> LShape = MathUtility.InstructionsToPoints(
            new List<Vector2> {
                Vector2.zero, Vector2.right * 2, Vector2.up * 2, Vector2.left, Vector2.down, Vector2.left
            });
        List<Vector2> oct = MathUtility.InstructionsToPoints(
        new List<Vector2> {
            Vector2.zero, Vector2.right, Vector2.up + Vector2.right, Vector2.up, Vector2.up + Vector2.left, Vector2.left, Vector2.left + Vector2.down, Vector2.down
        });
        List<Vector2> thang = MathUtility.InstructionsToPoints(
        new List<Vector2> {
            Vector2.zero, Vector2.right * 16, Vector2.up * 4 + Vector2.left * 4, Vector2.left * 8
        });
        List<Vector2> test = MathUtility.InstructionsToPoints(
        new List<Vector2> {
            Vector2.zero, Vector2.down * 4 + Vector2.right * 6, Vector2.right * 4, Vector2.up * 4 + Vector2.left * 2, Vector2.left * 4 + Vector2.up * 2
        });
        List<Vector2> H = MathUtility.InstructionsToPoints(
        new List<Vector2> {
            Vector2.zero * 2, Vector2.right * 2, Vector2.up * 2, Vector2.right * 2, Vector2.down * 2, Vector2.right * 2, Vector2.up * 6, Vector2.left * 2, Vector2.down * 2, Vector2.left * 2,
            Vector2.up * 2, Vector2.left  * 2
        });
        //for (int i = 0; i < thang.Count; i++) {
        //    Debug.DrawLine(thang[i], thang[(i + 1) % thang.Count], Color.black, 300f);
        //}
        List<Vector2> points = FloorplanManipulator.CreateSquareMesh(test);
        //for (int i = 0; i < points.Count; i++) {
        //    Debug.DrawLine(points[i], points[(i + 1) % points.Count], Color.red, 300f);
        //}
        List<Vector3> floorplan = MathUtility.ConvertTo3D(H);
        for (int i = 0; i < floorplan.Count - 1; i++) {
            Debug.DrawLine(floorplan[i], floorplan[(i + 1) % floorplan.Count], Color.red, 300f);
        }
        floorplan.Reverse();
        MeshCreator.WallMesh wallmesh = BuildingCreator.CreateBuilding(floorplan, BuildingCreator.BuildingType.RESIDENTIAL);
        
        //GameObject buildingObject = MeshCreator.AssignMeshesToGameObject(
        //    new List<Mesh>() { wallmesh.detailMesh, wallmesh.structuralMesh, wallmesh.doorMesh },
        //    new List<Material>() { debugDetailMat, debugStructMat, debugDoorMat }
        //    );
        //buildingObject.transform.SetParent(gameObject.transform);
        //Vector2 point = Vector2.up * 0.25f + Vector2.right * 1.75f;
        //Debug.Log(MathUtility.PointWithinPolygon(point, LShape));
        //Debug.DrawLine(Vector2.zero, point, Color.black, 300f);
    }

    IEnumerator code() {
        List<Vector2> LShape = MathUtility.InstructionsToPoints(
            new List<Vector2> {
                Vector2.zero, Vector2.right * 2, Vector2.up * 2, Vector2.left, Vector2.down, Vector2.left
            });
        HashSet<Vector2> points = FloorplanManipulator.GridPointsWithinPolygon(LShape);

        for (int i = 0; i < LShape.Count; i++) {
            Debug.DrawLine(LShape[i], LShape[(i + 1) % LShape.Count], Color.black, 300f);
        }
        foreach (Vector2 p in points) {
            Debug.DrawLine(p, p + Vector2.up * 0.1f, Color.red, 300f);
        }
        yield return null;
    }
}