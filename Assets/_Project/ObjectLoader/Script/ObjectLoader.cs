using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    public ObjectLoaderEntry[] entries;

    private void Awake()
    {
        if (entries == null) { return; }

        foreach (ObjectLoaderEntry entry in entries)
        {
            GameObject spawnedObject = Instantiate(entry.prefab);
            spawnedObject.transform.parent = null;

            if (!entry.enableRendering)
            {
                if (spawnedObject.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    renderer.enabled = false;
                    Debug.Log("Renderer disabled on " + spawnedObject.name);
                }
                else
                {
                    Debug.LogWarning("No Renderer found on " + spawnedObject.name);
                }
            }

            if (!entry.enableCollisions)
            {
                if (spawnedObject.TryGetComponent<Collider>(out Collider collider))
                {
                    collider.enabled = false;
                    Debug.Log("Collider disabled on " + spawnedObject.name);
                }
                else
                {
                    Debug.LogWarning("No Collider found on " + spawnedObject.name);
                }
            }
        }
    }
}
