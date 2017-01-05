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
    private Renderer m_renderer;

    void Start()
    {
        m_renderer = GetComponentInChildren<Renderer>();
        m_maxTran = transform.FindChild("TargetPos");
        m_startTran = transform.FindChild("StartPos");
        m_currentTran = transform.FindChild("Pillar");
        m_meshTran = m_currentTran.transform.FindChild("Mesh");
        m_pillarTop = m_meshTran.transform.FindChild("RaycastTarget");
        m_startTran.position = m_pillarTop.transform.position;
    }

    public override void Interact()
    {
        if (!m_pillarInMotion)
        {
            StartCoroutine(ExtendSin());
            //switch (m_currentStyle)
            //{
            //    case MovementStyle.Smooth:
            //        StartCoroutine(ExtendSin());
            //        break;
            //    case MovementStyle.Constant:
            //        StartCoroutine(ConstantExtension());
            //        break;
            //    case MovementStyle.SmoothInfinite:
            //        StartCoroutine(InfiniteSin());
            //        break;
            //    default:
            //        break;
            //}
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
        StartCoroutine("Retract");
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
        float delta = -1f;

        sinusCurveRange = ((m_maxTran.localPosition.y) - m_startTran.localPosition.y) / 2;
        offset = sinusCurveRange + m_startTran.localPosition.y - (m_renderer.bounds.size.y / 2);

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
        float sinusCurveRange;
        float offset;
        float delta = (float)(-1.5 / m_sinPillarExtendingSpeed);

        sinusCurveRange = ((m_maxTran.localPosition.y) - m_startTran.localPosition.y) / 2;
        offset = sinusCurveRange + m_startTran.localPosition.y - (m_renderer.bounds.size.y / 2);
        float targetPos = (m_maxTran.localPosition.y - m_renderer.bounds.size.y / 2) - 0.1f;
        while (true)
        {
            print(Mathf.Sin(delta * m_sinPillarExtendingSpeed) * sinusCurveRange);
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
        float sinusCurveRange;
        float offset;
        float delta = (float)(1.5 / m_sinPillarRetractionSpeed);

        sinusCurveRange = ((m_maxTran.localPosition.y) - m_startTran.localPosition.y) / 2;
        offset = sinusCurveRange + m_startTran.localPosition.y - (m_renderer.bounds.size.y / 2);

        float previousY = m_startTran.localPosition.y - (m_renderer.bounds.size.y / 2) + Mathf.Sin(delta * m_sinPillarRetractionSpeed) * sinusCurveRange * 2;

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_sinPillarRetractionSpeed) * sinusCurveRange;
            if ((m_startTran.localPosition.y - (m_renderer.bounds.size.y / 2)+ 0.01f) > y)
                break;
            m_currentTran.localPosition = new Vector3(
                                      m_currentTran.localPosition.x,
                                      y,
                                      m_currentTran.localPosition.z);
            delta += Time.deltaTime;
            previousY = y;
            yield return null;
        }
        m_pillarInMotion = false;
    }
}
