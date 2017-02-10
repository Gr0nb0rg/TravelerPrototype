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
    Collects all targets that are used by InteractableFinder.cs when selecting a object
*/

public class Interactable : MonoBehaviour {
    InteractableTarget[] m_targets;
	void Awake () {
        if ((m_targets = GetComponentsInChildren<InteractableTarget>()) == null)
            print("Interactable has no targets");
    }

    public InteractableTarget[] GetTargets(){
        return m_targets;
    }
}
