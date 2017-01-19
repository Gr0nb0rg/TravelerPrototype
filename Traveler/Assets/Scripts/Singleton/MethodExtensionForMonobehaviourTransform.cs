using UnityEngine;
using System.Collections;

static public class MethodExtensionForMonoBehaviourTransform
{
    static public T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }
}
