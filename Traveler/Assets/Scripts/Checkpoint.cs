using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public int m_ID = 0;

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponentInParent<ControllerCheckpoint>())
        {
            col.GetComponentInParent<ControllerCheckpoint>().SetCurrent(m_ID);
        }
    }
}
