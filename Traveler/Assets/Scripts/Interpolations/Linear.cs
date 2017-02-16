using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : MonoBehaviour
{
    private Transform m_interpolationObject;
    private Vector3 m_startPosition;
    private Vector3 m_endPosition;
    private float m_speed;
    private float m_time;
    private bool m_active = false;
    private bool m_finished = false;

    public void StartInterpolation(Transform origin, Transform target, float speed)
    {
        m_interpolationObject = origin;
        m_startPosition = m_interpolationObject.position;
        m_endPosition = target.position;
        m_speed = speed;
        m_time = 0;
        m_active = true;
    }

    public bool InterpolationFinished()
    {
        return m_finished;
    }

    void Update()
    {
        if (m_active)
        {
            m_time += Time.deltaTime * m_speed;

            if (m_time > 1)
            {
                m_time = 1;
                m_active = false;
                m_finished = true;
            }

            Vector3 pos = Vector3.Lerp(m_startPosition, m_endPosition, m_time);
            m_interpolationObject.position = pos;
        }
    }
}
