using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MathUtility {
    public static Vector2? GetIntersection(Vector2 a_1, Vector2 a_2, Vector2 b_1, Vector2 b_2) {
        if ((a_2 - a_1).normalized == (b_1 - b_2).normalized || (a_2 - a_1).normalized == -(b_2 - b_1).normalized) {
            return null; // parallel, will not intersect
        }

        Vector2 intersection = IntersectionBetween(a_1, a_2, b_1, b_2);

        if (intersection.x >= Mathf.Min(a_1.x, a_2.x) && intersection.x <= Mathf.Max(a_1.x, a_2.x) &&
            intersection.y >= Mathf.Min(a_1.y, a_2.y) && intersection.y <= Mathf.Max(a_1.y, a_2.y) &&
            intersection.x >= Mathf.Min(b_1.x, b_2.x) && intersection.x <= Mathf.Max(b_1.x, b_2.x) &&
            intersection.y >= Mathf.Min(b_1.y, b_2.y) && intersection.y <= Mathf.Max(b_1.y, b_2.y)) {
            return intersection; // line segments overlap
        }
        return null;
    }

    public static Vector2 GetIntersection2(Vector2 a_1, Vector2 a_2, Vector2 b_1, Vector2 b_2) {
        Vector2? v= GetIntersection(a_1, a_2, b_1, b_2);
        if(v == null) {
            return Vector2.zero;
        } else {
            return (Vector2)v;
        }             
    }

    // Returns inifity, NaN when it doesn't intersect
    public static Vector2 IntersectionBetween(Vector2 a_1, Vector2 a_2, Vector2 b_1, Vector2 b_2) {
        float x = ((a_1.x * a_2.y - a_1.y * a_2.x) * (b_1.x - b_2.x) - (a_1.x - a_2.x) * (b_1.x * b_2.y - b_1.y * b_2.x)) /
                   ((a_1.x - a_2.x) * (b_1.y - b_2.y) - (a_1.y - a_2.y) * (b_1.x - b_2.x));
        float y = ((a_1.x * a_2.y - a_1.y * a_2.x) * (b_1.y - b_2.y) - (a_1.y - a_2.y) * (b_1.x * b_2.y - b_1.y * b_2.x)) /
                    ((a_1.x - a_2.x) * (b_1.y - b_2.y) - (a_1.y - a_2.y) * (b_1.x - b_2.x));
        return new Vector2(x, y);
    }

    public static bool IntersectionExists(Vector2 a_1, Vector2 a_2, Vector2 b_1, Vector2 b_2) {
        float res = (a_1.x - a_2.x) * (b_1.y - b_2.y) - (a_1.y - a_2.y) * (b_1.x - b_2.x);
        return Mathf.Abs(res) < float.Epsilon; // TODO double check this re: floating point error
    }

    // Returns true if the line formed by the first two parameters intersects with the segment formed by the second two parameters
    public static bool BoundedIntersectionExists(Vector2 a_1, Vector2 a_2, Vector2 point_1, Vector2 point_2) {
        Vector2 intersectionPoint = IntersectionBetween(a_1, a_2, point_1, point_2);
        Vector2 lowerLeft = new Vector2(Mathf.Min(point_1.x, point_2.x), Mathf.Min(point_1.y, point_2.y));
        Vector2 upperRight = new Vector2(Mathf.Max(point_1.x, point_2.x), Mathf.Max(point_1.y, point_2.y));
        return PointWithinBounds(intersectionPoint, lowerLeft, upperRight);
    }

    public static bool IsPolygonCounterClockwise(List<Vector3> cycle) {
        float total = 0f;
        for (int i = 0; i < cycle.Count; i++) {
            Vector3 start = cycle[i];
            Vector3 end = cycle[(i + 1) % cycle.Count];
            total += (end.x - start.x) * (end.z + start.z);
        }
        return total < 0;
    }

    public static bool LineSegmentIntersectPolygon(List<Vector2> polygon, Vector2 a, Vector2 b) {
        for (int i = 0; i < polygon.Count - 1; i++) {
            if (GetIntersection(a, b, polygon[i], polygon[i + 1]) != null) {
                return true;
            }
        }
        return false;
    }

    public static bool PointOnLineSegment(Vector2 point, Vector2 a, Vector2 b) {
        return PointOnLineSegment(point, a, b, 0);
    }

    public static bool PointOnLineSegment(Vector2 point, Vector2 a, Vector2 b, float tolerance) {
        return Vector2.Angle((point - a).normalized, (b - a).normalized) <= tolerance && 
            point.x >= Mathf.Min(a.x, b.x) && point.x <= Mathf.Max(a.x, b.x) &&
            point.y >= Mathf.Min(a.y, b.y) && point.y <= Mathf.Max(a.y, b.y);
    }

    public static bool PointOnLine(Vector2 point, Vector2 a, Vector2 b) {
        return PointOnLine(point, a, b, 0);
    }
    public static bool PointOnLine(Vector2 point, Vector2 a, Vector2 b, float tolerance) {
        return Vector2.Angle((point - a).normalized, (b - a).normalized) <= tolerance;
    }

    // ISSUE: If a point is exactly on the border of two lines, it counts towards both.
    public static bool PointWithinBounds(Vector2 point, Vector2 lowerLeft, Vector2 upperRight ) {
        bool result = point.x > lowerLeft.x && point.x <= upperRight.x &&
            point.y > lowerLeft.y && point.y <= upperRight.y;
        Debug.Log(point + " " + lowerLeft + " " + upperRight + " " + result);
        Debug.Log(point.x - lowerLeft.x);
        return result;
    }

    public static Vector2 RotateDirection(Vector2 dir, float angle) {
        return Quaternion.Euler(0, 0, angle) * dir;
    }

    public static Vector3 RandomVector(float range, Vector3 axies) {
        return new Vector3(
            UnityEngine.Random.Range(-range, range) * axies.x,
            UnityEngine.Random.Range(-range, range) * axies.y,
            UnityEngine.Random.Range(-range, range) * axies.z
        );

    }

    // Currently has issues with if the point is outside but inline with many verts
    public static bool PointWithinPolygon(Vector2 point, List<Vector2> polygon) {
        Vector2 point_b = point + Vector2.right * 1000 + Vector2.up * 331;
        int intersectionCount = 0;
        int count = polygon.Count;
        for(int i = 0; i < count; i++) {
            int next = (i + 1) % count;
            if(GetIntersection(point, point_b, polygon[i], polygon[next]) != null) {
                intersectionCount++;
            }
        }
        // If not within the polygon (as the number of intersection is even)
        if(intersectionCount % 2 == 0) {
            return PointOnEdgeOfPolygon(point, polygon);
        } else {
            return true;
        }
    }


    // Compares the normalized direction from A->B with C->B (where C is the point),
    //if they are the same and |C->B| < |A->B|, then it's on the line.
    public static bool PointOnEdgeOfPolygon(Vector2 point, List<Vector2> polygon) {
        int count = polygon.Count;
        for(int i = 0; i < count; i++) {
            int next = (i + 1) % count;
            Vector2 lineDir = polygon[next] - polygon[i];
            Vector2 ourDir = polygon[next] - point;
            if(lineDir.normalized == ourDir.normalized && lineDir.magnitude > ourDir.magnitude) {
                return true;
            }
        }
        return false;
    }

    public static float DistanceToLine(List<Vector2> road, Vector2 extension) {
        float distance = float.MaxValue;
        for (int i = 0; i < road.Count - 1; i++) {
            distance = Mathf.Min(distance, DistanceToLineSegment(extension, road[i], road[i + 1]));
        }
        return distance;
    }

    public static float DistanceToLineSegment(Vector2 point, Vector2 from, Vector2 to) {
        Vector2 dir = to - from;
        float run = Vector2.Dot(dir, point - from) / Vector2.Dot(dir, dir);
        float dist = 0.0f;
        if (run <= 0) {
            dist = (from - point).magnitude;
        } else if (run >= 1) {
            dist = (to - point).magnitude;
        } else {
            dist = (point - (from + run * dir)).magnitude;
        }
        return dist;
    }

    // Converts Vector2 into Vector3s with Y as zero
    public static List<Vector3> ConvertTo3D(List<Vector2> input) {
        List<Vector3> output = new List<Vector3>();
        foreach(Vector2 v in input) {
            output.Add(new Vector3(v.x, 0, v.y));
        }
        return output;
    }

    // Converts Vector3 into Vector2s using X and Z
    public static List<Vector2> ConvertTo2D(List<Vector3> input) {
        List<Vector2> output = new List<Vector2>();
        foreach(Vector3 v in input) {
            output.Add(new Vector2(v.x, v.z));
        }
        return output;
    }

    public static List<Vector2> ShrinkPolygon(List<Vector2> polygon, float amount) {
        return ConvertTo2D(ScalePolygon(ConvertTo3D(polygon), amount, false));
    }

    public static List<Vector3> ShrinkPolygon (List<Vector3> polygon, float amount) {
        return ScalePolygon(polygon, amount);
        //List<Vector2> pol = ConvertTo2D(polygon);
        //return ConvertTo3D(ShrinkPolygon(pol, amount));
    }

    public static List<Vector3> ScalePolygon(List<Vector3> polygon, float amount, bool normalized = true) {
        return ScalePolygon(polygon, amount, MidPoint(polygon), normalized);
    }

    public static Vector3 MidPoint(List<Vector3> polygon) {
        float x = 0;
        float z = 0;

        for (int i = 0; i < polygon.Count; i++) {
            x += polygon[i].x;
            z += polygon[i].z;
        }
        z /= polygon.Count;
        x /= polygon.Count;
        return new Vector3(x, polygon[0].y, z);
    }

    // Scales polygon by [amount]%
    public static List<Vector3> ScalePolygon(List<Vector3> polygon, float amount, Vector3 mid, bool normalized = true) {
        List<Vector3> toReturn = new List<Vector3>();

        for (int i = 0; i < polygon.Count; i++) {
            Vector3 diff = normalized ? (mid - polygon[i]).normalized * amount : (mid - polygon[i]) * amount;
            toReturn.Add(polygon[i] + diff);
        }
        return toReturn;
    }
    // Calculates the distance to the nearest point on an infinite line
    public static float DistanceToLine(Vector2 a, Vector2 b, Vector2 point) {
        Vector2 line = b - a;
        Vector2 bisector = new Vector2(-line.y, line.x).normalized;
        Vector2 intersection = MathUtility.IntersectionBetween(a, b, point, point + bisector);
        return (intersection - point).magnitude;
    }
        // Computes the Bisector of the angle ABC
    // Returns zero vector if angle is NaN
    public static Vector2 AngleBisectorBetweenPoints(Vector2 a, Vector2 b, Vector2 c) {
        Vector2 e_1 = a - b;
        Vector2 e_2 = c - b;
        float angle = Vector2.Angle(e_1, e_2) / 2f;

        if (float.IsNaN(angle)) {
            return Vector2.zero;
        }
        Vector2 dir = b - c;
        Vector2 bisector = new Vector2(-dir.y, dir.x).normalized;
        bool isReflex = Vector2.Dot(e_1, bisector) <= 0;
        if (isReflex) {
            angle *= -1;
        }

        Vector2 result = MathUtility.RotateDirection(e_1, angle);
        if (isReflex)
            result *= -1;
        return result.normalized;
    }

    // Takes instructions (like turtle) and turns it into a polygon
    public static List<Vector2> InstructionsToPoints(List<Vector2> input) {
        List<Vector2> output = new List<Vector2>();
        output.Add(input[0]);
        Vector2 current = input[0];
        for (int i = 1; i < input.Count; i++) {
            current = current + input[i];
            output.Add(current);
        }
        return output;
    }

    // Takes turtle like instructions and turns it into a list of points
    public static List<Vector3> InstructionsToPoints(List<Vector3> input, float scale = 3)
    {
        List<Vector3> output = new List<Vector3>();
        output.Add(input[0] * scale);
        Vector3 current = input[0] * scale;
        for (int i = 1; i < input.Count; i++)
        {
            current = current + (input[i] * scale);
            output.Add(current);
        }
        return output;
    }


    // Calculates length of a line
    public static float DistanceOfLine(List<Vector3> line) {
        float dist = 0f;
        for (int i = 0; i < line.Count - 1; i++) {
            dist += (line[i + 1] - line[i]).magnitude;
        }
        return dist;
    }    
}
