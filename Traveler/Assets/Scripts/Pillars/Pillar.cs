using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/10
*/

/*
    SUMMARY
    Collects information for a pillar that can be used by functions that manipulate it
*/

public class Pillar : MonoBehaviour {

    // Reference positions
    private Transform m_maxTran;
    private Transform m_startTran;
    private Transform m_currentTran;
    private Transform m_meshTran;
    private Transform m_pillarTop;

    // Components
    private Rigidbody m_rigidbody;
    private BoxCollider m_standingArea;
    private MeshRenderer m_mesh;

    void Awake()
    {
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        m_mesh = GetComponentInChildren<MeshRenderer>();
        m_maxTran = transform.FindChild("TargetPos");
        m_startTran = transform.FindChild("StartPos");
        m_currentTran = transform.FindChild("Pillar");
        m_meshTran = m_currentTran.transform.FindChild("Mesh");
        m_pillarTop = m_meshTran.transform.FindChild("PillarTop");
        m_startTran.position = m_pillarTop.transform.position;
    }

    // Get functions
    public Rigidbody GetRigidbody(){ return m_rigidbody; }
    public MeshRenderer GetMesh() { return m_mesh; }
    public Transform GetTargetTran() { return m_maxTran; }
    public Transform GetStartTran() { return m_startTran; }
    public Transform GetPillarTopTran() { return m_pillarTop; }
    public Transform GetCurrentTran() { return m_currentTran; }

    // Set functions
    public void SetCurrentTran(Transform newTran) { m_currentTran = newTran; }
}
