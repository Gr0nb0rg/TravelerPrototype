using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour {
    

    private float pullForce = 0.01f;
    private float maxForce = 1f;
    private float arrowSpeed = 0.0f;
    private float forceTime = 0.0f;
    private float maxForceTime = 5000f;

    private bool steadyAim = true;
    private bool bowLoaded = false;

    bool m_IsPaused = false;

    public GameObject arrow;
	void Start () {
	
	}
	
	void Update () {
        if (!m_IsPaused)
	        CheckBow();
	}

    private void CheckBow()
    {
        if (Input.GetMouseButton(0))
        {
            if (!bowLoaded)
                bowLoaded = true;

            if (pullForce < maxForce)
            {
                pullForce += 0.01f*Time.fixedDeltaTime;
            }
        }

        if (!Input.GetMouseButtonUp(0) || !bowLoaded) return;

        //Create arrow here - may need cleaning...
        Debug.Log(pullForce);
        var player = GameObject.FindGameObjectWithTag("Player");
        var a = Instantiate(arrow);
        a.transform.position = transform.position + transform.forward * 1.2f;
        a.transform.forward = new Vector3(player.transform.forward.x, -Camera.main.transform.rotation.x, player.transform.forward.z);
        a.GetComponent<Arrow>().Velocity = (pullForce * 10);
        pullForce = 0.02f;
    }

    public void SetPaused(bool state)
    {
        m_IsPaused = state;
    }
}
