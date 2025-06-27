using System.Collections;
using UnityEngine;

public class Utils
{
    // Coroutine to toggle a flag over some time in seconds.
    public static IEnumerator ToggleFlag(System.Action<bool> setFlag, bool initialValue, bool finalValue, float duration)
    {
        setFlag(initialValue);
        yield return new WaitForSeconds(duration);
        setFlag(finalValue);
    }
}
