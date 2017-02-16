using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/10
*/

/*
    SUMMARY
    Finds Interactable targets and activates them
    Place once in scene 
    Use ClearActiveInteractables() & FindActiveInteractables() when switching scenes
*/

public class InteractableFinder : MonoBehaviour {

    // Settings and tweaking varaibles
    [SerializeField]
    private KeyCode m_activationKey;
    [SerializeField]
    private float m_range;
    [SerializeField]
    private int m_skipThisManyFrames = 5;
    [SerializeField]
    private LayerMask m_obstacleLayer;

    // Used to indicate reachable and selected objects
    [SerializeField]
    private Material m_foundMat;
    [SerializeField]
    private Material m_defMat;
    [SerializeField]
    private Material m_closestMat;

    // Active interractable objects
    private List<Renderer> m_interactableRend = new List<Renderer>();
    private Interactable[] m_activeInteractables;

    // Component variables
    private GameObject m_player;
    private Camera m_mainCam;

    // Reference values
    private Vector2 m_mainCamCenter;

    // Update state variables
    private FindingType m_currentFindingType = FindingType.NearCenter;
    private int m_framesCounter = 0;

    // Interactable finding bookkeeping
    private InteractableTarget m_closestTarget;
    private float m_closestDistToMid;
    private int m_closestIndex = 0;
    private int m_closestTargetIndex = 0;

    public enum FindingType{ NearCenter }

    void Start () {
        m_mainCam = Camera.main;
        m_mainCamCenter = new Vector2(m_mainCam.pixelWidth / 2, m_mainCam.pixelHeight / 2);
        m_player = GameObject.FindGameObjectWithTag("Player");
        FindActiveInteractables();
    }

    void Update()
    {
        // Update at at necessary rate
        m_framesCounter++;
        if (m_framesCounter % m_skipThisManyFrames == 0)
            FindInteractable();

        ListenForActivation();
    }

    // Activate closest reachable Interactable when player presses 'e'
    void ListenForActivation()
    {
        if (Input.GetKeyDown(m_activationKey))
            if (m_activeInteractables.Length > 0 && Vector3.Distance(m_player.transform.position, m_closestTarget.transform.position) < m_range)
                m_activeInteractables[m_closestIndex].GetComponent<AbstractInteractable>().Interact();
    }

    void FindInteractable()
    {
        switch (m_currentFindingType)
        {
            case FindingType.NearCenter:
                FindInteractableNearCenter();
                break;
            default:
                break;
        }
    }

    // Use following functions when switching scenes
    void FindActiveInteractables()
    {
        m_activeInteractables = GameObject.FindObjectsOfType<Interactable>();
        for (int i = 0; i < m_activeInteractables.Length; i++)
        {
            m_interactableRend.Add(m_activeInteractables[i].gameObject.GetComponentInChildren<MeshRenderer>());
        }
        InteractableTarget[] firstTargets = m_activeInteractables[0].GetTargets();
        m_closestTarget = firstTargets[0];
    }

    void ClearActiveInteractables()
    {
        m_activeInteractables = null;
        m_interactableRend.Clear();
    }

    // Sets m_closestTarget to reachable unobstructed target closest to center of view
    void FindInteractableNearCenter(){
        if (m_activeInteractables != null)
        {
            for (int i = 0; i < m_activeInteractables.Length; i++)
            {
                InteractableTarget[] targets = m_activeInteractables[i].GetTargets();
                for (int j = 0; j < targets.Length; j++)
                {
                    // Target in front of camera?
                    if (m_mainCam.WorldToViewportPoint(targets[j].transform.position).z > 0)
                    {
                        // Target within range?
                        if (Vector3.Distance(m_player.transform.position, targets[j].transform.position) < m_range)
                        {
                            // Target obstructed?
                            if (!Physics.Linecast(m_mainCam.transform.position, targets[j].transform.position, m_obstacleLayer))
                            {
                                Debug.DrawLine(m_mainCam.transform.position, targets[j].transform.position, Color.red);
                                if (m_closestIndex == i)
                                {
                                    m_interactableRend[i].material = m_closestMat;
                                }
                                else
                                {
                                    // Find the Interactables closest to camera center
                                    float distToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(targets[j].transform.position), m_mainCamCenter);
                                    m_closestDistToMid = Vector2.Distance(m_mainCam.WorldToScreenPoint(m_closestTarget.transform.position), m_mainCamCenter);
                                    if (distToMid < m_closestDistToMid)
                                    {
                                        m_closestIndex = i;
                                        m_closestTargetIndex = j;
                                        m_closestTarget = targets[j];
                                        m_interactableRend[m_closestIndex].material = m_closestMat;
                                    }
                                    else
                                    {
                                        m_interactableRend[i].material = m_foundMat;
                                    }
                                }
                            }
                            else
                            {
                                m_interactableRend[i].material = m_defMat;
                            }
                        }
                        else
                        {
                            m_interactableRend[i].material = m_defMat;
                        }
                    }
                    else
                    {
                        m_interactableRend[i].material = m_defMat;
                    }
                }
            }
        }
    }

    // Set functions
    void SetFindingType(FindingType type) { m_currentFindingType = type; }
}