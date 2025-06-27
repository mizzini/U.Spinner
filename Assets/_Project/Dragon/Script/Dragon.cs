using UnityEngine;

public class Dragon : MonoBehaviour
{
    // For Testing
    // TODO: randomize over an x,z ground plane
    //public Transform point1;
    //public Transform point2;

    private Vector3 _pointA, _pointB;

    //private Vector3 _midpoint;

    // For Testing
    //public Vector3 p;

    private Transform _t;
    [SerializeField]
    private float _duration = 3f;
    private float _timeElapsed = 0f;
    [SerializeField]
    private float _r = 5f; // Height of jump; radius of arc.
    private Plane _plane;

    Vector3 linearPosition;
    [SerializeField]
    private float _pointDist = 5f;

    private bool _isSeeking; // true when we are seeking for new points.
    private Coroutine _seekingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _t = transform;

        // Define a plane
        _plane = new Plane(Vector3.up, Vector3.zero);
        RandomPointPair(_plane, _pointDist, out _pointA, out _pointB);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSeeking)
        {
            _timeElapsed += Time.deltaTime;

            // Normalized time
            float t = Mathf.Clamp01(_timeElapsed / _duration);

            // Lerp between start and end points
            //linearPosition = Vector3.Lerp(point1.position, point2.position, t);
            linearPosition = Vector3.Lerp(_pointA, _pointB, t);

            ResetOnCompletion();

            // Add curved motion according to normal
            float height = Mathf.Sin(t * Mathf.PI) * _r;
            linearPosition.y += height; // TODO: any N

            _t.position = linearPosition;
        }
    }

    private void ResetOnCompletion()
    {
        // Reset
        if (linearPosition == _pointB && !_isSeeking)
        {
            _timeElapsed = 0f;
            RandomPointPair(_plane, _pointDist, out _pointA, out _pointB);

            // Pause for a time before next movement
            if (_seekingCoroutine == null)
            {
                _seekingCoroutine = StartCoroutine(Utils.ToggleFlag(value => _isSeeking = value, true, false, 1f));
            }
        }

        // Clear when finished
        if (_isSeeking)
        {
            _seekingCoroutine = null;
        }
    }

    // Ax + By + Cz + D = 0
    private Vector3 RandomPoint(Plane p, float range)
    {
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);

        // Plane.normal = (A, B, C)
        // Plane.distance = D or -D ?
        float y = (-p.normal.x * x - p.normal.z * z - p.distance) / p.normal.y;

        return new Vector3(x, y, z);
    }

    // Generate a random tangent direction to find co-planar point to any random point
    private Vector3 RandomTangentDirection(Plane p)
    {
        // Generate a random direction perpendicular to the plane's normal
        // by using the cross product.
        Vector3 tangent = Vector3.Cross(p.normal, Vector3.up);
        // Handle edge case when plane normal is (0,1,0) ?
        if (tangent == Vector3.zero) tangent = Vector3.right;

        Vector3 bitangent = Vector3.Cross(p.normal, tangent).normalized;

        // Rotate some degrees around plane normal
        float angle = Random.Range(0f, 360f);
        Vector3 direction = Quaternion.AngleAxis(angle, p.normal) * bitangent;

        return direction.normalized;
    }

    private void RandomPointPair(Plane p, float distance, out Vector3 pointA, out Vector3 pointB)
    {
        pointA = RandomPoint(p, distance);
        Vector3 randomDir = RandomTangentDirection(p);
        pointB = pointA + randomDir * distance;
    }

}
