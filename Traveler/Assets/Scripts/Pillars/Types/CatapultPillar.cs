using UnityEngine;
using System.Collections;
using System;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/10
*/

/*
    SUMMARY
    Accelerates to target postiton and launches player from there before going back
*/

[RequireComponent(typeof(Pillar))]
public class CatapultPillar : AbstractInteractable
{
    [SerializeField]
    private float m_launchAcceleration;
    [SerializeField]
    private float m_reloadAcceleration;

    private bool m_pillarInMotion = false;

    private bool m_physicalExtentionActive = false;
    private bool m_physicalExtentionStarted = false;
    private bool m_physicalRetractionActive = false;
    private bool m_physicalRetractionStarted = false;

    private Pillar m_pillar;

    void Start()
    {
        m_pillar = GetComponent<Pillar>();
    }

    public override void Interact()
    {
        if (!m_pillarInMotion)
        {
            m_physicalExtentionActive = true;
            m_physicalExtentionStarted = false;
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
                m_pillar.GetRigidbody().isKinematic = false;
                m_physicalExtentionStarted = true;
            }
            // Each frame
            if(m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y)
                < m_pillar.GetTargetTran().localPosition.y)
            {
                m_pillar.GetRigidbody().AddForce(transform.up * m_launchAcceleration);
            }
            // Reset
            else
            {
                m_pillar.GetRigidbody().velocity = Vector3.zero;
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
            if (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) > m_pillar.GetStartTran().localPosition.y)
            {
                m_pillar.GetRigidbody().AddForce(-transform.up * m_reloadAcceleration);
            }
            // Reset
            else
            {
                m_pillar.GetRigidbody().velocity = Vector3.zero;
                m_pillar.GetRigidbody().isKinematic = true;
                m_physicalRetractionActive = false;
                m_pillarInMotion = false;
            }
        }
    }
}
