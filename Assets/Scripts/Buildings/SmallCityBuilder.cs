using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCityBuilder : MonoBehaviour {
    public Texture2D map;
    public float scale = 4;
    public BuildingTextureAtlas atlas;


    public Color building = Color.black;
    public Color road = Color.white;

    // Use this for initialization
    void Start () {
		
	}
	
    public void BuildCity(){
        Debug.Log("Reading map. " + map.width + " " + map.height);
        Color[] colors = map.GetPixels();
        for(int i = 0; i < map.width; i++)
        {
            for(int j = 0; j < map.height; j++)
            {
                if(colors[(i * map.width) + j].r == 0)
                {
                    Debug.Log("Building at " + i + " " + j);
                    CreateBuildingAt(i, j);

                }
            }
        }
        Debug.Log("Finished reading");
    }

    private void CreateBuildingAt(float x, float y)
    {
        List<Vector3> floorplan = MathUtility.InstructionsToPoints(
            new List<Vector3>()
            {
                new Vector3(x * scale, 0, y * scale), Vector3.right * scale, Vector3.forward * scale, Vector3.left * scale
            });
        GameObject building = BuildingCreator.CreateBuildingObject(floorplan, BuildingCreator.BuildingType.RESIDENTIAL, atlas);
        BoxCollider bc = building.AddComponent<BoxCollider>();
        Vector3 mid = MathUtility.MidPoint(floorplan);

        Bounds b = new Bounds();
        b.center = mid;
        foreach (Vector3 v in floorplan)
        {
            b.Encapsulate(v);
            b.Encapsulate(v + Vector3.up * 3);
            Debug.DrawLine(v, mid, Color.red, 30f);
        }
        bc.size = b.size;
        bc.center = b.center;

        building.transform.parent = transform;
    }
}
