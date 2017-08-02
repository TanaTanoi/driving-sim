using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Building : MonoBehaviour {
    public BuildingCreator.BuildingType type;
    public Material primaryStructuralMaterial;
    public Material secondaryStructuralMaterial;
    public Material roofMaterial;
    public Material windowMaterial;
    public Material doorMaterial;

    public List<Vector3> floorplan;
    public List<int> validFaces;
    public void SetParams(List<Vector3> fp, BuildingCreator.BuildingType bt, List<Material> mats, List<int> vf) {
        floorplan = fp;
        fp.Reverse();
        type = bt;
        primaryStructuralMaterial = mats[0];
        secondaryStructuralMaterial = mats[1];
        roofMaterial = mats[2];
        windowMaterial = mats[3];
        doorMaterial = mats[4];
        validFaces = vf;
    }

    public void Build() {
        while (transform.childCount > 0) {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
        StringBuilder sb = new StringBuilder();
        foreach(Vector3 v in floorplan) {
            sb.Append("new Vector3(" + v.x + "f, " + v.y + ", " + v.z + "f),");
        }
        //Debug.Log(sb.ToString());
        try {
            MeshCreator.WallMesh wallmesh = BuildingCreator.CreateBuilding(floorplan, type, validFaces);
            //wallmesh.CombineMeshes(2, 3)
            MeshCreator.AssignMeshesToGameObjects(wallmesh.meshes, Materials(), gameObject);
        } catch(System.Exception e) {
            Debug.Log("No building here :" + floorplan[0] + " "  + e);
        }
    }

    private List<Material> Materials() {
        return new List<Material> {
            primaryStructuralMaterial,
            secondaryStructuralMaterial,
            roofMaterial,
            windowMaterial,
            doorMaterial
        };
    }
}
