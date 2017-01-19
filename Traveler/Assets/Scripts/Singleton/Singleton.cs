using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;
    private static object m_Lock = new object();
    private static bool m_IsQuitting = false;

    /// <summary>
    /// This is the static instance of any given singleton,
    /// any declared public-variables can be accessed through this instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_IsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopenning the scene might fix it.");
                        return m_Instance;
                    }

                    if (m_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        m_Instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " + m_Instance.gameObject.name);
                    }
                }

                return m_Instance;
            }
        }
    }

    public void OnDestroy()
    {
        m_IsQuitting = true;
    }
}