using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class LoadTrigger : MonoBehaviour
{

    public string loadName;
    public string unloadName;

    private void OnTriggerEnter(Collider col)
    {
        if(loadName != "")
            LoadManager.Instance.Load(loadName);

        if (unloadName != "")
            StartCoroutine("UnloadScene");
    }

    IEnumerator UnloadScene()
    {
        
        LoadManager.Instance.Unload(unloadName);
        yield return new WaitForSeconds(0f);
    }
}
