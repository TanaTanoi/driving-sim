using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CityGenerator;

public class MeshCreator {

    private const char EXTRUDE = 'E';
    private const char SHRINK = 'S';

    public struct WallMesh {
        public List<Mesh> meshes;
        public WallMesh(int count) {
            meshes = new List<Mesh>();
            for(int i = 0; i < count; i++) {
                meshes.Add(new Mesh());
            }
        }
        public void SetMesh(Mesh m, int index) {
            meshes[index] = m;
        }

        public Mesh GetMesh(int index) {
            return meshes[index];
        }

        public void RecalculateNormals() {
            foreach(Mesh m in meshes) {
                m.RecalculateNormals();
            }
        }

        public int Count() {
            return meshes.Count;
        }
    }

    public struct RawMesh {
        public List<List<Vector3>> vertsList;
        public List<List<int>> trisList;
        public List<List<Vector2>> uvsList;

        public RawMesh(int count) {
            vertsList = new List<List<Vector3>>();
            trisList = new List<List<int>>();
            uvsList = new List<List<Vector2>>();

            for(int i = 0; i < count; i++) {
                vertsList.Add(new List<Vector3>());
                trisList.Add(new List<int>());
                uvsList.Add(new List<Vector2>());
            }
        }

        public RawMesh(List<List<Vector3>> vl, List<List<int>> tl, List<List<Vector2>> ul) {
            vertsList = vl;
            trisList = tl;
            uvsList = ul;
        }

        public Mesh CreateMesh() {
            Vector3[] verts = vertsList.SelectMany(x => x).ToArray();
            Vector2[] uvs = uvsList.SelectMany(x => x).ToArray();

            Mesh mesh = new Mesh();
            mesh.subMeshCount = 5;
            mesh.vertices = verts;
            mesh.uv = uvs;
            for(int i = 0; i < trisList.Count; i++) {
                mesh.SetTriangles(trisList[i], i);
            }
            return mesh;
        }
    }

    // Creates a wall mesh with no door
    public static WallMesh CreateMeshForWall(List<IWallComponent> components, Vector3 startPoint, Vector3 direction, float totalWidth, float height) {
        return CreateMeshForWall(components, null, startPoint, direction, totalWidth, height);
    }

    /* Creates a wall with the specified components.
     * If a door is present, it overrides a random component with a door.
     */
    public static WallMesh CreateMeshForWall(List<IWallComponent> components, IWallComponent doorComponent, Vector3 startPoint, Vector3 direction, float totalWidth, float height) {
        RawMesh mesh = new RawMesh(5);

        int numberOfComponents = (int)Mathf.Ceil(totalWidth / height);
        numberOfComponents += 1 - (numberOfComponents % 2); // Ensures always odd
        float componentWidth = totalWidth / numberOfComponents;

        int doorIndex = -1;
        if (doorComponent != null) {
            doorIndex = Random.Range(0, numberOfComponents);
        }

        // Ensures the middle of the template is always the middle of the wall
        // Start index in the template array
        int startIndex = (components.Count / 2) - (numberOfComponents / 2);
        if(startIndex < 0) {
            startIndex = components.Count + (startIndex % -components.Count);
        }

        for(int i = 0; i < numberOfComponents; i++) {

            IWallComponent wall;
            if(i == doorIndex) {
                wall = doorComponent;
            } else { 
                wall = components[(startIndex + i) % components.Count];
            }
            wall.CreateMesh(startPoint + (direction * i * componentWidth), direction, componentWidth, height, mesh.vertsList, mesh.trisList, mesh.uvsList);
        }

        WallMesh wallmesh = new WallMesh(5);

        Mesh structuralMesh = new Mesh();
        structuralMesh.vertices = mesh.vertsList[IWallComponent.STRUCTURAL_LIST_ID].ToArray();
        structuralMesh.triangles = mesh.trisList[IWallComponent.STRUCTURAL_LIST_ID].ToArray();
        structuralMesh.uv = mesh.uvsList[IWallComponent.STRUCTURAL_LIST_ID].ToArray();
        wallmesh.SetMesh(structuralMesh, IWallComponent.STRUCTURAL_LIST_ID);

        Mesh accentMesh = new Mesh();
        accentMesh.vertices = mesh.vertsList[IWallComponent.STRUCTURAL_ACCENT_LIST_ID].ToArray();
        accentMesh.triangles = mesh.trisList[IWallComponent.STRUCTURAL_ACCENT_LIST_ID].ToArray();
        accentMesh.uv = mesh.uvsList[IWallComponent.STRUCTURAL_ACCENT_LIST_ID].ToArray();
        wallmesh.SetMesh(accentMesh, IWallComponent.STRUCTURAL_ACCENT_LIST_ID);

        Mesh detailMesh = new Mesh();
        detailMesh.vertices = mesh.vertsList[IWallComponent.WINDOW_LIST_ID].ToArray();
        detailMesh.triangles = mesh.trisList[IWallComponent.WINDOW_LIST_ID].ToArray();
        detailMesh.uv = mesh.uvsList[IWallComponent.WINDOW_LIST_ID].ToArray();
        wallmesh.SetMesh(detailMesh, IWallComponent.WINDOW_LIST_ID);

        Mesh doorMesh = new Mesh();
        doorMesh.vertices = mesh.vertsList[IWallComponent.DOOR_LIST_ID].ToArray();
        doorMesh.triangles = mesh.trisList[IWallComponent.DOOR_LIST_ID].ToArray();
        doorMesh.uv = mesh.uvsList[IWallComponent.DOOR_LIST_ID].ToArray();
        wallmesh.SetMesh(doorMesh, IWallComponent.DOOR_LIST_ID);

        return wallmesh;
    }

