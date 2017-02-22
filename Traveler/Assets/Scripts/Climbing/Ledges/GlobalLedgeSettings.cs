using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.SerializableAttribute]
[CreateAssetMenu(menuName = "Ledges/GlobalSettings")]
public class GlobalLedgeSettings : ScriptableObject {
    public float m_distBetweenClimbPoints = 0.5f;
    public float m_triggerHeight = 0.8f;
    public float m_triggerDepth = 0.9f;

    public float TriggerDepth { get { return m_triggerDepth; } set { m_triggerDepth = value; } }
    public float TriggerHeight { get { return m_triggerHeight; } set { m_triggerHeight = value; } }
    public float DistanceBetweenPoints { get { return m_distBetweenClimbPoints; } set { m_distBetweenClimbPoints = value; }  }
}
