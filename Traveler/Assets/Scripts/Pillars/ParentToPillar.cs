using UnityEngine;
using System.Collections;

/*
    SUMMARY
    Parents Player to 'Pillar' object
    Same as PillarParenter, except this parents one level instead
*/

public class ParentToPillar : MonoBehaviour
{
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
        if (m_parentedCollider != null)
            m_parentedCollider.transform.parent = null;
    }

    void ParentPlayer(Collider player)
    {
        m_parentedCollider = player;
        m_parentedCollider.transform.parent = transform.parent;
    }
}
