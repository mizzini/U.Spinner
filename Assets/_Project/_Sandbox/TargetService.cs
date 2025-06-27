using System.Collections.Generic;
using UnityEngine;

// TODO: determine when and how available targets are cleared.
public class TargetService : MonoBehaviour
{
    // Available for components to access.
    // All possible targets in a scene
    public HashSet<Transform> targets = new HashSet<Transform>();

    private void Start()
    {
        // Debug
        Debug.Log(targets.Count + " targets registered...");
    }

    // Available for targets in scene to register with TargetService
    public void Register(Transform target)
    {
        targets.Add(target);
        Debug.Log(target.name + " added to available targets");
    }

    // Available for targets in scene to remove themselves from available targets.
    public void Remove(Transform target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
            Debug.Log(target.name + " removed from available targets");
        }
    }

    // Maybe pass it a region?
    public HashSet<Transform> GetTargets()
    {
        return targets;
    }
}
