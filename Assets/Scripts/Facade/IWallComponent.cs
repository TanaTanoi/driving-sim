using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityGenerator {
    public abstract class IWallComponent {

        public const int STRUCTURAL_LIST_ID = 0;
        public const int STRUCTURAL_ACCENT_LIST_ID = 1;
        public const int ROOF_LIST_ID = 2;
        public const int WINDOW_LIST_ID = 3;
        public const int DOOR_LIST_ID = 4;

        // Creates a list of wall components which are only structural or detail components. 
        public virtual List<IWallComponent> Evaluate() {
            List<IWallComponent> components = new List<IWallComponent>();
            components.Add(this);
            return components;
        }

        public void AddTriWithOffset(List<int> tris, int offset, int a1, int a2, int a3) {
            tris.Add(offset + a1);
            tris.Add(offset + a2);
            tris.Add(offset + a3);
        }

        // Generates the mesh represented by this wall component and stories the values in verts and points fields
        // pointStart: The index at which the point IDs start (so it can be attached to another list and not break)
        public abstract void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList);
    }

    public class Split : IWallComponent {
        // Layout level component

        // left: ab, right: ba, center: bab, mix: ababa
        public enum SplitType { LEFT, RIGHT, CENTER, MIX }

        private readonly IWallComponent firstChild;
        private readonly IWallComponent secondChild;
        private SplitType splitType;

        public Split(SplitType type, IWallComponent first, IWallComponent second) {
            firstChild = first;
            secondChild = second;
            splitType = type;
        }

        public override List<IWallComponent> Evaluate() {
            List<IWallComponent> firstChildComponents = firstChild.Evaluate();
            List<IWallComponent> secondChildComponents = secondChild.Evaluate();

            List<IWallComponent> components = new List<IWallComponent>();

            switch (splitType) {
                case SplitType.CENTER:
                    components.AddRange(secondChildComponents);
                    components.AddRange(firstChildComponents);
                    components.AddRange(secondChildComponents);
                    break;
                case SplitType.LEFT:
                    components.AddRange(firstChildComponents);
                    components.AddRange(secondChildComponents);
                    break;
                case SplitType.RIGHT:
                    components.AddRange(secondChildComponents);
                    components.AddRange(firstChildComponents);
                    break;
                case SplitType.MIX:
                    components.AddRange(secondChildComponents);
                    components.AddRange(firstChildComponents);
                    components.AddRange(secondChildComponents);
                    components.AddRange(firstChildComponents);
                    components.AddRange(secondChildComponents);
                    break;
            }

            return components;
        }

       public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {
            throw new NotImplementedException("Layout components do not implement CreateMesh");
        }

        public override string ToString() {
            return "SPLIT[" + splitType + "] {" + firstChild.ToString() + "} {" + secondChild.ToString() + "}";
        }
    }

    public class Repeat : IWallComponent {
        private readonly IWallComponent child;
        private int repeatNumber;

        public Repeat(IWallComponent child, int count) {
            this.child = child;
            repeatNumber = count;
        }

        public override List<IWallComponent> Evaluate() {
            List<IWallComponent> components = new List<IWallComponent>();
            for (int i = 0; i < repeatNumber; i++) {
                components.AddRange(child.Evaluate());
            }
            return components;
        }

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {
            throw new NotImplementedException("Layout components do not implement CreateMesh");
        }

        public override string ToString() {
            return "REPEAT[" + repeatNumber + "] {" + child.ToString() + "}";
        }
    }

    public class Mirror : IWallComponent {
        private readonly IWallComponent child;

        public Mirror(IWallComponent child) {
            this.child = child;
        }

        public override List<IWallComponent> Evaluate() {
            List<IWallComponent> components = new List<IWallComponent>();
            List<IWallComponent> childrenComponents = child.Evaluate();
            components.AddRange(childrenComponents);
            childrenComponents.Reverse();
            components.AddRange(childrenComponents);
            return components;
        }

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {
            throw new NotImplementedException("Layout components do not implement CreateMesh");
        }

        public override string ToString() {
            return "MIRROR{" + child.ToString() + "}";
        }
    }

    public class Border : IWallComponent {
        // Structural level component
        private readonly IWallComponent child;
        public readonly float verticalMargin;   // Percentage of margin out of 100, or normalised if > 50
        public readonly float horizontalMargin;

        // Vertical margin and Horizontal margins 
        public Border(IWallComponent child, float ver, float hor) {
            verticalMargin = Mathf.Min(ver / 100, 0.5f);
            horizontalMargin = Mathf.Min(hor / 100, 0.5f);
            this.child = child;
        }

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {
            List<Vector3> structuralVerts = vertsList[STRUCTURAL_LIST_ID]; // Verts 0 - 3 are the outer points counter clockwise. 4 - 7 are inner points counter clockwise

            int structTriOffset = structuralVerts.Count;

            float vGap = height * (verticalMargin); // Vertical gap from top and bottom
            float hGap = width * (horizontalMargin); // Horizontal gap from left and right

            float innerWidth = (width - (hGap * 2));
            float innerHeight = (height - (vGap * 2));

            structuralVerts.Add(topLeft); // top left
            structuralVerts.Add(topLeft + (Vector3.down * height)); // bottom left
            structuralVerts.Add(topLeft + (Vector3.down * height) + (direction * width)); // bottom right
            structuralVerts.Add(topLeft + (direction * width)); // top right
            Vector3 innerTopLeft = topLeft + (Vector3.down * vGap) + (direction * hGap);
            structuralVerts.Add(innerTopLeft); // inner top left
            structuralVerts.Add(innerTopLeft + (Vector3.down * innerHeight)); // inner bottom left
            structuralVerts.Add(innerTopLeft + (Vector3.down * innerHeight) + (direction * innerWidth)); // inner bottom right
            structuralVerts.Add(innerTopLeft + (direction * innerWidth)); // inner top right

            float maxWidth = width / height;
            List<Vector2> structuralUvs = uvsList[STRUCTURAL_LIST_ID];
            structuralUvs.Add(new Vector2(0, 0));
            structuralUvs.Add(new Vector2(0, 1));
            structuralUvs.Add(new Vector2(maxWidth, 1));
            structuralUvs.Add(new Vector2(maxWidth, 0));
            structuralUvs.Add(new Vector2(horizontalMargin * maxWidth, verticalMargin));
            structuralUvs.Add(new Vector2(horizontalMargin * maxWidth, 1 - verticalMargin));
            structuralUvs.Add(new Vector2(maxWidth - (horizontalMargin * maxWidth), 1 - verticalMargin));
            structuralUvs.Add(new Vector2(maxWidth - (horizontalMargin * maxWidth), verticalMargin));


            List<int> structuralTris = trisList[STRUCTURAL_LIST_ID];
            AddTriWithOffset(structuralTris, structTriOffset, 0, 1, 4);
            AddTriWithOffset(structuralTris, structTriOffset, 4, 1, 5);
            AddTriWithOffset(structuralTris, structTriOffset, 1, 6, 5);
            AddTriWithOffset(structuralTris, structTriOffset, 1, 2, 6);
            AddTriWithOffset(structuralTris, structTriOffset, 2, 7, 6);
            AddTriWithOffset(structuralTris, structTriOffset, 2, 3, 7);
            AddTriWithOffset(structuralTris, structTriOffset, 3, 4, 7);
            AddTriWithOffset(structuralTris, structTriOffset, 3, 0, 4);

            // new point start is pointStart + verts.length (8)
            child.CreateMesh(innerTopLeft, direction, innerWidth, innerHeight, vertsList, trisList, uvsList);
        }

        public override string ToString() {
            return "BORDER[" + verticalMargin + ", " + horizontalMargin
                + "] {" + child.ToString() + "}";
        }
    }

    public class Extrude : IWallComponent {
        // Structural level component
        private readonly IWallComponent child;
        public readonly float extrusion;

        public Extrude(IWallComponent child, float extrusion) {
            this.child = child;
            this.extrusion = extrusion / 100f;
        }

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {

            List<Vector3> structuralVerts = vertsList[STRUCTURAL_LIST_ID]; // Verts 0 - 3 are the outer points counter clockwise. 4 - 7 are inner points counter clockwise

            int structTriOffset = structuralVerts.Count;

            Vector3 bisector = new Vector3(-direction.z, 0, direction.x);
            structuralVerts.Add(topLeft); // top left
            structuralVerts.Add(topLeft + (Vector3.down * height)); // bottom left
            structuralVerts.Add(topLeft + (Vector3.down * height) + (direction * width)); // bottom right
            structuralVerts.Add(topLeft + (direction * width)); // top right

            for (int i = 0; i < 4; i++) {
                structuralVerts.Add(structuralVerts[i + structTriOffset] + (bisector * extrusion)); // Same for the inner parts.
            }

            float maxWidth = width / height;
            float maxExtrusion = extrusion / maxWidth;

            List<Vector2> structuralUvs = uvsList[STRUCTURAL_LIST_ID];
            structuralUvs.Add(new Vector2(0, 0));
            structuralUvs.Add(new Vector2(0, 1));
            structuralUvs.Add(new Vector2(maxWidth, 1));
            structuralUvs.Add(new Vector2(maxWidth, 0));
            structuralUvs.Add(new Vector2(maxExtrusion, 0));
            structuralUvs.Add(new Vector2(maxExtrusion, 1));
            structuralUvs.Add(new Vector2(maxExtrusion, 1));
            structuralUvs.Add(new Vector2(maxExtrusion, 0));

            List<int> structuralTris = trisList[STRUCTURAL_LIST_ID];
            AddTriWithOffset(structuralTris, structTriOffset, 0, 1, 4);
            AddTriWithOffset(structuralTris, structTriOffset, 4, 1, 5);
            AddTriWithOffset(structuralTris, structTriOffset, 1, 6, 5);
            AddTriWithOffset(structuralTris, structTriOffset, 1, 2, 6);
            AddTriWithOffset(structuralTris, structTriOffset, 2, 7, 6);
            AddTriWithOffset(structuralTris, structTriOffset, 2, 3, 7);
            AddTriWithOffset(structuralTris, structTriOffset, 3, 4, 7);
            AddTriWithOffset(structuralTris, structTriOffset, 3, 0, 4);
            
            // Call from the extruded point
            child.CreateMesh(topLeft + (bisector * extrusion), direction, width, height, vertsList, trisList, uvsList);
        }

        public override string ToString() {
            return "EXTRUDE[" + extrusion + "] {" + child.ToString() + "}";
        }
    }

    public class Panel : IWallComponent {
        // Detail level component
        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {

            List<Vector3> structuralVerts = vertsList[STRUCTURAL_LIST_ID];

            int structTriOffset = structuralVerts.Count;

            structuralVerts.Add(topLeft);
            structuralVerts.Add(topLeft + (Vector3.down * height)); // bottom left
            structuralVerts.Add(topLeft + (Vector3.down * height) + (direction * width)); // bottom right
            structuralVerts.Add(topLeft + (direction * width)); // top right

            List<int> structuralTris = trisList[STRUCTURAL_LIST_ID];
            AddTriWithOffset(structuralTris, structTriOffset, 0, 1, 3);
            AddTriWithOffset(structuralTris, structTriOffset, 3, 1, 2);

            List<Vector2> structuralUvs = uvsList[STRUCTURAL_LIST_ID];
            float maxWidth = width / height;
            structuralUvs.Add(new Vector2(0, 0));
            structuralUvs.Add(new Vector2(0, 1));
            structuralUvs.Add(new Vector2(maxWidth, 1));
            structuralUvs.Add(new Vector2(maxWidth, 0));
        }

        public override string ToString() {
            return "PANEL";
        }
    }

    public class Window : IWallComponent {
        private static float sillHeight = 0.03f;
        public float sillLength;
        public int verticalBars;
        public int horizontalBars;
        private int hozBarDiff;

        private float barWidth = 0.01f;
        private float barWidthFromWindow = 0.001f;

        public Window(int sill, int vb, int hb, int hbz) {
            sillLength = sill / 50f;
            verticalBars = vb;
            horizontalBars = hb;
            hozBarDiff = hbz;
        }

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {

            List<Vector3> detailVerts = vertsList[WINDOW_LIST_ID];

            int detailTriOffset = detailVerts.Count;

            detailVerts.Add(topLeft);
            detailVerts.Add(topLeft + (Vector3.down * height)); // bottom left
            detailVerts.Add(topLeft + (Vector3.down * height) + (direction * width)); // bottom right
            detailVerts.Add(topLeft + (direction * width)); // top right
            List<int> detailTris = trisList[WINDOW_LIST_ID];
            AddTriWithOffset(detailTris, detailTriOffset, 0, 1, 3);
            AddTriWithOffset(detailTris, detailTriOffset, 3, 1, 2);

            List<Vector2> detailUvs = uvsList[WINDOW_LIST_ID];
            float maxHeight = height / width;
            detailUvs.Add(new Vector2(0, 0));
            detailUvs.Add(new Vector2(0, maxHeight));
            detailUvs.Add(new Vector2(1, maxHeight));
            detailUvs.Add(new Vector2(1, 0));


            // Add window sill
            List<Vector3> structuralVerts = vertsList[STRUCTURAL_ACCENT_LIST_ID];
            List<int> structuralTris = trisList[STRUCTURAL_ACCENT_LIST_ID];
            List<Vector2> structuralUvs = uvsList[STRUCTURAL_ACCENT_LIST_ID];

            Vector3 bisector = new Vector3(-direction.z, 0, direction.x);

            if (sillLength > 0) {
                int structTriOffset = structuralVerts.Count;

                Vector3 topLeftSil = topLeft + (Vector3.down * (height - sillHeight));
                structuralVerts.Add(topLeftSil); // top left sill
                structuralVerts.Add(topLeftSil + (Vector3.down * sillHeight)); // bottom left sill
                structuralVerts.Add(topLeftSil + (Vector3.down * sillHeight) + (direction * width)); // bottom right sill
                structuralVerts.Add(topLeftSil + (direction * width)); // top right sil
                Vector3 bisectorSillLength = bisector * sillLength;
                for (int i = 0; i < 4; i++) {
                    structuralVerts.Add(structuralVerts[i + structTriOffset] + bisectorSillLength);
                }
                AddTriWithOffset(structuralTris, structTriOffset, 0, 1, 4);
                AddTriWithOffset(structuralTris, structTriOffset, 4, 1, 5);
                AddTriWithOffset(structuralTris, structTriOffset, 1, 6, 5);
                AddTriWithOffset(structuralTris, structTriOffset, 1, 2, 6);
                AddTriWithOffset(structuralTris, structTriOffset, 2, 7, 6);
                AddTriWithOffset(structuralTris, structTriOffset, 2, 3, 7);
                AddTriWithOffset(structuralTris, structTriOffset, 3, 4, 7);
                AddTriWithOffset(structuralTris, structTriOffset, 3, 0, 4);
                AddTriWithOffset(structuralTris, structTriOffset, 4, 6, 7);
                AddTriWithOffset(structuralTris, structTriOffset, 4, 5, 6);

                structuralUvs.Add(new Vector2(0, 0));
                structuralUvs.Add(new Vector2(0, sillHeight));
                structuralUvs.Add(new Vector2(1, sillHeight));
                structuralUvs.Add(new Vector2(1, 0));
                structuralUvs.Add(new Vector2(0, 0));
                structuralUvs.Add(new Vector2(0, sillHeight));
                structuralUvs.Add(new Vector2(width, sillHeight));
                structuralUvs.Add(new Vector2(width, 0));
            }

            for(int i = 1; i <= horizontalBars; i++) {
                float heightAlong = ((float)i / hozBarDiff) * height;
                AddBarBetween(
                    topLeft + (Vector3.down * heightAlong) + (bisector * barWidthFromWindow),
                    topLeft + (direction * width) + (Vector3.down * heightAlong) + (bisector * barWidthFromWindow),
                    barWidth, bisector, structuralVerts, structuralTris, structuralUvs);
            }

            for(int i = 1; i <= verticalBars; i++) {
                float widthAlong = ((float)i / (verticalBars + 1)) * width;
                Vector3 top = topLeft + (direction * widthAlong) + (bisector * barWidthFromWindow);
                AddBarBetween(
                    top,
                    top + (Vector3.down * height),
                    barWidth, bisector, structuralVerts, structuralTris, structuralUvs);
            }
        }

        public void AddBarBetween(Vector3 start, Vector3 end, float width, Vector3 bisector, List<Vector3> verts, List<int> tris, List<Vector2> uvs) {
            int startVerts = verts.Count;
            Vector3 dir = Vector3.Cross(bisector.normalized, (end - start).normalized); 
            verts.Add(start + (dir * (width / 2f))); // top left
            verts.Add(start - (dir * (width / 2f))); // bottom left
            verts.Add(end - (dir * (width / 2f))); // bottom right
            verts.Add(end + (dir * (width / 2f))); // top right

            AddTriWithOffset(tris, startVerts, 0, 1, 2);
            AddTriWithOffset(tris, startVerts, 2, 3, 0);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
        }

        public override string ToString() {
            return "WINDOW [" + sillLength + ", " + verticalBars + ", " + horizontalBars + "]";
        }
    }

    public class Door : IWallComponent {

        public override void CreateMesh(Vector3 topLeft, Vector3 direction, float width, float height, List<List<Vector3>> vertsList, List<List<int>> trisList, List<List<Vector2>> uvsList) {
            List<Vector3> doorVerts = vertsList[DOOR_LIST_ID];

            int doorTriOffset = doorVerts.Count;

            doorVerts.Add(topLeft);
            doorVerts.Add(topLeft + (Vector3.down * height)); // bottom left
            doorVerts.Add(topLeft + (Vector3.down * height) + (direction * width)); // bottom right
            doorVerts.Add(topLeft + (direction * width)); // top right

            List<int> doorTris = trisList[DOOR_LIST_ID];
            AddTriWithOffset(doorTris, doorTriOffset, 0, 1, 3);
            AddTriWithOffset(doorTris, doorTriOffset, 3, 1, 2);

            List<Vector2> doorUvs = uvsList[DOOR_LIST_ID];
            doorUvs.Add(new Vector2(0, 0));
            doorUvs.Add(new Vector2(0, 1));
            doorUvs.Add(new Vector2(1, 1));
            doorUvs.Add(new Vector2(1, 0));
        }

        public override string ToString() {
            return "DOOR";
        }
    }
}
