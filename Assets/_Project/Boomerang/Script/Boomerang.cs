using System;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public event Action OnRevolutionComplete;

    [Header("Parametric Equation Parameters")]
    [SerializeField]
    private float _radius = 5.0f;       // Radius of the curve
    [SerializeField]
    private float _speed = 1.0f;        // Speed of rotation
    private float t = 0.0f;           // Parameter (time)

    private Transform _t; // Cache the transform for performance.
    private Vector3 _startPosition; // TODO: this will follow the player

    // TODO: Just use collider.
    private float _tolerance = 0.2f; // Value to help detect when boomerang has left launch point.
    private bool _detectThrow;

    // Boomerang states
    // The player has triggered a throw.
    [SerializeField]
    private bool _isThrown;

    [SerializeField]
    private float _detectThrowDuration = 0.2f;
    private Coroutine _detectThrowCoroutine;

    [SerializeField]
    private bool _enabled;

    // TODO: this should be more of a follow/return object, maybe assigned at handler level
    // in terms of possible re-use.
    private Transform _player;
    private CharacterMovement _characterMovement;

    [SerializeField]
    private AnimationCurve _radiusCurve;

    // Curve visualization
    [SerializeField]
    private int _renderResolution = 100;

    //private MathUtils.ParametricCurveDelegate CardioidCurve;

    [SerializeField]
    private float _xScaleFactor = 1f;
    [SerializeField]
    private float _zScaleFactor = 1f;

    // Catmull-Rom
    [SerializeField]
    private List<Transform> _targets;
    [SerializeField]
    private Vector3[] _controlPoints; // Array of control points
    private int _currentSegment = 1; // Current spline segment index (starts at 1)

    private void SetTargets(Transform[] targets)
    {
        // Using player position as guides, but also as control points.
        _targets = new List<Transform> { _player, _player };
        for (int i = 0; i < targets.Length; i++)
        {
            _targets.Add(targets[i]);
        }
        _targets.Add(_player);
        _targets.Add(_player);

        _controlPoints = new Vector3[_targets.Count];
        for (int i = 0; i < _targets.Count; i++)
        {
            _controlPoints[i] = _targets[i].position;
        }
    }

    private void ClearTargets()
    {
        _targets.Clear();
        _controlPoints = null;
    }

    // TODO: implement animation curve for speed
    private void Start()
    {
        //CardioidCurve = MathUtils.CardioidCurve;

        _t = transform;
        _startPosition = _t.position;

        // Find the player
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _player = player.transform;
            _characterMovement = player.GetComponent<CharacterMovement>();
        }
        else
        {
            Debug.LogWarning(name + " cannot find Player in scene!");
        }
    }

    void Update()
    {
        UpdateBoomerang();
    }

    private void UpdateBoomerang()
    {
        if (!_isThrown) { return; }

        if (_player != null)
        {
            _startPosition = _player.position;
        }

        Vector3 facingDirection = -_characterMovement.facingDirection;

        // Normalize the facing direction to ensure correct calculations
        facingDirection.y = 0; // Ignore the Y-axis for rotation
        // Already normalized.
        //facingDirection.Normalize();

        // Update the parameter t based on time and speed
        t += Time.deltaTime * _speed;
        // Debug.Log(t.ToString());

        // Check if we reached the end of the current segment
        if (t > 1f)
        {
            Debug.Log("revolution...");
            t = 0f; // Reset t
            _currentSegment++; // Move to the next segment

            // Handle looping or stopping at the end
            if (_currentSegment >= _controlPoints.Length - 3)
            {
                // A revolution (hitting all control points) has completed.
                // Looping... wrap to the first segment
                _currentSegment = 1;
                // Cancel throw state.
                _isThrown = false;
                Disable();
                OnRevolutionComplete?.Invoke();
            }
        }

        // Evaluate the position on the spline
        Vector3 newPosition = MathUtils.CatmullRomSpline(t, _currentSegment, _controlPoints);

        // Update the object's position
        _t.position = newPosition;
    }

    public void Throw(Transform[] targets)
    {
        SetTargets(targets);
        //Debug.Log(targets.Length);
        StartThrow();
    }

    private void Enable()
    {
        // Ensure boomerang isn't already enabled.
        if (_enabled) { return; }
        _enabled = true;

        // Enable visibility
        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            renderer.enabled = true;
            //Debug.Log("Renderer enabled on " + name);
        }

        // Enable collisions
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = true;
            //Debug.Log("Collider enabled on " + name);
        }
    }

    private void Disable()
    {
        // Ensure boomerang isn't already enabled.
        if (!_enabled) { return; }
        _enabled = false;

        // Enable visibility
        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            renderer.enabled = false;
            //Debug.Log("Renderer disabled on " + name);
        }

        // Enable collisions
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
            //Debug.Log("Collider disabled on " + name);
        }
    }

    // Set up the throw.
    private void StartThrow()
    {
        // Enable the boomerang
        Enable();

        // Ensure the boomerang is accurately detecting when it has been throwed from Player.
        if (_detectThrowCoroutine == null)
        {
            _detectThrowCoroutine = StartCoroutine(Utils.ToggleFlag(value => _detectThrow = value, false, true, _detectThrowDuration));
        }

        // Thrown state.
        _isThrown = true;
    }

    void OnDrawGizmos()
    {
        if (_controlPoints == null || _controlPoints.Length < 4) return;

        Gizmos.color = Color.green;

        int resolution = 20;
        for (int i = 1; i < _controlPoints.Length - 2; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float t1 = j / (float)resolution;
                float t2 = (j + 1) / (float)resolution;
                Gizmos.DrawLine(MathUtils.CatmullRomSpline(t1, i, _controlPoints), MathUtils.CatmullRomSpline(t2, i, _controlPoints));
            }
        }
    }

}

