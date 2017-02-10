using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/10
*/

/*
    SUMMARY
    Handles all collisions with the player capsule collider
*/

public class PlayerCollisions : MonoBehaviour {

    private ControllerCheckpoint m_controllerCheckpoint;
    private CapsuleCollider m_playerCollider;
    private Rigidbody m_playerRigidbody;

    void Start ()
    {
        if ((m_controllerCheckpoint = FindObjectOfType<ControllerCheckpoint>()) == null)
            print("Controller checkpoint not found");
        if ((m_playerCollider = GetComponent<CapsuleCollider>()) == null)
            print("Player has no collider attached");
        if ((m_playerRigidbody = GetComponent<Rigidbody>()) == null)
            print("Player has no rigidbody");
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Killbox":
                // Stop and place player at current checkpoint
                m_playerRigidbody.velocity = Vector3.zero;
                transform.position = m_controllerCheckpoint.GetCurrentCheckpoint().position - new Vector3(0, (m_playerCollider.bounds.size.y/2), 0);
                break;
            default:
                break;
        }
    }

    //void OnTriggerExit(Collider other)
    //{

    //}
}
