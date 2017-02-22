using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/21
*/

public class LedgeVertex : MonoBehaviour {
    //[Header("Do not assign these manually")]
    [HideInInspector]
    [SerializeField]
    private LedgeVertex m_rightNeighbour = null;
    [HideInInspector]
    [SerializeField]
    private LedgeVertex m_leftNeighbour = null;
    [HideInInspector]
    [SerializeField]
    private LedgeEdge m_edgeFurthestFromOrigin = null;
    [HideInInspector]
    [SerializeField]
    private LedgeEdge m_edgeClosestToOrigin = null;

    void OnDrawGizmosSelected()
    {
        DrawSelf();
        DrawNeighbours();
    }

    void DrawSelf()
    {
        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireSphere(transform.position, 0.1f);

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    void DrawNeighbours()
    {
        if(m_rightNeighbour != null)
        {
            Gizmos.color = new Color(1, 1, 0, 1);
            Gizmos.DrawLine(transform.position, m_rightNeighbour.transform.position);
            Gizmos.DrawWireSphere(m_rightNeighbour.transform.position, 0.1f);

            Gizmos.color = new Color(1, 1, 0, 0.4f);
            Gizmos.DrawSphere(m_rightNeighbour.transform.position, 0.1f);
        }
        if (m_leftNeighbour != null)
        {
            Gizmos.color = new Color(1, 1, 0, 1);
            Gizmos.DrawLine(transform.position, m_leftNeighbour.transform.position);
            Gizmos.DrawWireSphere(m_leftNeighbour.transform.position, 0.1f);

            Gizmos.color = new Color(1, 1, 0, 0.4f);
            Gizmos.DrawSphere(m_leftNeighbour.transform.position, 0.1f);
        }
    }

    // GET & SET

    // Neighbour vertices
    public LedgeVertex RightNeighbour { get { return m_rightNeighbour; } set { m_rightNeighbour = value; } }
    public LedgeVertex LeftNeighbour { get { return m_leftNeighbour; } set { m_leftNeighbour = value; } }
    
    // Connected edges

    public LedgeEdge EdgeFurthestFromOrigin { get{ return m_edgeFurthestFromOrigin; } set { m_edgeFurthestFromOrigin = value; } }
    public LedgeEdge EdgeClosestToOrigin { get{ return m_edgeClosestToOrigin; } set { m_edgeClosestToOrigin = value; } }
}
