using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableFinder : MonoBehaviour {

    public float m_range;
    public float m_updateFrequency;
    public LayerMask m_obstacleLayer;
    public Material m_foundMat;
    public Material m_defMat;
    public Material m_closestMat;

    private List<Renderer> m_interactableRend = new List<Renderer>();
    // TEMPORARY: Not needed when object piviots are set to raycast target
    private List<Transform> m_raycastTargets = new List<Transform>();
    GameObject[] m_activeInteractables;

    private GameObject m_player;
    private Camera m_mainCam;
    private Vector2 m_mainCamCenter;
    private string m_currentFindingType = "FindInteractableNearCenter";
    private float m_closestDistToMid;
    private int m_closestIndex = 0;

    public enum FindingType{
        NearCenter
    }

    void Start () {
        m_mainCam = Camera.main;
        m_mainCamCenter = new Vector2(m_mainCam.pixelWidth / 2, m_mainCam.pixelHeight / 2);
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeInteractables = GameObject.FindGameObjectsWithTag("Pillar");
        for (int i = 0; i < m_activeInteractables.Length; i++)
        {
            m_interactableRend.Add(m_activeInteractables[i].GetComponent<Renderer>());
            m_raycastTargets.Add(m_activeInteractables[i].transform.FindChild("PillarTop"));
        }
        InvokeRepeating(m_currentFindingType, 0.0f, m_updateFrequency);
    }
	
	void Update () {
        // Activate closest reachable Interactable when player presses 'e'
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (m_activeInteractables.Length > 0 && Vector3.Distance(m_player.transform.position, m_raycastTargets[m_closestIndex].transform.position) < m_range)
            {
                m_activeInteractables[m_closestIndex].GetComponentInParent<AbstractInteractable>().Interact();
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

    public GameObject GetClosestInteractable()
    {
        return m_activeInteractables[m_closestIndex];
    }

    void FindInteractableNearCenter(){
        for (int i = 0; i < m_interactableRend.Count; i++)
        {
            // Find Interactables objects within main cam frustum
            if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(m_mainCam), m_interactableRend[i].bounds))
            {
                // Find Interactables within range
                if (Vector3.Distance(m_player.transform.position, m_raycastTargets[i].transform.position) < m_range)
                {
                    // Find unobstructed Interactables
                    if (!Physics.Linecast(m_mainCam.transform.position, m_raycastTargets[i].transform.position, m_obstacleLayer))
                    {
                        Debug.DrawLine(m_mainCam.transform.position, m_raycastTargets[i].position, Color.red);
                        if (m_closestIndex != i)
                        {
                            // Find the Interactables closest to camera center
                            float distToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(m_raycastTargets[i].transform.position), m_mainCamCenter);
                            m_closestDistToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(m_raycastTargets[m_closestIndex].transform.position), m_mainCamCenter);
                            if (distToMid < m_closestDistToMid)
                            {
                                m_closestIndex = i;
                                m_interactableRend[m_closestIndex].material = m_closestMat;
                            }
                            else
                            {
                                m_interactableRend[i].material = m_foundMat;
                            }
                        }
                        else
                        {
                            m_interactableRend[i].material = m_closestMat;
                        }
                    }
                    else
                    {
                        m_interactableRend[i].material = m_defMat;
                    }
                }
            }
            else
            {
                m_interactableRend[i].material = m_defMat;
            }
        }
    }
}
