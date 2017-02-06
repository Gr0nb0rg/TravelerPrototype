using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Toolbox singleton-class, use this to set and get any public variables needed,
/// note that no more than 1 instance of this class should be present at any time!
/// </summary>
public class Toolbox : Singleton<Toolbox>
{
    //Make sure constructor cannot be used, true singleton
    protected Toolbox(){}

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public float m_CameraSensX = 3.0f;
    public float m_CameraSensY = 1.0f;
    public bool m_IsYInvert = false;

    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
