using UnityEngine;

public class MathUtils
{
    public delegate Vector3 ParametricCurveDelegate(float t, float radius, Vector3 forwardDirection);

    public static Vector3 CardioidCurve(float t, float radius, Vector3 forwardDirection)
    {
        // Parametric equation for cardioid curve (boomerang motion)
        float xOffset = radius * Mathf.Cos(t) * (1 - Mathf.Cos(t)); // x = r * cos(t) * 1-cos(t)
        float zOffset = radius * Mathf.Sin(t) * (1 - Mathf.Cos(t)); // y = r * sin(t) * 1-cos(t)

        // Calculate the rotation angle based on the facing direction
        float rotationAngle = Mathf.Atan2(forwardDirection.x, forwardDirection.z);

        // Precompute sine and cosine of the rotation angle
        float cosA = Mathf.Cos(rotationAngle);
        float sinA = Mathf.Sin(rotationAngle);

        // Apply 2D rotation matrix
        float rotatedZ = cosA * xOffset - sinA * zOffset;
        float rotatedX = sinA * xOffset + cosA * zOffset;

        return new Vector3(rotatedX, 0, rotatedZ);
    }

    public static Vector3 CatmullRomSpline(float t, int i, Vector3[] controlPoints, float tension = 0.5f)
    {
        // Ensure valid index range
        int p0 = Mathf.Clamp(i - 1, 0, controlPoints.Length - 1);
        int p1 = i;
        int p2 = Mathf.Clamp(i + 1, 0, controlPoints.Length - 1);
        int p3 = Mathf.Clamp(i + 2, 0, controlPoints.Length - 1);

        // Catmull-Rom formula
        float t2 = t * t;
        float t3 = t2 * t;

        return tension * (
            (2f * controlPoints[p1]) +
            (-controlPoints[p0] + controlPoints[p2]) * t +
            (2f * controlPoints[p0] - 5f * controlPoints[p1] + 4f * controlPoints[p2] - controlPoints[p3]) * t2 +
            (-controlPoints[p0] + 3f * controlPoints[p1] - 3f * controlPoints[p2] + controlPoints[p3]) * t3
        );
    }

    // Return a midpoint in 3d space between p1 and p2
    // N is the desired normal vector
    // r is the radius or distance along the normal
    public static Vector3 Midpoint(Vector3 p1, Vector3 p2, Vector3 N, float r)
    {
        Vector3 m = new Vector3
        {
            x = (p1.x + p2.x) / 2,
            y = (p1.y + p2.y) / 2,
            z = (p1.z + p2.z) / 2
        };

        Vector3 p = m + r * N;

        return p;
    }

}
