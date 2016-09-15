using UnityEngine;
using System.Collections;

public class ControllerCamera : MonoBehaviour
{
    //Public vars
    public Vector3 m_Offset;
    public Vector2 m_Sensitivity = new Vector2(5, 5);
    public float m_ClampY = 80;

    //Player vars
    Transform m_Player;

    //Rotation vars
    float m_AbsoluteY;

	void Start ()
    {
        m_Player = GameObject.Find("Player").transform;
	}
	
	void Update ()
    {
        Cursor.lockState = CursorLockMode.Locked;
	}

    void LateUpdate()
    {
        //Get input values
        Vector2 input = new Vector2(Input.GetAxis("Mouse X") * m_Sensitivity.x, Input.GetAxis("Mouse Y") * m_Sensitivity.y);

        //Rotate player
        m_Player.Rotate(0, input.x, 0);

        //Get Y mouse input and clamp it
        m_AbsoluteY += input.y;
        m_AbsoluteY = Mathf.Clamp(m_AbsoluteY, -m_ClampY, m_ClampY);

        //Get player rotation and set camera rotation/position relative to Y input and player rotation
        float desired = m_Player.eulerAngles.y;
        Quaternion rot = Quaternion.Euler(m_AbsoluteY, desired, 0);
        transform.position = m_Player.transform.position - (rot * m_Offset);

        transform.LookAt(m_Player);
    }
}
