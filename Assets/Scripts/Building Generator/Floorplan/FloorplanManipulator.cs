using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorplanManipulator {
    public static float gridSize = 0.5f;

    /* 
     * Creates the largest possible polygon that fits within a grid of straight and diagonal lines.
     * Assumes the first edge is the primary road
     */
    //public List<Vector2> LargestQuantifiedPolygon(List<Vector2> input) {

    //}

    /*
     * Creates a grid based floor plan encapsulated by the input polygon.
     * Polygon must be counterclockwise, result is also counterclockwise
     */
    public static List<Vector2> CreateSquareMesh(List<Vector2> polygon) {
        float firstLineMagnitude = (polygon[0] - polygon[1]).magnitude;
        Vector2 zeroPoint = polygon[0];
        Vector2 forward = (polygon[1] - polygon[0]).normalized * gridSize;
        Vector2 left = new Vector2(-forward.y, forward.x);
        Debug.DrawLine(polygon[0], polygon[1], Color.blue, 300f);
        Debug.DrawLine(polygon[1], polygon[2], Color.cyan, 300f);
        Debug.DrawLine(zeroPoint, zeroPoint + forward, Color.grey, 300f);
        Debug.DrawLine(zeroPoint, zeroPoint + left, Color.green, 300f);
        // While invalid and not too far away from start point
        while (!ForwardAndLeftInPolygon(polygon, zeroPoint, forward, left)
            && firstLineMagnitude > (zeroPoint - polygon[0]).magnitude) {
            zeroPoint = zeroPoint + forward;
            Debug.Log("MOVED START");
        }
        Debug.DrawLine(zeroPoint, Vector2.down, Color.cyan, 300f);
        List<Vector2> points = new List<Vector2>();
        points.Add(zeroPoint);
        Vector2 currentDirection = forward;
        Vector2 currentPoint = zeroPoint;
        int count = 0;
        // While it doens't line up with the fist point TODO remove count once the condition is consistently good
        while (!MathUtility.PointOnLine(currentPoint, zeroPoint, zeroPoint + left, 0.0001f) && count < 50){
            count++;
            Vector2 currentLeft = new Vector2(-currentDirection.y, currentDirection.x);
            // if right is inside the polygon, go left and store current point
            if(MathUtility.PointWithinPolygon(currentPoint - currentLeft, polygon)) {
                Debug.Log("Moving right");
                points.Add(currentPoint);
                currentDirection = -currentLeft;
                currentPoint += currentDirection;
                
                // if can move forward, extend current point
            }  else if(MathUtility.PointWithinPolygon(currentPoint + currentDirection, polygon)) {
                Debug.Log("Moving forward");
                currentPoint += currentDirection;
                // if we can move left
            } else if(MathUtility.PointWithinPolygon(currentPoint + currentLeft, polygon)) {
                Debug.Log("Moving left");
                points.Add(currentPoint);
                currentDirection = currentLeft;
                currentPoint += currentDirection;
            } else { // we are stuck in a corner, back up until we can move left
                Debug.Log("waah");
                Debug.Log(currentPoint + currentLeft);
                Debug.DrawLine(currentPoint, currentPoint + currentLeft, Color.blue, 300f);
                Debug.DrawLine(currentPoint, currentPoint + currentDirection, Color.magenta, 300f);
                Debug.Log("Left is within polygon? " + MathUtility.PointWithinPolygon(currentPoint + currentLeft, polygon));
                Debug.DrawLine(currentPoint, Vector2.right * 10 + Vector2.down, Color.grey, 300f);
                int c = 0;
                while(!MathUtility.PointWithinPolygon(currentPoint + currentLeft, polygon) && c < 11) {
                    c++;
                    currentPoint -= currentDirection;
                }
                Debug.DrawLine(currentPoint, Vector2.right * 10 + Vector2.down, Color.green, 300f);
                points.Add(currentPoint);
                currentDirection = currentLeft;
                currentPoint += currentDirection;
            }
        }
        points.Add(currentPoint);
        Debug.Log(!MathUtility.PointOnLine(currentPoint, zeroPoint, zeroPoint + left, 0.0001f) + " " + count);
        return points;
    }

    public static bool ForwardAndLeftInPolygon(List<Vector2> polygon, Vector2 point, Vector2 forward, Vector2 left) {
        return MathUtility.PointWithinPolygon(point + forward, polygon) && MathUtility.PointWithinPolygon(point + left, polygon);

    }


    public static HashSet<Vector2> GridPointsWithinPolygon(List<Vector2> polygon) {
        Vector2 zeroPoint = polygon[0];
        Vector2 left = (polygon[1] - polygon[0]).normalized * gridSize;
        Vector2 forward = new Vector2(-left.y, left.x);
        HashSet<Point> contained = new HashSet<Point>();
        CheckAdjacent(contained, polygon, zeroPoint, forward, left);
        HashSet<Vector2> points = new HashSet<Vector2>();
        foreach(Point p in contained) {
            points.Add(p.Vec());
        }
        return points;
    }

    private static void CheckAdjacent(HashSet<Point> points, List<Vector2> polygon, Vector2 point, Vector2 forward, Vector2 left) {
        List<Vector2> neighbouringPoints = NeighbouringPoints(point, forward, left);
        foreach(Vector2 newPoint in neighbouringPoints) {
            Point p = new Point(newPoint, gridSize);
            if(MathUtility.PointWithinPolygon(newPoint, polygon) && !points.Contains(p)) {
                points.Add(p);
                Debug.Log(points.Count);
                CheckAdjacent(points, polygon, newPoint, forward, left);
            }
        }
        
    }

    private static List<Vector2> NeighbouringPoints(Vector2 center, Vector2 forward, Vector2 left) {
        return new List<Vector2>() {
            center + forward, center + forward + left, center + left,
            center + left - forward, center - forward, center - forward - left,
            center - left, center - left + forward
        };
    }
}

public struct Point {
    double x, y;
    public Point(Vector2 vec) {
        x = vec.x;
        y = vec.y;
    }

    public Point(Vector2 vec, float nearestValue) {
        float round = 1f / nearestValue;
        this.x = Mathf.Round(vec.x * round) / round;
        this.y = Mathf.Round(vec.y * round) / round;
    }

    public Point(float x, float y) {
        this.x = x;
        this.y = y;
    }

    // Prime factorisation should mean it's fine
    public override int GetHashCode() {
        return (int)((x + Mathf.Pow(3f, (float)y)) * 10);
    }

    public Vector2 Vec() {
        return new Vector2((float)x, (float)y);
    }
}