using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractInteractable : MonoBehaviour {
    public abstract void Interact();
    public abstract void Signal();
}
