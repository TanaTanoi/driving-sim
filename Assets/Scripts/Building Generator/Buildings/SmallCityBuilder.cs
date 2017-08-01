using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCityBuilder : MonoBehaviour {
    public Texture2D map;
    public float scale = 12;
    public BuildingTextureAtlas atlas;

    public Material footpath;
    public GameObject item;

    public Color buildingColour = Color.black;
    public Color roadColour = Color.white;
    public Color itemColour = Color.green;
    public Color startColour = Color.blue;
    public Color noDrawColor = new Color(1, 1, 0);

    private Color[] pixels;
    private Vector3 startPosition;

    public void BuildCity(){
        DestroyChildren();
        Debug.Log("Reading map. " + map.width + " " + map.height);
        pixels = map.GetPixels();

        GameObject buildings = new GameObject("Buildings");
        buildings.transform.parent = transform;
        GameObject items = new GameObject("Items");
        items.transform.parent = transform;

        for(int i = 0; i < map.width; i++){
            for(int j = 0; j < map.height; j++){
                if(ColorAt(i, j) == buildingColour){
                    CreateBuildingAt(i, j, DirsForPixel(i, j), buildings.transform);
                } else if(ColorAt(i, j) == itemColour) {
                    PlaceItem(items.transform, i, j);
                } else if(ColorAt(i, j) == startColour) {
                    // The middle of this tile
                    startPosition = new Vector3(((i + 0.5f) * scale), 0, ((j + 0.5f) * scale));
                }
            }
        }
        Debug.Log("Finished reading");
    }

    public Vector3 GetStartPosition() {
        if(startPosition == Vector3.zero) {
            startPosition = FindStartPosition();
        }
        Debug.Log("Start pos " + startPosition);
        return startPosition;
    }

    private Vector3 FindStartPosition() {
        for(int i = 0; i < map.width; i++){
            for(int j = 0; j < map.height; j++){
                if(ColorAt(i, j) == startColour) {
                    return new Vector3(((i + 0.5f) * scale), 0, ((j + 0.5f) * scale));
                }
            }
        }
        throw new System.Exception("No start position on this layout! Please use a different one.");
    }

    private void PlaceItem(Transform  parent, int x, int y) {
        GameObject newItem = Instantiate(item);
        newItem.transform.position = new Vector3(x * scale + scale / 2f, 1, y * scale + scale / 2f);
        newItem.transform.parent = parent;

        // Allign the arcs of the item
        // If horizontal buildings
        if(ColorAt(x - 1, y) == buildingColour && ColorAt(x + 1, y) == buildingColour) {
            // Do nothing, they are correct by default
        // If vertical buildings
        } else if (ColorAt(x, y - 1) == buildingColour && ColorAt(x, y + 1) == buildingColour){
            newItem.transform.Rotate(Vector3.up, 90);
        }
    }

      private Color ColorAt(float x, float y) {
        return ColorAt((int)x, (int)y);
    }
    private Color ColorAt(int x, int y) {
        return pixels[(x * map.width) + y];
    }

    private void CreateBuildingAt(float x, float y, Directions dirs, Transform parent) {

        // relative to image: right = up, forward = right
        // 3 is left, 0 is up, 1 is right, 2 is down (id to image dir)
        List<Vector3> floorplan = MathUtility.InstructionsToPoints(
            new List<Vector3>()
            {
                new Vector3(x * scale, 0, y * scale), Vector3.right * scale, Vector3.forward * scale, Vector3.left * scale
            }, 1);

        List<int> validFaces = new List<int> { 0, 1, 2, 3 };
        if (x + 1 >= map.width || ColorAt(x + 1, y) == noDrawColor || ColorAt(x + 1, y) == buildingColour) {
            // right
            validFaces.Remove(1);
        }

        if (x - 1 < 0 || ColorAt(x - 1, y) == noDrawColor || ColorAt(x - 1, y) == buildingColour) {
            // left
            validFaces.Remove(3);
        }

        if (y + 1 >= map.height || ColorAt(x, y + 1) == noDrawColor || ColorAt(x, y + 1) == buildingColour) {
            // up
            validFaces.Remove(0);
        }

        if (y - 1 < 0 || ColorAt(x, y - 1) == noDrawColor || ColorAt(x, y - 1) == buildingColour) {
            //down 
            validFaces.Remove(2);
        }
        // Create building
        GameObject building = BuildingCreator.CreateBuildingObject(floorplan, BuildingCreator.BuildingType.RESIDENTIAL, atlas, validFaces);
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

        building.transform.parent = parent;

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
                y + 1 < map.height && ColorAt(x, y + 1) != buildingColour,
                y - 1 > 0 && ColorAt(x, y - 1) != buildingColour,
                x - 1 > 0 && ColorAt(x - 1, y) != buildingColour,
                x + 1 < map.width && ColorAt(x + 1, y) != buildingColour
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
