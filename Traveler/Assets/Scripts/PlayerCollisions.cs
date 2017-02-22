using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/21
*/

/*
    SUMMARY
    Handles all collisions with the player capsule collider
*/

[RequireComponent(typeof(ControllerPlayer))]
[RequireComponent(typeof(ControllerCheckpoint))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCollisions : MonoBehaviour {

    private ControllerPlayer m_controllerPlayer;
    private ControllerCheckpoint m_controllerCheckpoint;
    private CapsuleCollider m_playerCollider;
    private Rigidbody m_playerRigidbody;

    void Start ()
    {
        if((m_controllerPlayer = GetComponent<ControllerPlayer>()) == null)
            print("ControllerPlayer.cs not found");
        if ((m_controllerCheckpoint = FindObjectOfType<ControllerCheckpoint>()) == null)
            print("ControllerCheckpoint.cs not found");
        if ((m_playerCollider = GetComponent<CapsuleCollider>()) == null)
            print("Player has no collider attached");
        if ((m_playerRigidbody = GetComponent<Rigidbody>()) == null)
            print("Player has no rigidbody");
    }

    //void OnTriggerEnter(Collider other)
    //{

    //}


    //void OnTriggerExit(Collider other)
    //{

    //}

    //void OnCollisionEnter(Collision other)
    //{

    //}

}
