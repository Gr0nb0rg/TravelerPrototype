using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/21
*/

public class Ledge : MonoBehaviour {
    [SerializeField]
    private List<LedgeEdge> m_edges;
    [SerializeField]
    private List<LedgeVertex> m_vertices;

    public void AddEdge(LedgeEdge edge)
    {
        m_edges.Add(edge);
    }

    public void AddVertex(LedgeVertex vertex)
    {
        m_vertices.Add(vertex);
    }

    public bool ConnectFirstToLast()
    {
        if (m_edges.Count > 2 && m_vertices.Count > 2)
        {
            if ((m_edges[0].FirstTarget != null) && (m_edges[m_edges.Count - 1].LastTarget != null))
            {
                m_edges[0].SetFirstLeftNeighbour(m_edges[m_edges.Count - 1].LastTarget);
                m_edges[m_edges.Count - 1].SetLastRightNeighbour(m_edges[0].FirstTarget);
                return true;
            }   
        }
        return false;
    }

    public bool DisconnectFirstFromLast()
    {
        if ((m_edges[0].FirstTarget != null) && (m_edges[m_edges.Count - 1].LastTarget != null))
        {
            m_edges[0].SetFirstLeftNeighbour(null);
            m_edges[m_edges.Count - 1].SetLastRightNeighbour(null);
            return true;
        }
        return false;
    }

    /* DEBUG GIZMOS */
    void OnDrawGizmosSelected()
    {
        //DrawTriggerBoxes();
        DrawVertexPositions();
    }

    void DrawTriggerBoxes()
    {
        if (m_edges.Count > 0)
        {
            for (int i = 0; i < m_edges.Count; i++)
            {
                if(m_edges[i] != null)
                {
                    BoxCollider box = m_edges[i].Triggerbox;

                    Gizmos.color = new Color(1, 1, 0, 1);
                    Gizmos.DrawWireCube(m_edges[i].transform.localPosition + box.center, new Vector3(box.size.x * m_edges[i].transform.localScale.x, box.size.y * m_edges[i].transform.localScale.y, box.size.z * m_edges[i].transform.localScale.z));

                    Gizmos.color = new Color(1, 1, 0, 0.4f);
                    Gizmos.DrawCube(m_edges[i].transform.localPosition + box.center, new Vector3(box.size.x * m_edges[i].transform.localScale.x, box.size.y * m_edges[i].transform.localScale.y, box.size.z * m_edges[i].transform.localScale.z));
                }
            }
        }
    }

    void DrawVertexPositions()
    {
        if (m_vertices.Count > 0)
        {
            for (int i = 0; i < m_vertices.Count; i++)
            {
                if(m_vertices[i] != null)
                {
                    Vector3 point = m_vertices[i].transform.position;

                    Gizmos.color = new Color(1, 1, 0, 1);
                    Gizmos.DrawWireSphere(point, 0.1f);

                    Gizmos.color = new Color(1, 1, 0, 0.4f);
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
    }

    // Get & Set
    public List<LedgeEdge> Edges { get { return m_edges; } }
    public List<LedgeVertex> Vertices { get { return m_vertices; } }
}
