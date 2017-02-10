using UnityEngine;
using System.Collections;
using System;

public class PillarTranslator : AbstractInteractable
{
    private enum MovementStyle
    {
        Smooth, Constant, SmoothInfinite, Catapult
    }

    [SerializeField]
    private MovementStyle m_currentStyle;
    [SerializeField]
    private float m_secondsEnlarged;
    [SerializeField]
    private float m_constantExtendingSpeed;
    [SerializeField]
    private float m_constantRetractionSpeed;
    [SerializeField]
    private float m_smoothExtendingSpeed;
    [SerializeField]
    private float m_smoothRetractionSpeed;
    [SerializeField]
    private float m_catapultLaunchAcceleration;
    [SerializeField]
    private float m_catapultReloadAcceleration;

    private bool m_pillarInMotion = false;
    private Transform m_maxTran;
    private Transform m_startTran;
    private Transform m_currentTran;
    private Transform m_meshTran;

    // Speed values are scaled to same level to make editing values more intuitive
    private float m_actualSmoothExtendingSpeed;
    private float m_actualSmoothRetractionSpeed;
    private float m_smoothScaleFactor = 0.14f;

    private Rigidbody m_rigidbody;

    // TEMPORARY: Remove when pillars with piviot at intended top are imported
    private Transform m_pillarTop;
    private BoxCollider m_standingArea;
    private MeshRenderer m_mesh;

    private bool m_physicalExtentionActive = false;
    private bool m_physicalExtentionStarted = false;
    private bool m_physicalRetractionActive = false;
    private bool m_physicalRetractionStarted = false;

    void Start()
    {
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        m_mesh = GetComponentInChildren<MeshRenderer>();
        m_maxTran = transform.FindChild("TargetPos");
        m_startTran = transform.FindChild("StartPos");
        m_currentTran = transform.FindChild("Pillar");
        m_meshTran = m_currentTran.transform.FindChild("Mesh");
        m_pillarTop = m_meshTran.transform.FindChild("PillarTop");
        m_startTran.position = m_pillarTop.transform.position;
        m_actualSmoothExtendingSpeed = m_smoothExtendingSpeed * m_smoothScaleFactor;
        m_actualSmoothRetractionSpeed = m_smoothRetractionSpeed * m_smoothScaleFactor;
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
                case MovementStyle.Catapult:
                    m_physicalExtentionActive = true;
                    m_physicalExtentionStarted = false;
                    break;
                default:
                    break;
            }
            m_pillarInMotion = true; 
        }
    }

    void FixedUpdate(){
        UpdatePhysicalExtension();
        UpdatePhysicalRetraction();
    }

    void UpdatePhysicalExtension() {
        if (m_physicalExtentionActive)
        {
            // Initiate
            if (!m_physicalExtentionStarted)
            {
                m_rigidbody.isKinematic = false;
                m_physicalExtentionStarted = true;
            }
            // Each frame
            if(m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) < m_maxTran.localPosition.y)
            {
                m_rigidbody.AddForce(transform.up * m_catapultLaunchAcceleration);
            }
            // Reset
            else
            {
                m_rigidbody.velocity = Vector3.zero;
                GetComponentInChildren<PillarParenter>().DeparentPlayer();
                m_physicalExtentionActive = false;
                m_physicalRetractionActive = true;
            }
        }
    }

    void UpdatePhysicalRetraction()
    {
        if (m_physicalRetractionActive)
        {
            // Each frame
            if (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) > m_startTran.localPosition.y)
            {
                m_rigidbody.AddForce(-transform.up * m_catapultReloadAcceleration);
            }
            // Reset
            else
            {   
                m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.isKinematic = true;
                m_physicalRetractionActive = false;
                m_pillarInMotion = false;
            }
        }
    }

    // Constant Translation
    IEnumerator ConstantExtension()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) < m_maxTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, m_constantExtendingSpeed * Time.deltaTime, 0));
            yield return null;
        }
        yield return new WaitForSeconds(m_secondsEnlarged);
        StartCoroutine("ConstantRetraction");
    }
    IEnumerator ConstantRetraction()
    {
        while (m_currentTran.localPosition.y + (m_pillarTop.localPosition.y * m_meshTran.localScale.y) > m_startTran.localPosition.y)
        {
            m_currentTran.Translate(new Vector3(0, -m_constantRetractionSpeed * Time.deltaTime, 0));
            yield return null;
        }
        m_pillarInMotion = false;
    }
    // Translation along a sinus curve
    IEnumerator InfiniteSin()
    {
        float sinusCurveRange;
        float offset;
        float delta = (float)(-1.4 / m_actualSmoothExtendingSpeed);

        sinusCurveRange = Mathf.Abs(m_maxTran.localPosition.y - m_startTran.localPosition.y) / 2;
        offset = sinusCurveRange + m_startTran.localPosition.y - (m_mesh.transform.localScale.y/2); 

        while (true)
        {
            // step = m_constantExtendingSpeed* Time.deltaTime
            float y = offset + Mathf.Sin(delta * m_actualSmoothExtendingSpeed) * sinusCurveRange;

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
        float delta = (float)(-1.5 / m_actualSmoothExtendingSpeed);

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_actualSmoothExtendingSpeed) * sinusCurveRange;

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
        float delta = (float)(1.5 / m_actualSmoothRetractionSpeed);

        while (true)
        {
            float y = offset + Mathf.Sin(delta * m_actualSmoothRetractionSpeed) * sinusCurveRange;

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
