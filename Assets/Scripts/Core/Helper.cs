using System.Collections;
using UnityEngine;

public class Helper
{
    public static Transform FindTransform(Transform parent, string name)
    {
        if (parent.name.Equals(name)) return parent;
        foreach (Transform child in parent)
        {
            Transform result = FindTransform(child, name);
            if (result != null) return result;
        }
        return null;
    }
    public static IEnumerator SetTimeout(System.Action callback, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for game time
        callback?.Invoke();
    }
    // ¥Îªk:
    //StartCoroutine(SetTimeout(() => {
    //    Debug.Log("Executed after 2 seconds!");
    //}, 2.0f));
}