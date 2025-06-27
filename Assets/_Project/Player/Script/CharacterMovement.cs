using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;
    [SerializeField]
    private Vector3 _moveDirection;

    public Vector3 facingDirection; // Current facing direction

    void Update()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Calculate movement direction
        _moveDirection = new Vector3(horizontal, 0, vertical);

        // Normalize direction to avoid faster diagonal movement
        if (_moveDirection.magnitude > 1f)
        {
            _moveDirection.Normalize();
        }

        // Move the character
        transform.Translate(_moveSpeed * Time.deltaTime * _moveDirection, Space.World);

        // Update facing direction based on mouse position
        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert mouse position to world space
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            // Get the point where the mouse ray hits the ground (XZ plane)
            Vector3 targetPoint = hitInfo.point;

            // Calculate the direction to face
            facingDirection = (targetPoint - transform.position).normalized;
            facingDirection.y = 0; // Ensure no vertical rotation

            // Rotate the character to face the target direction
            if (facingDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(facingDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720f * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green; // Set the color for the Gizmo line
            Gizmos.DrawLine(transform.position, transform.position + facingDirection * 2f); // Draw a line
            Gizmos.color = Color.red; // Set the color for the Gizmo arrowhead
            Gizmos.DrawSphere(transform.position + facingDirection * 2f, 0.1f); // Draw an arrowhead
        }
    }
}
