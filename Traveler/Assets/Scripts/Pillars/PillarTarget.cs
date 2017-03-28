using UnityEngine;

public class PillarTarget : MonoBehaviour
{
    public Vector3 m_position;
    public float m_time;

    public PillarTarget(Vector3 postion, float time)
    {
        m_position = postion;

        m_time = time <= 0 ? 0 : time;
    }
}
