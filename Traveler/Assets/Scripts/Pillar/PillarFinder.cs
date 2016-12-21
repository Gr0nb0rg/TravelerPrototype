using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PillarFinder : MonoBehaviour {

    public float m_pillarRange;
    public float m_updateFrequency;
    public LayerMask m_obstacleLayer;
    public Material m_foundMat;
    public Material m_defMat;
    public Material m_closestMat;

    private List<Renderer> m_activePillarsRend = new List<Renderer>();
    // TEMPORARY: Remove when pillars with piviot at intended top are imported
    private List<Transform> m_activePillarTops = new List<Transform>();
    GameObject[] m_activePillars;

    private GameObject m_player;
    private Camera m_mainCam;
    private Vector2 m_mainCamCenter;
    private string m_currentFindingType = "FindPillarNearCenter";
    private float m_closestPillarDistToMid;
    private int m_closestPillarIndex = 0;

    public enum FindingType{
        NearCenter
    }

    void Start () {
        m_mainCam = Camera.main;
        m_mainCamCenter = new Vector2(m_mainCam.pixelWidth / 2, m_mainCam.pixelHeight / 2);
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activePillars = GameObject.FindGameObjectsWithTag("Pillar");
        for (int i = 0; i < m_activePillars.Length; i++)
        {
            m_activePillarsRend.Add(m_activePillars[i].GetComponentInChildren<Renderer>());
            m_activePillarTops.Add(m_activePillars[i].transform.FindChild("PillarTop"));
        }
        InvokeRepeating(m_currentFindingType, 0.0f, m_updateFrequency);
    }
	
	void Update () {
        // Activate closest reachable pillar when player presses 'e'
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (m_activePillars.Length > 0 && Vector3.Distance(m_player.transform.position, m_activePillarTops[m_closestPillarIndex].transform.position) < m_pillarRange)
            {
                m_activePillars[m_closestPillarIndex].GetComponentInParent<PillarTranslator>().ActivatePillar();
            }
        }

    }

    void SetFindingType(string findingType)
    {
        CancelInvoke(m_currentFindingType);
        switch (findingType)
        {
            case "FindPillarNearCenter":
                InvokeRepeating(findingType, 0.0f, m_updateFrequency);
                break;
            default:
                print("Invalid finding type");
                return;
        }
        m_currentFindingType = findingType;
    }

    public GameObject GetClosestPillar()
    {
        return m_activePillars[m_closestPillarIndex];
    }

    void FindPillarNearCenter(){
        for (int i = 0; i < m_activePillarsRend.Count; i++)
        {
            // Find pillars within main cam frustum
            if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(m_mainCam), m_activePillarsRend[i].bounds))
            {
                // Find pillars within range
                if (Vector3.Distance(m_player.transform.position, m_activePillarTops[i].transform.position) < m_pillarRange)
                {
                    // Find unobstructed pillars
                    
                    if (!Physics.Linecast(m_mainCam.transform.position, m_activePillarTops[i].transform.position, m_obstacleLayer))
                    {
                        Debug.DrawLine(m_mainCam.transform.position, m_activePillarTops[i].position, Color.red);
                        if (m_closestPillarIndex != i)
                        {
                            // Find the pillar closest to camera center
                            float distToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(m_activePillarTops[i].transform.position), m_mainCamCenter);
                            m_closestPillarDistToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(m_activePillarTops[m_closestPillarIndex].transform.position), m_mainCamCenter);
                            if (distToMid < m_closestPillarDistToMid)
                            {
                                m_closestPillarIndex = i;
                                m_activePillarsRend[m_closestPillarIndex].material = m_closestMat;
                            }
                            else
                            {
                                m_activePillarsRend[i].material = m_foundMat;
                            }
                        }
                        else
                        {
                            m_activePillarsRend[i].material = m_closestMat;
                        }
                    }
                    else
                    {
                        m_activePillarsRend[i].material = m_defMat;
                    }
                }
                else
                {
                    m_activePillarsRend[i].material = m_defMat;
                }
            }
            else
            {
                m_activePillarsRend[i].material = m_defMat;
            }
        }
    }
}