    public static Mesh CombineMeshes(List<Mesh> meshes, bool combineSubmeshes = true) {
        CombineInstance[] combine = new CombineInstance[meshes.Count];

        for(int i = 0; i < combine.Length; i++) {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.identity;
        }
        Mesh m = new Mesh();
        m.CombineMeshes(combine, combineSubmeshes);
        m.RecalculateNormals();
        return m;
    }

    public static WallMesh CombineWallMeshes(List<WallMesh> meshes) {
        List<List<Mesh>> meshesList = new List<List<Mesh>>(meshes[0].Count());
        for(int i = 0; i < meshesList.Capacity; i++) {
            meshesList.Add(new List<Mesh>());
        }
        foreach(WallMesh mesh in meshes) {
            if(mesh.Count() != meshesList.Capacity) {
                throw new System.Exception("Meshes are not of same dimension");
            }
            for(int i = 0; i < meshesList.Capacity; i++) {
                meshesList[i].Add(mesh.GetMesh(i));
            }
        }
        WallMesh toReturn = new WallMesh(meshesList.Count);
        for(int i = 0; i < meshesList.Count; i++) {
            Mesh m = CombineMeshes(meshesList[i]);
            toReturn.SetMesh(m, i);
        }
        toReturn.RecalculateNormals();

        return toReturn;
    }

    // Creates a building floor from a given wall template and floorplan. Floorplan must be clockwise.
    public static WallMesh CreateFloor(List<IWallComponent> walls, List<Vector3> floorplan, float height, bool generateCeiling) {
        return CreateFloor(walls, null, floorplan, height, generateCeiling);
    }

    // Creates a floor with a door on the first face
    public static WallMesh CreateFloor(List<IWallComponent> wallTemplate, IWallComponent door, List<Vector3> floorplan, float height, bool generateCeiling) {
        return CreateFloor(wallTemplate, door, 0, floorplan, height, generateCeiling);
    }

    // TODO consider having more than just one front face. List of selected ints? Assume contiguous and have lower to upper?
    // Creates a building floor from a given regular wall template, front template, and floorplan. Floorplan must be clockwise.
    public static WallMesh CreateFloor(List<IWallComponent> wallTemplate, IWallComponent door, int frontFaceIndex, List<Vector3> floorplan, float height, bool generateCeiling) {
        List<WallMesh> wallmeshes = new List<WallMesh>();
        // For each line in the floorplan
        for(int i = 0; i < floorplan.Count; i++) {
            Vector3 topLeft = floorplan[i];
            Vector3 direction = floorplan[(i + 1) % floorplan.Count] - topLeft;
            float width = direction.magnitude;
            direction.Normalize();
            WallMesh wallMesh;
            if(i == frontFaceIndex) {
                wallMesh = CreateMeshForWall(wallTemplate, door, topLeft, direction, width, height);
            } else {
                wallMesh = CreateMeshForWall(wallTemplate, topLeft, direction, width, height);
            }
            wallmeshes.Add(wallMesh);
        }
        if (generateCeiling) {
             wallmeshes.Add(FillPolygonWallMesh(floorplan));
        }

        return CombineWallMeshes(wallmeshes);
    }
    
