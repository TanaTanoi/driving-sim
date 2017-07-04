using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCityBuilder : MonoBehaviour {
    public Texture2D map;
    public float scale = 12;
    public BuildingTextureAtlas atlas;

    public Material footpath;

    public Color building = Color.black;
    public Color road = Color.white;

    private Color[] pixels;

    public void BuildCity(){
        DestroyChildren();
        Debug.Log("Reading map. " + map.width + " " + map.height);
        pixels = map.GetPixels();
        for(int i = 0; i < map.width; i++)
        {
            for(int j = 0; j < map.height; j++)
            {
                if(ColorAt(i, j).r == 0)
                {
                    CreateBuildingAt(i, j, DirsForPixel(i, j));

                }
            }
        }
        Debug.Log("Finished reading");
    }

    private Color ColorAt(int x, int y) {
        return pixels[(x * map.width) + y];
    }

    private void CreateBuildingAt(float x, float y, Directions dirs)
    {
        List<Vector3> floorplan = MathUtility.InstructionsToPoints(
            new List<Vector3>()
            {
                new Vector3(x * scale, 0, y * scale), Vector3.right * scale, Vector3.forward * scale, Vector3.left * scale
            }, 1);

        for(int i = 0; i < floorplan.Count - 1; i ++) {
            Debug.DrawLine(floorplan[i], floorplan[i + 1], Color.black, 30f);
        }

        // Create building
        GameObject building = BuildingCreator.CreateBuildingObject(floorplan, BuildingCreator.BuildingType.RESIDENTIAL, atlas);
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

        // Create surrounding footpath
        List<Vector3> points = dirs.FloorplanForBuilding(x, y, scale, scale / 8f);
        points.Reverse();
        List<Vector3> loweredPoints = MeshCreator.TranslatePolygon(points, Vector3.down * 0.1f);

        Mesh m = MeshCreator.CreateMeshesBetweenPolygons(new List<List<Vector3>>() { loweredPoints, points });
        Mesh top = MeshCreator.FillPolygon(points);
        m = MeshCreator.CombineMeshes(new List<Mesh>() { m, top });
        GameObject block = new GameObject("Block");
        block.AddComponent<MeshFilter>().mesh = m;
        block.AddComponent<MeshRenderer>().material = footpath;
        block.transform.parent = building.transform;
    }

    private void DestroyChildren() {
        while (transform.childCount > 0) {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private Directions DirsForPixel(int x, int y) {
        // TODO change .r != 0 to something about non-building
        return new Directions(
                y + 1 < map.height && ColorAt(x, y + 1).r != 0,
                y - 1 > 0 && ColorAt(x, y - 1).r != 0,
                x - 1 > 0 && ColorAt(x - 1, y).r != 0,
                x + 1 < map.width && ColorAt(x + 1, y).r != 0
            );
    }

    struct Directions {
        public bool up, down, left, right;
        public Directions(bool u, bool d, bool l, bool r) {
            up = u; down = d; left = l; right = r;
        }

        public List<Vector3> FloorplanForBuilding(float x, float y, float scale, float width) {
            Vector3 start = new Vector3(x * scale, 0,  y * scale);
            start = start + (Vector3.back * (down ? 1 : 0) * width) + (Vector3.left * (left ? 1 : 0) * width);
            Vector3 next = start + Vector3.right * ((left ? 1 : 0) * width + (right ? 1 : 0) * width + scale);
            Vector3 next2 = next + Vector3.forward * ((down ? 1 : 0) * width + (up ? 1 : 0) * width + scale);
            Vector3 next3 = next2 + Vector3.left * ((left ? 1 : 0) * width + (right ? 1 : 0) * width + scale);
            return new List<Vector3>() {
                start, next, next2, next3
            };
        }
    }
}
