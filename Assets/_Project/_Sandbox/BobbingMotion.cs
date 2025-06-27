using UnityEngine;

public class BobbingMotion : MonoBehaviour
{
    public float amplitude = 0.5f; // Height of the bobbing motion
    public float frequency = 1f;  // Speed of the bobbing motion

    private Vector3 startPosition;

    void Start()
    {
        // Record the object's initial position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate new position using sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the new position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