    public static Mesh FillPolygon(List<Vector3> poly) {
        List<Vector2> polygon = new List<Vector2>(poly.Count);
        Vector2[] uvs = new Vector2[poly.Count];
        for(int i = 0; i < poly.Count; i++) {
            polygon.Add(new Vector2(poly[i].x, poly[i].z));
            uvs[i] = polygon[i];
        }
        List<int> ids = new List<int>();
        ids.AddRange(Enumerable.Range(0, polygon.Count));

        List<int> tris = new List<int>();
        Vector2 lastRemoved = polygon.First();
        while(polygon.Count > 3) {
            int earIndex = -1;
            try { 
                earIndex = FindEarOfPolygon(polygon); // A of the ABC that is an ear
            } catch(System.Exception e) {
                List<Vector2> p = MathUtility.ConvertTo2D(poly);
                //for (int i = 0; i < p.Count; i++) {
                //    Debug.DrawLine(p[i], p[(i + 1) % p.Count], Color.yellow, 300f);
                //}
                //for (int i = 0; i < polygon.Count; i++) {
                //    Debug.DrawLine(polygon[i], polygon[(i + 1) % polygon.Count], Color.red, 300f);
                //}
                //Debug.DrawLine(lastRemoved, lastRemoved + Vector2.down * 0.5f, Color.magenta, 300f);
            }
            // Add the triangle
            tris.Add(ids[earIndex]);
            tris.Add(ids[(earIndex + 1) % ids.Count]);
            tris.Add(ids[(earIndex + 2) % ids.Count]);
            // Remove the middle of the triangle
            ids.RemoveAt((earIndex + 1) % ids.Count);
            lastRemoved = polygon[(earIndex + 1) % ids.Count];
            polygon.RemoveAt((earIndex + 1) % ids.Count);
        }
        // Add last remaining triangle which must be convex
        tris.Add(ids[0]);
        tris.Add(ids[1]);
        tris.Add(ids[2]);

        Mesh mesh = new Mesh();
        mesh.vertices = poly.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;
        return mesh;
    }

    // Triangulates a concave polygon
    public static WallMesh FillPolygonWallMesh(List<Vector3> poly) {
        WallMesh m = new WallMesh(5);
        m.SetMesh(FillPolygon(poly), IWallComponent.ROOF_LIST_ID);
        return m;
    }

    private static int FindEarOfPolygon(List<Vector2> polygon) {
        for(int i = 0; i < polygon.Count; i++) {
            if(IsEar(i, polygon)) {
                return i;
            }                                
        }
        throw new System.Exception("No ears in this polygon. Most likely not enough samples");
    }

    // Is the triangle formed by I, I+1, and I+2 an ear
    // Samples two points and if they are both inside the polygon, and there are no intersections, return true;
    private static  bool IsEar(int index, List<Vector2> polygon) {
        int A = index;
        int B = (index + 1) % polygon.Count;
        int C = (index + 2) % polygon.Count;
        Vector2 start = polygon[A];
        Vector2 mid = polygon[B];
        Vector2 end = polygon[C];

        Vector2 dir = end - start;

        int count = 0;
        for(int i = 0; i < polygon.Count; i ++) {
            Vector2 seg_a = polygon[i];
            Vector2 seg_b = polygon[(i + 1) % polygon.Count];
                
            if(MathUtility.GetIntersection(start + dir.normalized * 0.1f, end - dir.normalized * 0.1f, seg_a, seg_b) != null) {
                count++;
            }
        }
        return count == 0 && LineWithinPolygon(start, end, polygon, 25);
    }

    private static bool LineWithinPolygon(Vector2 start, Vector2 end, List<Vector2> polygon, int samples) {
        Vector2 dir = (end - start);
        for(float i = 1; i < samples; i ++) {
            // Debug.DrawLine(start + dir * (i / (float)samples), (start + dir * (i / (float)samples)) + Vector2.down * 0.1f, Color.blue, 300f);
            if (!MathUtility.PointWithinPolygon(start + dir * ( i / (float)samples), polygon))
                return false;
        }
        return true;
    }

    public static GameObject AssignMeshesToGameObjects(List<Mesh> meshes, List<Material> mats) {
        GameObject parent = new GameObject("Building");
        return AssignMeshesToGameObjects(meshes, mats, parent);
    }


