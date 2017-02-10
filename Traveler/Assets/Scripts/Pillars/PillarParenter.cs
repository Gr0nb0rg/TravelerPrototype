using UnityEngine;
using System.Collections;

/*
    Authors: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/10
*/

/*
    SUMMARY
    Parents Player to 'Pillar' object
*/

public class PillarParenter : MonoBehaviour {
    private Collider m_parentedCollider = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            ParentPlayer(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            DeparentPlayer();
        }
    }

    public void DeparentPlayer()
    {
        if(m_parentedCollider != null)
            m_parentedCollider.transform.parent = null;
    }

    void ParentPlayer(Collider player)
    {
        m_parentedCollider = player;
        m_parentedCollider.transform.parent = transform.parent.parent;
    }
}
