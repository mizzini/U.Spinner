using System.Collections;
using UnityEngine;

public class PointAndClickMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;
    private Vector3 targetDirection;
    [SerializeField]
    private float _moveDurationSeconds = 3f;

    private bool _isMoving = false;

    private Coroutine _movementCoroutine;

    private Transform _t;
    private Vector3 _lastPosition;

    private void Start()
    {
        _t = transform;
        _lastPosition = _t.position;
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (_movementCoroutine == null)
            {
                _movementCoroutine = StartCoroutine(MovementCoroutine(_moveDurationSeconds));
            }
            else
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
                _movementCoroutine = StartCoroutine(MovementCoroutine(_moveDurationSeconds));
            }

            // Get the clicked position in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Get the direction to the clicked position
                Vector3 direction = hit.point - _t.position;
                direction.y = 0; // Ignore vertical direction (only XZ plane)

                // Normalize the direction to one of 8 cardinal directions
                targetDirection = GetClosestCardinalDirection(direction);
            }
        }

        if (_isMoving)
        {
            _lastPosition = _t.position;
            // Move the object in the chosen direction
            if (targetDirection != Vector3.zero)
            {
                _t.position += _moveSpeed * Time.deltaTime * targetDirection;
            }
        }
        else
        {
            _t.position = _lastPosition;
        }
    }

    Vector3 GetClosestCardinalDirection(Vector3 direction)
    {
        // Define the 8 cardinal directions
        Vector3[] cardinalDirections = new Vector3[]
        {
            new Vector3(0, 0, 1),   // N
            new Vector3(1, 0, 1).normalized, // NE
            new Vector3(1, 0, 0),   // E
            new Vector3(1, 0, -1).normalized, // SE
            new Vector3(0, 0, -1),  // S
            new Vector3(-1, 0, -1).normalized, // SW
            new Vector3(-1, 0, 0),  // W
            new Vector3(-1, 0, 1).normalized  // NW
        };

        // Find the closest cardinal direction
        float maxDot = float.MinValue;
        Vector3 closestDirection = Vector3.zero;

        foreach (Vector3 cardinal in cardinalDirections)
        {
            float dot = Vector3.Dot(direction.normalized, cardinal);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestDirection = cardinal;
            }
        }

        return closestDirection;
    }

    private IEnumerator MovementCoroutine(float duration)
    {
        _isMoving = true;
        yield return new WaitForSeconds(duration);
        _isMoving = false;
        _movementCoroutine = null;
    }
}
