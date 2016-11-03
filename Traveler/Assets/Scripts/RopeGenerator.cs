using UnityEngine;
using System.Collections;

public class RopeGenerator : MonoBehaviour
{
    private Vector3 fromPos;
    private Vector3 toPos;

    private GameObject arrow;

    public GameObject ropePart;

    public GameObject Arrow
    {
        set { arrow = value; }
    }

    public Vector3 FromPos
    {
        set { fromPos = value; }
    }

    public Vector3 ToPos
    {
        set { toPos = value; }
    }

    void Start () {
	
	}

	void Update () {
	
	}

    public void Spawn()
    {
        var link = Instantiate(ropePart);
        //link.transform.position = 
    }
}
