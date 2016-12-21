using UnityEngine;
using System.Collections;
using System;

public class PillarTranslator : MonoBehaviour {
    [SerializeField]
    private float m_secondsEnlarged;
    [SerializeField]
    private float m_pillarMovementSpeed;
    private bool m_pillarInMotion = false;
    private Transform m_maxTran;
    private Transform m_startTran;
    private Transform m_currentTran;
    private Transform m_meshTran;
    // TEMPORARY: Remove when pillars with piviot at intended top are imported
    private Transform m_pillarTop;
    private BoxCollider m_standingArea;
    private Renderer m_renderer;

    void Start ()
    {
        m_renderer = GetComponentInChildren<Renderer>();
        m_maxTran = transform.FindChild("TargetPos");
        m_startTran = transform.FindChild("StartPos");
        m_currentTran = transform.FindChild("Pillar");
        m_meshTran = m_currentTran.transform.FindChild("Mesh");
        m_pillarTop = m_meshTran.transform.FindChild("PillarTop");
        m_startTran.position = m_pillarTop.transform.position;
    }

    public void ActivatePillar()
    {
        if (!m_pillarInMotion)
        {
            m_pillarInMotion = true;
            StartCoroutine("Extend");
        }
    }

    // Translation
    IEnumerator Extend()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) < m_maxTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, m_pillarMovementSpeed * Time.deltaTime, 0));
            yield return null;
        }
        yield return new WaitForSeconds(m_secondsEnlarged);
        StartCoroutine("Retract");
    }

    IEnumerator Retract()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) > m_startTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, - m_pillarMovementSpeed * Time.deltaTime, 0));
            yield return null;
        }
        m_pillarInMotion = false;
    }
}
