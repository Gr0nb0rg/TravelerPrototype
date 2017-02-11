using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[ExecuteInEditMode]
public class Killbox : MonoBehaviour
{
#if UNITY_EDITOR

    void Update()
    {
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0:
                    dir = transform.up;
                    break;
                case 1:
                    dir *= -1;
                    break;
                case 2:
                    dir = transform.right;
                    break;
                case 3:
                    dir *= -1;
                    break;
                case 4:
                    dir = transform.forward;
                    break;
                case 5:
                    dir *= -1;
                    break;
            }
            Debug.DrawRay(transform.position, dir * 3.0f, Color.blue);
        }
    }

#endif

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ControllerCheckpoint>())
            col.gameObject.GetComponent<ControllerCheckpoint>().SetPlayerToCurrent();
    }
}
