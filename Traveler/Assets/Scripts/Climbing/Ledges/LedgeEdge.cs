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
    Collects all points for a ledge collider
*/


[RequireComponent(typeof(BoxCollider))]
public class LedgeEdge : MonoBehaviour
{
    // PRIVATE SERALIZABLES
    //[SerializeField]
    //private bool m_refreshTargets = false;
    [HideInInspector]
    public enum TargetType{ CORNER, MID }

    // PRIVATE
    [SerializeField]
    private List<ClimbTarget> m_targets;
    [SerializeField]
    private LedgeVertex m_vertexClosestToOrigin;
    [SerializeField]
    private LedgeVertex m_vertexFurthestFromOrigin;
    [SerializeField]
    private float m_facingDirection = 1;

    void Awake()
    {
        //FindAndRefreshTargets();
    }

    public void SetLastRightNeighbour(ClimbTarget neighbour)
    {
        if (m_targets.Count > 0)
            m_targets[m_targets.Count - 1].RightNeigbour = neighbour;
    }

    public void SetFirstLeftNeighbour(ClimbTarget neighbour)
    {
        if (m_targets.Count > 0)
            m_targets[0].LeftNeigbour = neighbour;
    }

    public void UpdateClimbTargets(float distBetweenTargets)
    {
        float distanceBetweenVertices = Vector3.Distance(VertexClosestToOrigin.transform.position, VertexFurthestFromOrigin.transform.position);
        int totalTargetNeed = (int)Mathf.Floor(distanceBetweenVertices / distBetweenTargets);
        int targetDelta = totalTargetNeed - m_targets.Count;

        // Delete targets to fit shorter distance
        if(targetDelta < 0)
        {
            for (int i = m_targets.Count-1; i > (totalTargetNeed); i--)
            {
                m_targets.RemoveAt(i);
                GameObject destructionTarget;
                if ((destructionTarget = transform.Find("ClimbTarget " + (i+1)).gameObject) != null)
                {
                    DestroyImmediate(destructionTarget);
                }
            }
        }
        // Create enough targets to fill distance
        if (targetDelta > 0)
        {
            for (int i = 0; i < targetDelta; i++)
            {
                if (!CreateClimbTarget(TargetType.MID))
                    Debug.Log("CouldNotPlaceTarget");
            }
        }
        SetFurthestOutNeighbours();
        PlaceClimbTargets();
    }

    public void SetFurthestOutNeighbours()
    {
        if (m_targets.Count > 0)
        {
            LedgeVertex furthestOutVert = null;
            LedgeEdge forwardNeigbourEdge = null;
            if ((furthestOutVert = VertexFurthestFromOrigin) != null)
            {
                if((forwardNeigbourEdge = furthestOutVert.EdgeFurthestFromOrigin) != null)
                {
                    ClimbTarget furthestInOnNeighbourEdge;
                    if ((furthestInOnNeighbourEdge = forwardNeigbourEdge.FirstTarget) != null)
                    {
                        ClimbTarget furthestOutTarget = LastTarget;
                        furthestOutTarget.RightNeigbour = furthestInOnNeighbourEdge;
                        furthestInOnNeighbourEdge.LeftNeigbour = furthestOutTarget;
                    }
                }
            } 
        }
    }

    public void PlaceClimbTargets()
    {
        if(m_targets.Count > 0)
        {
            // Place targets along line
            float currentDistance = 0;
            float incrementAmount = (1f / m_targets.Count);
            Vector3 closestToFurthestDirection = (VertexFurthestFromOrigin.transform.position - VertexClosestToOrigin.transform.position);

            for (int i = 0; i < m_targets.Count; i++)
            {
                //m_targets[i].transform.position = GetVertexClosestToOrigin().transform.position + ();
                Debug.DrawRay(VertexClosestToOrigin.transform.position, closestToFurthestDirection, Color.red);
                //Debug.DrawLine(m_targets[i].transform.position, GetVertexClosestToOrigin().transform.position + (closestToFurthestDirection * 0.5f), Color.red);
                m_targets[i].transform.position = VertexClosestToOrigin.transform.position + (closestToFurthestDirection * (incrementAmount * i));
                currentDistance += incrementAmount;
            }
        }
    }

