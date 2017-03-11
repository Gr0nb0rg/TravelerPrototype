using UnityEngine;

public class TempParenter : MonoBehaviour
{
    private Collider m_parentedCollider = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("Parented");
            ParentPlayer(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("UnParented");
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
        Debug.Log("parented to: " + transform.name);
    }
}
