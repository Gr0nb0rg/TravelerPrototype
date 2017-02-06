using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/04
*/

/*
    SUMMARY
    Handles all collisions with the player capsule collider
*/

public class PlayerCollisions : MonoBehaviour {

    void Start ()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Killbox":
                // Place player at current checkpoint
                break;
            default:
                break;
        }
    }

    //void OnTriggerExit(Collider other)
    //{

    //}
}
