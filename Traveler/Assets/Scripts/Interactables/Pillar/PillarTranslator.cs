using UnityEngine;
using System.Collections;
using System;

public class PillarTranslator : AbstractInteractable
{
    private enum MovementStyle
    {
        Smooth, Constant, SmoothInfinite
    }

    [SerializeField]
    private MovementStyle m_currentStyle;
    [SerializeField]
    private float m_secondsEnlarged;
    [SerializeField]
    private float m_constantPillarExtendingSpeed;
    [SerializeField]
    private float m_constantPillarRetractionSpeed;
    [SerializeField]
    private float m_sinPillarExtendingSpeed;
    [SerializeField]
    private float m_sinPillarRetractionSpeed;

    private bool m_pillarInMotion = false;
    private Transform m_maxTran;
    private Transform m_startTran;
    private Transform m_currentTran;
    private Transform m_meshTran;
    // TEMPORARY: Remove when pillars with piviot at intended top are imported
    private Transform m_pillarTop;
    private BoxCollider m_standingArea;
    private MeshRenderer m_mesh;

    void Start()
    {
        m_mesh = GetComponentInChildren<MeshRenderer>();
        m_maxTran = transform.FindChild("TargetPos");
        m_startTran = transform.FindChild("StartPos");
        m_currentTran = transform.FindChild("Pillar");
        m_meshTran = m_currentTran.transform.FindChild("Mesh");
        m_pillarTop = m_meshTran.transform.FindChild("PillarTop");
        m_startTran.position = m_pillarTop.transform.position;
    }

    public override void Interact()
    {
        if (!m_pillarInMotion)
        {
            switch (m_currentStyle)
            {
                case MovementStyle.Smooth:
                    StartCoroutine(ExtendSin());
                    break;
                case MovementStyle.Constant:
                    StartCoroutine(ConstantExtension());
                    break;
                case MovementStyle.SmoothInfinite:
                    StartCoroutine(InfiniteSin());
                    break;
                default:
                    break;
            }
            m_pillarInMotion = true; 
        }
    }

    // Constant Translation
    IEnumerator ConstantExtension()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) < m_maxTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, m_constantPillarExtendingSpeed * Time.deltaTime, 0));
            yield return null;
        }
        yield return new WaitForSeconds(m_secondsEnlarged);
        StartCoroutine("ConstantRetraction");
    }
    IEnumerator ConstantRetraction()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) > m_startTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, -m_constantPillarRetractionSpeed * Time.deltaTime, 0));
            yield return null;
        }
        m_pillarInMotion = false;
    }
    // Translation along a sinus curve
    IEnumerator InfiniteSin()
    {
        float sinusCurveRange;
        float offset;
        float delta = (float)(-1.4 / m_sinPillarExtendingSpeed);

        sinusCurveRange = Mathf.Abs(m_maxTran.localPosition.y - m_startTran.localPosition.y) / 2;
        offset = sinusCurveRange + m_startTran.localPosition.y - (m_mesh.transform.localScale.y/2); 

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_sinPillarExtendingSpeed) * sinusCurveRange;

            m_currentTran.localPosition = new Vector3(
                                      m_currentTran.localPosition.x,
                                      y,
                                      m_currentTran.localPosition.z);

            delta += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ExtendSin()
    {
        float sinusCurveRange = ((m_maxTran.localPosition.y) - m_startTran.localPosition.y) / 2;
        float offset = sinusCurveRange + m_startTran.localPosition.y - (m_mesh.transform.localScale.y / 2);
        float targetPos = (m_maxTran.localPosition.y - m_mesh.transform.localScale.y / 2) - 0.01f;
        float delta = (float)(-1.5 / m_sinPillarExtendingSpeed);

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_sinPillarExtendingSpeed) * sinusCurveRange;

            if (targetPos < y)
                break;

            m_currentTran.localPosition = new Vector3(
                                      m_currentTran.localPosition.x,
                                      y,
                                      m_currentTran.localPosition.z);

            delta += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(m_secondsEnlarged);
        StartCoroutine(RetractSin());
    }

    IEnumerator RetractSin()
    {
        float sinusCurveRange = ((m_maxTran.localPosition.y) - m_startTran.localPosition.y) / 2;
        float offset = sinusCurveRange + m_startTran.localPosition.y - (m_mesh.transform.localScale.y / 2);
        float targetPos = (m_startTran.localPosition.y - (m_mesh.transform.localScale.y / 2) + 0.01f);
        float delta = (float)(1.5 / m_sinPillarRetractionSpeed);

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_sinPillarRetractionSpeed) * sinusCurveRange;

            if (targetPos > y)
                break;
 
            m_currentTran.localPosition = new Vector3(
                                      m_currentTran.localPosition.x,
                                      y,
                                      m_currentTran.localPosition.z);

            delta += Time.deltaTime;
            yield return null;
        }
        m_pillarInMotion = false;
    }
}
