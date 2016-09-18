using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerCamera : MonoBehaviour
{
    public enum Mode
    {
        FollowPlayer,
        LookAt,
        Static
    }

    //Public vars
    public LayerMask m_ZoomMask;
    public Mode m_Mode = Mode.FollowPlayer;
    public CursorLockMode m_CursorMode = CursorLockMode.Locked;
    public Vector3 m_Offset;
    public Vector2 m_Sensitivity = new Vector2(5, 5);
    public bool m_InvertY = false;
    public float m_ClampY = 80;
    public float m_PlayerDistance = 2.0f;

    public Transform[] m_LookAtTransforms;

    //Component vars
    ControllerPlayer m_Player;

    //Rotation vars
    float m_AbsoluteY;
    int m_InvertVal = 1;

    //Zoom vars
    float m_MaxZ;
    RaycastHit hit;
    Vector3 m_DesiredPosition;
    Vector3 m_StartOffset;
    Vector3 m_Target;

    //Look vars
    int m_CurrentLookAt = 0;

    //Text vars
    Text m_Text;

	void Start ()
    {
        m_MaxZ = m_Offset.z;
        m_StartOffset = m_Offset;
        m_Player = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        m_Text = GameObject.Find("CameraInfo2").GetComponent<Text>();

        //Find all current lookat transforms
        var transforms = GameObject.FindGameObjectsWithTag("LookAtTransform");
        if (transforms.Length > 0)
        {
            m_LookAtTransforms = new Transform[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                m_LookAtTransforms[i] = transforms[i].transform;
            }

            //Sort transforms
            for (int write = 0; write < transforms.Length; write++)
            {
                for (int sort = 0; sort < transforms.Length - 1; sort++)
                {
                    if (m_LookAtTransforms[sort].GetComponent<LookAtTransform>().m_ID > m_LookAtTransforms[sort + 1].GetComponent<LookAtTransform>().m_ID)
                    {
                        Transform temp = m_LookAtTransforms[sort + 1];
                        m_LookAtTransforms[sort + 1] = m_LookAtTransforms[sort];
                        m_LookAtTransforms[sort] = temp;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Couldn't find any lookat transforms!");
        }
	}
	
	void Update ()
    {
        Cursor.lockState = m_CursorMode;
        TextUpdate();
	}

    void LateUpdate()
    {
        ModeUpdate();
    }

    void ModeUpdate()
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

        //Change camera modes
        if (Input.GetKeyDown(KeyCode.R))
            SetMode(Mode.FollowPlayer);
        else if (Input.GetKeyDown(KeyCode.T) && m_LookAtTransforms.Length > 0)
            SetMode(Mode.LookAt);
        else if (Input.GetKeyDown(KeyCode.Y) && m_LookAtTransforms.Length > 0)
            SetMode(Mode.Static);

        switch (m_Mode)
        {
            case Mode.FollowPlayer:
                transform.position = m_Player.transform.position - (rot * m_Offset);
                m_DesiredPosition = m_Player.transform.position - (rot * m_StartOffset);
                m_Target = m_Player.transform.position;

                //Vector3 tar = m_Target - transform.position;
                //Quaternion q = Quaternion.LookRotation(tar);
                //transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.9f * Time.deltaTime);

                transform.LookAt(m_Player.transform);
                break;

            case Mode.LookAt:
                if (Input.GetKeyDown(KeyCode.Tab))
                    m_CurrentLookAt++;
                if (m_CurrentLookAt > m_LookAtTransforms.Length - 1)
                    m_CurrentLookAt = 0;

                transform.position = m_Player.transform.position - (rot * m_Offset);
                m_DesiredPosition = m_Player.transform.position - (rot * m_Offset);
                m_Target = m_LookAtTransforms[m_CurrentLookAt].position;
                //m_DesiredPosition = Vector3.Lerp(m_DesiredPosition, m_Player.transform.position - (rot * m_Offset), 0.9f * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, m_Player.transform.position - (rot * m_Offset), 0.9f * Time.deltaTime);

                Vector3 tar1 = m_Target - transform.position;
                Quaternion q1 = Quaternion.LookRotation(tar1);
                transform.rotation = Quaternion.Lerp(transform.rotation, q1, 0.9f * Time.deltaTime);
                break;

            case Mode.Static:
                if (Input.GetKeyDown(KeyCode.Tab))
                    m_CurrentLookAt++;
                if (m_CurrentLookAt > m_LookAtTransforms.Length - 1)
                    m_CurrentLookAt = 0;

                //m_DesiredPosition = m_LookAtTransforms[m_CurrentLookAt].position;
                //transform.position = m_LookAtTransforms[m_CurrentLookAt].position;
                m_Target = m_Player.transform.position;
                m_DesiredPosition = Vector3.Lerp(m_DesiredPosition, m_LookAtTransforms[m_CurrentLookAt].position, 0.9f * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, m_LookAtTransforms[m_CurrentLookAt].position, 0.9f * Time.deltaTime);

                Vector3 tar2 = m_Target - transform.position;
                Quaternion q2 = Quaternion.LookRotation(tar2);
                transform.rotation = Quaternion.Lerp(transform.rotation, q2, 0.9f * Time.deltaTime);

                //transform.LookAt(m_Player.transform);
                break;
        }

        //Raycast from player to camera, set camera to hit point if raycast hits something
        Debug.DrawRay(m_Target, m_DesiredPosition - m_Target, Color.green);
        if (Physics.Raycast(m_Target, m_DesiredPosition - m_Target, out hit, (m_DesiredPosition - m_Target).magnitude, m_ZoomMask))
        {
            transform.position = (hit.point - m_Target) * 0.8f + m_Target; //- new Vector3(m_Offset.x, m_Offset.y, 0);
            Debug.DrawRay(hit.point, Vector3.up * 2, Color.red);
        }
    }

    void TextUpdate()
    {
        m_Text.text = "Current mode: " + m_Mode.ToString() + "\nCurrent lookat index: " + m_CurrentLookAt;
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

    void SetMode(Mode newMode)
    {
        m_Mode = newMode;
    }

    public Mode GetMode()
    {
        return m_Mode;
    }
}
