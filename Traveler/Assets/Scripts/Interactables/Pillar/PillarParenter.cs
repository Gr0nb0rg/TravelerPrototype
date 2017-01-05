using UnityEngine;
using System.Collections;

// Parents Player to 'Pillar' object in 
public class PillarParenter : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.tag == "Player")
        {
            other.transform.parent.parent = transform.parent.parent;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.tag == "Player")
        {
            other.transform.parent.parent = null;
        }
    }
}
