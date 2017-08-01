using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour {


    public BuildingTextureAtlas atlas;

    private List<List<Vector3>> floorplans = new List<List<Vector3>>() {
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(1,0,2), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(1,0,4), Vector3.right, Vector3.forward, Vector3.left
        }),
        /* Top Left building group */
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(1,0,7), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(1,0,8), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(3,0,8), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(3,0,6), Vector3.right, Vector3.forward * 2, Vector3.left
        }),
        //
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(1,0,6), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        /* Center Bottom building group */
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(3,0,2), Vector3.right, Vector3.forward * 3, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(4,0,2), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(4,0,4), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(5,0,3), Vector3.right, Vector3.forward * 2, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(5,0,6), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(5,0,8), Vector3.right, Vector3.forward, Vector3.left
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(7,0,2), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        /* Right building block */
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(7,0,4), Vector3.right * 2, Vector3.forward * 2, Vector3.left * 2
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(7,0,6), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(7,0,7), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(7,0,8), Vector3.right * 2, Vector3.forward, Vector3.left * 2
        }),
        // Left border
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(0,0,1), Vector3.forward * 9, Vector3.left * 1, Vector3.back * 9
        }),
        // Bottom border
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(0,0,1), Vector3.back, Vector3.right * 10, Vector3.forward
        }),
        // Top border
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(0,0,10), Vector3.right * 10, Vector3.forward, Vector3.left * 10
        }),
        // Right border
        MathUtility.InstructionsToPoints(new List<Vector3> {
            new Vector3(10,0,1), Vector3.right, Vector3.forward * 9, Vector3.left
        }),
    };


    public void Build() {
        if(transform.childCount == 0) {
            // Make new buildings if none are present
            foreach(List<Vector3> floorplan in floorplans) {
                GameObject building = BuildingCreator.CreateBuildingObject(floorplan, BuildingCreator.BuildingType.RESIDENTIAL, atlas, new List<int>());
                BoxCollider bc = building.AddComponent<BoxCollider>();
                Vector3 mid = MathUtility.MidPoint(floorplan);
                
                Bounds b = new Bounds();
                b.center = mid;
                foreach (Vector3 v in floorplan) {
                    b.Encapsulate(v);
                    b.Encapsulate(v + Vector3.up * 3);
                    Debug.DrawLine(v, mid, Color.red, 30f);
                }
                bc.size = b.size;
                bc.center = b.center;

                building.transform.parent = transform;
            }
        } else {
            // Regenerate if buildings are present
            for(int i = 0; i < transform.childCount;i++) {
                GameObject o = transform.GetChild(i).gameObject;
                Building building = o.GetComponent<Building>();
                if (building != null) {
                    building.Build();
                }
            }
        }
    }
}
