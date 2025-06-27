using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerVision : MonoBehaviour
{
    [SerializeField]
    private float _maxDistance = 10f; // Maximum range of vision
    [SerializeField]
    private float _angle = 45f; // Degrees of the vision cone
    [SerializeField]
    private int _resolution = 30; // Number of segments in the vision cone

    private LineRenderer _lineRenderer; // Reference to the LineRenderer
    private Transform _t; // Cache the player's transform

    private bool _renderVisionCone = false;

    void Start()
    {
        _t = transform;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = true; // Ensures the cone boundary is closed
        _lineRenderer.positionCount = _resolution + 2; // +2: one for center, one to close the cone
    }

    void Update()
    {
        if (_renderVisionCone)
        {
            _lineRenderer.enabled = true;
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    public void DrawVisionCone()
    {
        if (!_renderVisionCone) { _renderVisionCone = true; }
        // Set the center position (the player's position)
        _lineRenderer.SetPosition(0, _t.position);

        // Half of the cone angle
        float halfAngle = _angle / 2f;

        // Generate points along the cone's arc
        for (int i = 0; i <= _resolution; i++)
        {
            // Calculate the angle for this segment
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / _resolution);
            float radians = Mathf.Deg2Rad * currentAngle;

            // Calculate the point on the arc
            Vector3 direction = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
            Vector3 point = _t.position + (_t.rotation * direction) * _maxDistance;

            // Assign the point to the LineRenderer
            _lineRenderer.SetPosition(i + 1, point);
        }

        // Close the cone by connecting back to the center
        _lineRenderer.SetPosition(_resolution + 1, _t.position);
    }

    public void RemoveVisionCone()
    {
        if (_renderVisionCone) { _renderVisionCone = false; }
    }

    public bool CheckVisibility(Transform mark)
    {
        // Direction to target
        var directionToTarget = mark.position - _t.position;
        // Ignore the y axis when detecting targets.
        Vector3 flatDirectionToTarget = new Vector3(directionToTarget.x, 0, directionToTarget.z);
        var degreesToTarget = Vector3.Angle(transform.forward, flatDirectionToTarget);

        // Degrees from forward direction
        //var degreesToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // Check if within arc
        var withinArc = degreesToTarget < (_angle / 2);
        if (!withinArc)
        {
            return false;
        }

        var distanceToTarget = directionToTarget.magnitude;

        // Fire a ray that goes either the max distance or to the target
        var rayDistance = Mathf.Min(_maxDistance, distanceToTarget);
        var ray = new Ray(_t.position, directionToTarget);
        RaycastHit hitInfo;

        var canSee = false;

        if (Physics.Raycast(ray, out hitInfo, rayDistance))
        {
            if (hitInfo.collider.transform == mark)
            {
                canSee = true;
            }
            Debug.DrawLine(_t.position, hitInfo.point);
        }
        return canSee;
    }
}
