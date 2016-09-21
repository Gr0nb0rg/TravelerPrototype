using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Mode
{
    FollowPlayer,
    LookAt,
    Static
}

public class ControllerCamera : MonoBehaviour
{
    //Public vars
    public LayerMask m_ZoomMask;
    public Mode m_Mode = Mode.FollowPlayer;
    public CursorLockMode m_CursorMode = CursorLockMode.Locked;
    public Vector3 m_Offset;
    public Vector2 m_Sensitivity = new Vector2(5, 5);
    public bool m_InvertY = false;
    public float m_ClampY = 80;
    public float m_PlayerDistance = 2.0f;

    Transform[] m_LookAtTransforms;

    //Component vars
    ControllerPlayer m_Player;
    MeshRenderer m_PlayerRenderer;

    //Rotation vars
    float m_AbsoluteY;
    float m_AbsoluteX;
    int m_InvertVal = 1;

    //Zoom vars
    RaycastHit m_Hit;
    Vector3 m_DesiredPosition;
    Vector3 m_Target;
    Vector3 m_StartOffset;
    Vector3 m_NonZoomPosition;
    //bool b = false;

    //Look vars
    int m_CurrentLookAt = 0;

    //Input vars
    Vector2 m_Inputs;

    //Text vars
    Text m_Text;

	void Start ()
    {
        m_Player = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        m_PlayerRenderer = m_Player.GetComponentInChildren<MeshRenderer>();
        m_Text = GameObject.Find("CameraInfo2").GetComponent<Text>();
        m_StartOffset = m_Offset;

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
            if (transforms.Length > 1)
            {
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
        m_Inputs = new Vector2(Input.GetAxis("Mouse X") * m_Sensitivity.x, Input.GetAxis("Mouse Y") * m_InvertVal * m_Sensitivity.y);

        var angle = Mathf.Asin(Vector3.Cross(transform.forward, m_Player.transform.forward).y) * Mathf.Rad2Deg;

        //Set absolute X
        m_AbsoluteX += m_Inputs.x;
        if (m_AbsoluteX > 360)
            m_AbsoluteX -= 360;
        else if (m_AbsoluteX < -360)
            m_AbsoluteX += 360;

        //m_AbsoluteX = Mathf.Clamp(m_AbsoluteX, m_Player.transform.rotation.eulerAngles.y - 45, m_Player.transform.rotation.eulerAngles.y + 45);
        //Debug.Log((m_Player.transform.rotation.eulerAngles.y - 45) + " " + (m_Player.transform.rotation.eulerAngles.y + 45));

        //Set absolute Y
        m_AbsoluteY += m_Inputs.y;
        m_AbsoluteY = Mathf.Clamp(m_AbsoluteY, -m_ClampY, m_ClampY);

        //Get player rotation and set camera rotation/position relative to Y input and player rotation
        //float desired = m_Player.transform.eulerAngles.y;
        Quaternion rot = Quaternion.Euler(m_AbsoluteY, m_AbsoluteX, 0);
        m_NonZoomPosition = m_Player.transform.position - (rot * m_StartOffset);

        //Change camera modes
        if (Input.GetKeyDown(KeyCode.R))
            SetMode(Mode.FollowPlayer);
        else if (Input.GetKeyDown(KeyCode.T) && m_LookAtTransforms.Length > 0)
            SetMode(Mode.LookAt);
        else if (Input.GetKeyDown(KeyCode.Y) && m_LookAtTransforms.Length > 0)
            SetMode(Mode.Static);

        //Rotate camera and change position depending on mode
        switch (m_Mode)
        {
            case Mode.FollowPlayer:
                //if (!b)
                transform.position = m_Player.transform.position - (rot * m_Offset);
                //transform.position = Vector3.Lerp(transform.position, m_Player.transform.position - (rot * m_Offset), 10.0f * Time.deltaTime);
                m_DesiredPosition = m_Player.transform.position - (rot * m_Offset);
                m_Target = m_Player.transform.position;

                //Vector3 tar = m_Target - transform.position;
                //Quaternion q = Quaternion.LookRotation(tar);
                //transform.rotation = Quaternion.Lerp(transform.rotation, q, 10.0f * Time.deltaTime);

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
        Debug.DrawRay(m_Target, m_NonZoomPosition - m_Target, Color.green);
        if (Physics.Raycast(m_Target, m_NonZoomPosition - m_Target, out m_Hit, (m_NonZoomPosition - m_Target).magnitude, m_ZoomMask))
        {
            transform.position = (m_Hit.point - m_Target) * 0.8f + m_Target;
            //Vector3 v = (new Vector3(m_Hit.point.x, m_Hit.point.y, 0) - new Vector3(m_Target.x, m_Target.y, 0)) * 0.8f + new Vector3(m_Target.x, m_Target.y, 0);
            //transform.position = new Vector3(v.x, v.y, Mathf.Lerp(transform.position.z, (m_Hit.point.z - m_Target.z) * 0.8f + m_Target.z, 4.0f * Time.deltaTime));
            //transform.position = Vector3.Lerp(transform.position, (m_Hit.point - m_Target) * 0.8f + m_Target, 4.0f * Time.deltaTime);
            //b = true;
            Debug.DrawRay(m_Hit.point, Vector3.up * 2, Color.red);
        }
        //else
        //    b = false;

        //Change player material opacity if camera is too close
        if (Vector3.Distance(transform.position, m_Player.transform.position) < m_PlayerDistance)
        {
            Color col = m_PlayerRenderer.material.color;
            col.a = Mathf.Lerp(col.a, 0.3f, 4 * Time.deltaTime);
            m_PlayerRenderer.material.color = col;
        }
        else
        {
            Color col = m_PlayerRenderer.material.color;
            col.a = Mathf.Lerp(col.a, 1.0f, 4 * Time.deltaTime);
            m_PlayerRenderer.material.color = col;
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

    public Vector2 GetInput()
    {
        return m_Inputs;
    }
}
