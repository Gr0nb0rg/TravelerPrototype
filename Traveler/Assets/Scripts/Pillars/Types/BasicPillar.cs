using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/21
*/

/*
    SUMMARY
    Linearly moves a pillar from starting position to target and back again
*/

[RequireComponent(typeof(Pillar))]
public class BasicPillar : AbstractInteractable {
    // Settings and tweaking varaibles
    [SerializeField]
    private float m_secondsEnlarged;
    [SerializeField]
    private float m_extendingSpeed;
    [SerializeField]
    private float m_retractionSpeed;
    [SerializeField]
    private bool m_alwaysActive = false;

    private bool m_moveForwardAtStart = true;
    private bool m_pillarInMotion = false;
    private Pillar m_pillar;

    void Start()
    {
        m_pillar = GetComponent<Pillar>();

        if (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) > m_pillar.GetTargetTran().localPosition.y)
            m_moveForwardAtStart = false;

        if (m_alwaysActive)
            StartCoroutine(ConstantExtension());
    }

    public override void Interact()
    {
        if (!m_alwaysActive)
        {
            if (!m_pillarInMotion)
                StartCoroutine(ConstantExtension());
        }
    }

    // Constant Translation
    IEnumerator ConstantExtension()
    {
        m_pillarInMotion = true;
        if (m_moveForwardAtStart)
        {
            while (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) < m_pillar.GetTargetTran().localPosition.y)
            {
                m_pillar.GetCurrentTran().Translate(new Vector3(0, m_extendingSpeed * Time.deltaTime, 0));
                yield return null;
            }
        }
        else
        {
            while (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) > m_pillar.GetTargetTran().localPosition.y)
            {
                m_pillar.GetCurrentTran().Translate(new Vector3(0, -m_extendingSpeed * Time.deltaTime, 0));
                yield return null;
            }
        }

        yield return new WaitForSeconds(m_secondsEnlarged);
        StartCoroutine(ConstantRetraction());
    }
    IEnumerator ConstantRetraction()
    {
        if (m_moveForwardAtStart)
        {
            while (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) > m_pillar.GetStartTran().localPosition.y)
            {
                m_pillar.GetCurrentTran().Translate(new Vector3(0, -m_retractionSpeed * Time.deltaTime, 0));
                yield return null;
            }
        }
        else
        {
            while (m_pillar.GetCurrentTran().localPosition.y + (m_pillar.GetPillarTopTran().localPosition.y * m_pillar.GetMesh().transform.localScale.y) < m_pillar.GetStartTran().localPosition.y)
            {
                m_pillar.GetCurrentTran().Translate(new Vector3(0, m_retractionSpeed * Time.deltaTime, 0));
                yield return null;
            }
        }
        
        if(m_alwaysActive)
            StartCoroutine(ConstantExtension());
        m_pillarInMotion = false;
    }
}

