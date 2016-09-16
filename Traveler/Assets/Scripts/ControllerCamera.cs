using UnityEngine;
using System.Collections;

public class ControllerCamera : MonoBehaviour
{
    //Public vars
    public LayerMask m_ZoomMask;
    public Vector3 m_Offset;
    public Vector2 m_Sensitivity = new Vector2(5, 5);
    public bool m_InvertY = false;
    public float m_ClampY = 80;
    public float m_PlayerDistance = 2.0f;

    //Player vars
    ControllerPlayer m_Player;

    //Rotation vars
    float m_AbsoluteY;
    int m_InvertVal = 1;

    //Zoom vars
    float m_MaxZ;
    RaycastHit hit;
    Vector3 m_DesiredPosition;
    Vector3 m_StartOffset;

	void Start ()
    {
        m_MaxZ = m_Offset.z;
        m_StartOffset = m_Offset;
        m_Player = GameObject.Find("Player").GetComponent<ControllerPlayer>();
	}
	
	void Update ()
    {
        Cursor.lockState = CursorLockMode.Locked;
	}

    void LateUpdate()
    {
        //Get input values
        if (m_InvertY)
            m_InvertVal = 1;
        else
            m_InvertVal = -1;
        m_InvertVal = Mathf.Clamp(m_InvertVal, -1, 1);
        Vector2 input = new Vector2(Input.GetAxis("Mouse X") * m_Sensitivity.x, Input.GetAxis("Mouse Y") * m_InvertVal * m_Sensitivity.y);

        //Rotate player
        m_Player.transform.Rotate(0, input.x, 0);

        m_AbsoluteY += input.y;
        m_AbsoluteY = Mathf.Clamp(m_AbsoluteY, -m_ClampY, m_ClampY);

        //Get player rotation and set camera rotation/position relative to Y input and player rotation
        float desired = m_Player.transform.eulerAngles.y;
        Quaternion rot = Quaternion.Euler(m_AbsoluteY, desired, 0);
        transform.position = m_Player.transform.position - (rot * m_Offset);
        m_DesiredPosition = m_Player.transform.position - (rot * m_StartOffset);
        transform.LookAt(m_Player.transform);

        //Raycast from player to camera, set camera to hit point if raycast hits something
        Debug.DrawRay(m_Player.transform.position, m_DesiredPosition - m_Player.transform.position, Color.green);
        if (Physics.Raycast(m_Player.transform.position, m_DesiredPosition - m_Player.transform.position, out hit, (m_DesiredPosition - m_Player.transform.position).magnitude, m_ZoomMask))
        {
            transform.position = (hit.point - m_Player.transform.position) * 0.8f + m_Player.transform.position; //- new Vector3(m_Offset.x, m_Offset.y, 0);
            Debug.DrawRay(hit.point, Vector3.up * 2, Color.red);
        }
        else
        {
            //m_Offset.z = Mathf.Lerp(m_Offset.z, m_MaxZ, 0.01f);
            m_Offset = Vector3.Lerp(m_Offset, m_StartOffset, 0.01f);
        }
    }

    bool IsColliding()
    {
        float distance = 1f;

        Vector3 tempV = Vector3.zero;
        Vector3 v = transform.position;
        Ray ray = new Ray();

        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case (0):
                    tempV = transform.forward;
                    break;

                case (1):
                    tempV = -tempV;
                    break;

                case (2):
                    tempV = transform.right;
                    break;

                case (3):
                    tempV = -tempV;
                    break;

                case (4):
                    tempV = transform.up;
                    break;

                case (5):
                    tempV = -tempV;
                    break;

                default:
                    break;
            }
            ray = new Ray(v, tempV);
            Debug.DrawRay(v, tempV * distance, Color.red);
            if (Physics.Raycast(ray, distance))
            {
                return true;
            }
        }

        return false;
    }
}