    public bool CreateClimbTarget(TargetType type)
    {
        GameObject instance = null;
        // Rot = transform.rot * facingDirection
        switch (type)
        {
            case TargetType.CORNER:
                instance = Instantiate(Resources.Load("ClimbTargetCorner", typeof(GameObject))) as GameObject;
                instance.transform.parent = transform;
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
                //instance.transform.LookAt(instance.transform.position);
                break;
            case TargetType.MID:
                instance = Instantiate(Resources.Load("ClimbTargetMid", typeof(GameObject))) as GameObject;
                instance.transform.parent = transform;
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
                break;
            default:
                break;
        }
        if(instance != null)
        {
            instance.name = "ClimbTarget " + (m_targets.Count + 1);
            ClimbTarget target = instance.GetComponent<ClimbTarget>();
            if(m_targets.Count > 0) { 
                m_targets[m_targets.Count - 1].RightNeigbour = target;
                target.LeftNeigbour = m_targets[m_targets.Count - 1];
            }
            else{
                LedgeEdge previousEdge = null;
                if ((previousEdge = VertexClosestToOrigin.EdgeClosestToOrigin) != null)
                {
                    if(previousEdge.m_targets.Count > 0)
                    {
                        target.LeftNeigbour = previousEdge.m_targets[previousEdge.m_targets.Count - 1];
                        previousEdge.m_targets[previousEdge.m_targets.Count - 1].RightNeigbour = target;
                    }
                }
            }
            m_targets.Add(target);
            return true;
        }
        return false;
    }

    public void PlaceEdgeBetweenVertices(float facingDirection)
    {
        Vector3 pointBetweenVertices = VertexClosestToOrigin.transform.position + (VertexFurthestFromOrigin.transform.position - VertexClosestToOrigin.transform.position) * 0.5f;

        transform.position = pointBetweenVertices;
        //Debug.DrawRay(pointBetweenVertices, Vector3.Cross(GetVertexFurthestFromOrigin().transform.position - GetVertexClosestToOrigin().transform.position, Vector3.up), Color.red);
        transform.LookAt(pointBetweenVertices + Vector3.Cross(VertexFurthestFromOrigin.transform.position - VertexClosestToOrigin.transform.position, Vector3.up) * facingDirection);
    }

    public void FitTriggerBox(float facingDirection, float height, float depth)
    {
        BoxCollider col = Triggerbox;
        float newX = Vector3.Distance(m_vertexClosestToOrigin.transform.position, m_vertexFurthestFromOrigin.transform.position);

        col.size = new Vector3(newX, height, depth);
        col.center = new Vector3(col.center.x, col.center.y, col.center.z);
    }

    void OnDrawGizmosSelected()
    {
        BoxCollider box = Triggerbox;
        Gizmos.matrix = transform.localToWorldMatrix;


        Gizmos.color = new Color(1, 1, 0, 1);
        Gizmos.DrawWireCube(Vector3.zero, box.size);

        Gizmos.color = new Color(1, 1, 0, 0.4f);
        Gizmos.DrawCube(Vector3.zero, box.size);
    }

    public LedgeVertex VertexClosestToOrigin{ get { return m_vertexClosestToOrigin; } set { m_vertexClosestToOrigin = value; } }
    public LedgeVertex VertexFurthestFromOrigin { get { return m_vertexFurthestFromOrigin; } set { m_vertexFurthestFromOrigin = value; } }

    public BoxCollider Triggerbox { get{ return GetComponent<BoxCollider>(); } }

    public ClimbTarget LastTarget { get { ClimbTarget retval = (m_targets.Count > 0) ? m_targets[m_targets.Count - 1] : null; return retval; } }
    public ClimbTarget FirstTarget { get { ClimbTarget retval = (m_targets.Count > 0) ? m_targets[0] : null; return retval; } }

    // Find climb target closest to specified point in collection
    public ClimbTarget GetClosestTarget(Vector3 closestToThisPos)
    {
        if (m_targets.Count > 0)
        {
            float shortestDist = Mathf.Infinity;
            int shortestDistIndex = 0;
            for (int i = 0; i < m_targets.Count; i++)
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
