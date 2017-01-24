using UnityEngine;
using System.Collections;
using System.Threading;

public class Arrow : MonoBehaviour
{
    private float velocity;
    private float dist;
    private float removeTime = 0;
    private Vector3 spawnPoint;
    private bool collided = false;

    public float Velocity
    {
        set { velocity = value; }
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.position += velocity*transform.forward*Time.fixedDeltaTime*50;   
        //CheckDistance();

        if (!collided) return;

        Debug.Log("bababoi");
        removeTime += Time.fixedDeltaTime;
        if (removeTime >= 10)
            Destroy(this.gameObject);
    }

    void CheckDistance()
    {
     
        dist += velocity;
        if (!(dist >= 5)) return;

        
        dist = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        collided = true;
        velocity = 0f;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
