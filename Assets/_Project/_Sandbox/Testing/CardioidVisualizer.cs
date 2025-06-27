using UnityEngine;

public class CardioidVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int resolution = 100; // Number of points on the curve
    public float _radius = 5f;
    public Vector3 _startPosition = Vector3.zero;
    public float rotationAngle = Mathf.PI / 4;    // Rotation angle in radians

    public Vector3 facingDirection;

    private CharacterMovement _characterMovement;
    private Transform _t;

    // TODO: make override for mathutil cardioid curve function
    // ****
    public float xScaleFactor = 1f;
    public float zScaleFactor = 1f;
    public float z0 = 1f; // Z  
    // ****

    private MathUtils.ParametricCurveDelegate CardioidCurve;

    [SerializeField]
    private bool _enabled = true;

    void Start()
    {
        if (!_enabled) { return; }

        CardioidCurve = MathUtils.CardioidCurve;

        _t = transform;
        _characterMovement = GetComponent<CharacterMovement>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        _startPosition = transform.position;
        //DrawCardioidCurve();
    }

    private void Update()
    {
        if (!_enabled) { return; }

        // TODO: if aiming...

        _startPosition = _t.position;
        // This is negated because we want to flip the curve.
        facingDirection = -_characterMovement.facingDirection;

        DrawCardioidCurve(facingDirection);
    }

    void DrawCardioidCurve(Vector3 facingDirection)
    {
        //...
        // Normalized facing direction
        facingDirection.y = 0; // Ignore the Y-axis for rotation

        // Set LineRenderer vertex count
        lineRenderer.positionCount = resolution + 1;

        // Generate and rotate cardioid points
        for (int i = 0; i <= resolution; i++)
        {
            float t = (Mathf.PI * 2f / resolution) * i;

            // Apply cardioid transformation to points.
            Vector3 rotatedPoint = _startPosition + CardioidCurve(t, _radius, facingDirection);

            // Assign the rotated point to the LineRenderer
            lineRenderer.SetPosition(i, rotatedPoint);
        }
    }
}
