using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{
    //Public vars
    public Vector3 m_Offset;

    //Player vars
    Transform m_Player;

	void Start ()
    {
        m_Player = GameObject.Find("Player").transform;
	}
	
	void Update ()
    {
        transform.position = m_Player.transform.position + m_Offset;
	}
}