    // Creates a game object for each mesh and assigns the corresponding material to it
    public static GameObject AssignMeshesToGameObjects(List<Mesh> meshes, List<Material> mats, GameObject parent) {
        for (int i = 0; i < meshes.Count; i++){
            Mesh m = meshes[i];
            m.RecalculateNormals();
            Material mat = mats[i];
            GameObject child = new GameObject("Component");
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.AddComponent<MeshFilter>();
            child.AddComponent<MeshRenderer>();
            child.GetComponent<MeshRenderer>().material = mat;
            child.GetComponent<MeshFilter>().mesh = m;
        }
        return parent;
    }

    // Combines the meshes into a single mesh with submeshes and assigns the textures to it.
    public static GameObject AssignMeshesToGameObject(List<Mesh> meshes, List<Material> mats) {
        Mesh m = MeshCreator.CombineMeshes(meshes, false);

        GameObject parent = new GameObject("Building");
        parent.AddComponent<MeshFilter>().mesh = m;
        parent.AddComponent<MeshRenderer>().materials = mats.ToArray();
        return parent;
    }

    public struct BuildingProperties {
        public enum Roof { FLAT, BORDERED, RAISED, OFFSETSQUARE, POINTED }
        public float height;
        public string growthInstructions;
        public bool floorTrim;
        public Roof roofType;
        public BuildingProperties(float h, string gi, bool ft, Roof rt) {
            height = h;
            growthInstructions = gi;
            floorTrim = ft;
            roofType = rt;
        }
    }


    public static WallMesh CreateBuildingMesh(List<IWallComponent> primaryTemplate, IWallComponent doorWall, List<Vector3> floorplan, BuildingProperties properties) {
        List<WallMesh> meshes = new List<WallMesh>();
        // Shif it up so that the floorplan is the top left 
        floorplan = TranslatePolygon(floorplan, Vector3.up * properties.height);
        // If the first instruction is a shrink, put a roof on it
        WallMesh groundFloor = CreateFloor(primaryTemplate, doorWall, floorplan, properties.height, properties.growthInstructions[0] == SHRINK);
        meshes.Add(groundFloor);
        // shift up after creating ground floor
        for (int i = 0; i < properties.growthInstructions.Length; i++) {
            bool needCeil = i < properties.growthInstructions.Length - 1 && properties.growthInstructions[i + 1] != EXTRUDE;
            if(properties.growthInstructions[i] == EXTRUDE) {
                WallMesh floor;
                if(properties.floorTrim) { 
                    meshes.Add(CreateRoofTrim(floorplan, properties.height * 0.1f));
                    floorplan = TranslatePolygon(floorplan, Vector3.up * (properties.height));
                    floor = CreateFloor(primaryTemplate, floorplan, properties.height * 0.9f, needCeil);
                } else {
                    floorplan = TranslatePolygon(floorplan, Vector3.up * (properties.height));
                    floor = CreateFloor(primaryTemplate, floorplan, properties.height, needCeil);
                }
                meshes.Add(floor);
            } else if(properties.growthInstructions[i] == SHRINK) {
                floorplan = MathUtility.ShrinkPolygon(floorplan, 0.2f);
            }
        }
        
        meshes.Add(RoofMesh(floorplan, properties));
        
        return CombineWallMeshes(meshes);
    }

    public static WallMesh RoofMesh(List<Vector3> floorplan, BuildingProperties properties) {
        switch(properties.roofType) {
            case BuildingProperties.Roof.FLAT:
                return CreateRoofTrim(floorplan, properties.height * 0.1f);
            case BuildingProperties.Roof.BORDERED:
                return CreateBorderedRoof(floorplan, properties.height * 0.1f);
            case BuildingProperties.Roof.RAISED:
                return CreateRaisedRoof(floorplan, properties.height * Random.Range(0.5f, 0.9f));
            case BuildingProperties.Roof.POINTED:
                return CreatePointedRoof(floorplan, properties.height * Random.Range(0.5f, 1.5f));
            case BuildingProperties.Roof.OFFSETSQUARE:
                return CreateOffsetRoofPart(floorplan, properties.height * 0.5f);
            default:
                return CreateRoofTrim(floorplan, properties.height * 0.1f);
        }
    }

