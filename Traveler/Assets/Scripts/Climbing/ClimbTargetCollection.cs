using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/04
*/

/*
    SUMMARY
    Collects all points for a ledge collider
*/

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class ClimbTargetCollection : MonoBehaviour
{
    // PRIVATE SERALIZABLES
    [SerializeField]
    private bool m_refreshTargets = false;

    // PRIVATE
    private ClimbTarget[] m_targets;


    void Start()
    {
        FindAndRefreshTargets();
    }

    #if UNITY_EDITOR
    void Update()
    {
        if (m_refreshTargets)
        {
            FindAndRefreshTargets();
            m_refreshTargets = false;
        }
    }
    #endif

    void FindAndRefreshTargets()
    {
        m_targets = GetComponentsInChildren<ClimbTarget>();
        for (int i = 0; i < m_targets.Length; i++)
        {
            m_targets[i].m_refreshPoints = true;
        }
    }

    // Find climb target closest to specified point in collection
    public ClimbTarget GetClosestTarget(Vector3 closestToThisPos)
    {
        if (m_targets.Length > 0)
        {
            float shortestDist = Mathf.Infinity;
            int shortestDistIndex = 0;
            for (int i = 0; i < m_targets.Length; i++)
            {
                float dist = Vector3.Distance(closestToThisPos, m_targets[i].transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    shortestDistIndex = i;
                }
            }
            return m_targets[shortestDistIndex];
        }
        else
            return null;
    }
}
