using UnityEngine;

public class CatmullRomSpline : MonoBehaviour
{
    public Vector3[] controlPoints; // Array of control points
    public Transform[] objects;


    private void Start()
    {
        controlPoints = new Vector3[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            controlPoints[i] = objects[i].position;
        }

    }

    public Vector3 Evaluate(float t, int i)
    {
        // Ensure valid index range
        int p0 = Mathf.Clamp(i - 1, 0, controlPoints.Length - 1);
        int p1 = i;
        int p2 = Mathf.Clamp(i + 1, 0, controlPoints.Length - 1);
        int p3 = Mathf.Clamp(i + 2, 0, controlPoints.Length - 1);

        // Catmull-Rom formula
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 result =
            0.5f * ((2 * controlPoints[p1]) +
                    (-controlPoints[p0] + controlPoints[p2]) * t +
                    (2 * controlPoints[p0] - 5 * controlPoints[p1] + 4 * controlPoints[p2] - controlPoints[p3]) * t2 +
                    (-controlPoints[p0] + 3 * controlPoints[p1] - 3 * controlPoints[p2] + controlPoints[p3]) * t3);

        return result;
    }

    void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Length < 4) return;

        Gizmos.color = Color.green;

        int resolution = 20;
        for (int i = 1; i < controlPoints.Length - 2; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float t1 = j / (float)resolution;
                float t2 = (j + 1) / (float)resolution;
                Gizmos.DrawLine(Evaluate(t1, i), Evaluate(t2, i));
            }
        }
    }
}
