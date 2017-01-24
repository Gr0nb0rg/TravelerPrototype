using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
[ExecuteInEditMode]
public class LoadTrigger : MonoBehaviour
{

    public string loadName;
    public string unloadName;
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
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), dir * 2.0f, Color.red);
        }
    }

    #endif

    private void OnTriggerEnter(Collider col)
    {
        if (loadName != "")
            StartCoroutine("LoadScene");
            

        if (unloadName != "")
            StartCoroutine("UnloadScene");
    }

    IEnumerator LoadScene()
    {
        LoadManager.Instance.Load(loadName);
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator UnloadScene()
    {
        
        LoadManager.Instance.Unload(unloadName);
        yield return new WaitForSeconds(0.1f);
    }
}
