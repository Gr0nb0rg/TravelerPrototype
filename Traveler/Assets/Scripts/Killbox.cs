using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Killbox : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ControllerCheckpoint>())
            col.gameObject.GetComponent<ControllerCheckpoint>().SetPlayerToCurrent();
    }

    void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();

        Gizmos.color = new Color(1,0,0,1);
        Gizmos.DrawWireCube(transform.position + collider.center, new Vector3(collider.size.x * transform.localScale.x, collider.size.y * transform.localScale.y, collider.size.z * transform.localScale.z));

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(transform.position + collider.center, new Vector3(collider.size.x * transform.localScale.x, collider.size.y * transform.localScale.y, collider.size.z * transform.localScale.z));
    }

    void OnDrawGizmosSelected()
    {
        BoxCollider collider = GetComponent<BoxCollider>();

        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireCube(transform.position + collider.center, new Vector3(collider.size.x * transform.localScale.x, collider.size.y * transform.localScale.y, collider.size.z * transform.localScale.z));

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawCube(transform.position + collider.center, new Vector3(collider.size.x * transform.localScale.x, collider.size.y * transform.localScale.y, collider.size.z * transform.localScale.z));
    }
}
