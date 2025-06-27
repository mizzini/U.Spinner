using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Make the sprite face the camera
        //transform.LookAt(mainCamera.transform);
        //transform.Rotate(0, 180, 0);

        transform.rotation = mainCamera.transform.rotation;
    }
}