    public static Mesh CreateMeshesBetweenPolygons(List<List<Vector3>> polygons) {
        int c = polygons[0].Count;
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        Vector2[] uvs = new Vector2[c * polygons.Count];

        foreach(List<Vector3> polygonVerts in polygons) {
            verts.AddRange(polygonVerts);
        }
        // Upper expanded verts will be c * 2 to c * 3
        for(int i = 0; i < c; i++) {
            // set as x y coords for uvs
            for(int j = 0; j < polygons.Count - 1; j++) {
                Vector2 x = new Vector2(polygons[j][i].x, polygons[j][i].z);
                uvs[i + (c * j)] = x;

                tris.Add(i + (c * j));
                tris.Add(((i + 1) % c) + (c * j));
                tris.Add(((i + 1) % c) + c * (j + 1));

                tris.Add(i + (c * j));
                tris.Add(((i + 1) % c) + c * (j + 1));
                tris.Add(i + c * (j + 1));
            }
        }

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();
        m.uv = uvs;
        return m;
    }

    // Triangulates 3 sets of faces: A->B, B->C, then triangulates C.
    public static WallMesh ThreeFaceRoof(List<Vector3> firstVerts, List<Vector3> secondVerts, List<Vector3> thirdVerts,
        int textureId = IWallComponent.STRUCTURAL_ACCENT_LIST_ID) {

        WallMesh trim = new WallMesh(5);
        Mesh m = CreateMeshesBetweenPolygons(new List<List<Vector3>>() {
            firstVerts, secondVerts, thirdVerts
        });
        trim.SetMesh(m, textureId);

        WallMesh roof = FillPolygonWallMesh(thirdVerts);
        return CombineWallMeshes(new List<WallMesh> { trim, roof });
    }

    public static WallMesh CreateRoofTrim(List<Vector3> floorplan, float height) {
        List<Vector3> expandedFloorplan = MathUtility.ScalePolygon(floorplan, -0.04f);
        List<Vector3> expandedGrownFloorplan = MeshCreator.TranslatePolygon(expandedFloorplan, Vector2.up * height);
        return ThreeFaceRoof(floorplan, expandedFloorplan, expandedGrownFloorplan);
    }

    public static WallMesh CreateRaisedRoof(List<Vector3> floorplan, float height) {
        List<Vector3> shrunkRaised = MathUtility.ScalePolygon(floorplan, 0.3f, false);
        shrunkRaised = TranslatePolygon(shrunkRaised, Vector3.up * height * 0.6f);

        List<Vector3> shrunkRaised2 = MathUtility.ScalePolygon(shrunkRaised, 0.6f, false);
        shrunkRaised2 = TranslatePolygon(shrunkRaised2, Vector3.up * height * 0.4f);
        return ThreeFaceRoof(floorplan, shrunkRaised, shrunkRaised2, IWallComponent.ROOF_LIST_ID);
    }

    public static WallMesh CreateOffsetRoofPart(List<Vector3> floorplan, float height) {
        Vector3 mid = MathUtility.MidPoint(floorplan);
        // Move the mid point towards one of the other points in the floorplan
        Vector3 other = floorplan[Random.Range(0, floorplan.Count)];
        mid += (other - mid) * 0.5f;
        List<Vector3> shrunk = MathUtility.ScalePolygon(floorplan, 0.5f, mid, false);
        List<Vector3> shrunkRaised = TranslatePolygon(shrunk, Vector3.up * height);
        return ThreeFaceRoof(floorplan, shrunk, shrunkRaised);
    }


    public static WallMesh CreatePointedRoof(List<Vector3> floorplan, float height) {
        List<Vector3> shrunk = MathUtility.ScalePolygon(floorplan, 0.05f, false);

        List<Vector3> shrunkRaised = MathUtility.ScalePolygon(shrunk, 0.9f, false);
        shrunkRaised = TranslatePolygon(shrunkRaised, Vector3.up * height * 0.8f);
        return ThreeFaceRoof(floorplan, shrunk, shrunkRaised, IWallComponent.ROOF_LIST_ID);
    }

    public static WallMesh CreateBorderedRoof(List<Vector3> floorplan, float depth) {
        List<Vector3> topInner = MathUtility.ScalePolygon(floorplan, 0.04f);
        List<Vector3> bottomInner = MeshCreator.TranslatePolygon(topInner, Vector2.down * depth);
        return ThreeFaceRoof(floorplan, topInner, bottomInner, IWallComponent.STRUCTURAL_LIST_ID);
    }



    // Doesn't translate in place
    public static List<Vector3> TranslatePolygon(List<Vector3> floorplan, Vector3 translation) {
        List<Vector3> toReturn = new List<Vector3>();
        for(int i = 0; i < floorplan.Count; i++) {
            toReturn.Add(floorplan[i] + translation);
        }
        return toReturn;
    }
}

