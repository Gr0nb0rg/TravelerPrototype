using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ControllerWallClimbing))]
[RequireComponent(typeof(ControllerLedgeClimbing))]
[RequireComponent(typeof(PlayerCollisions))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(ControllerCheckpoint))]
public class ControllerPlayer : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Running,
        LedgeClimbing,
        WallClimbing,
        Sliding
    }

    //Public vars
    public MovementState m_State = MovementState.Idle;
    public float m_MovementSpeed = 15.0f;

    public float m_StrafeTopSpeed = 15.0f;
    public float m_ForwardTopSpeed = 20.0f;
    public float m_BackUpTopSpeed = 10.0f;

    public float m_JumpForce = 15.0f;
    public float m_MaxAngle = 45.0f;
    public float m_SlopeSpeed = 25.0f;

    //Component vars
    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private ControllerCamera m_Camera;
    private ControllerWallClimbing m_controllerWallClimbing;
    private ControllerLedgeClimbing m_controllerLedgeClimbing;

    // Climbing vars
    [SerializeField]
    private float m_climbCheckRayDist;
    [SerializeField]
    private Transform m_climbRaycheckR;
    [SerializeField]
    private Transform m_climbRaycheckL;
    private bool m_releasedClimb = false;

    //Jump vars
    private bool m_IsOnGround = false;

    //Movement vars
    [SerializeField]
    private LayerMask m_StandableLayers;
    private RaycastHit m_Hit;
    private Vector3 m_SlopeVelocity;

    //Paused vars
    private bool m_IsPaused = false;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_Camera = Camera.main.GetComponent<ControllerCamera>();
        m_controllerWallClimbing = GetComponent<ControllerWallClimbing>();
        m_controllerLedgeClimbing = GetComponent<ControllerLedgeClimbing>();
    }

    void Update()
    {
        if (!m_IsPaused)
        {
            CheckForClimbingAreas();
            CheckState();
            HorizontalUpdate();
        }
    }

    void FixedUpdate()
    {
        JumpUpdate();
    }

    void HorizontalUpdate()
    {
        if (!GetState().Equals(MovementState.WallClimbing) && !GetState().Equals(MovementState.LedgeClimbing))
        {
            //Get forward rotation
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            Quaternion rot; //= Quaternion.LookRotation(forward, Vector3.up);

            //Set rotation to camera look if mode is followplayer
            if (m_Camera.GetMode().Equals(Mode.FollowPlayer))
            {
                if (!GetState().Equals(MovementState.Moving))
                {
                    //If player is still then the rotation should be static
                    Vector3 f = transform.forward;
                    f.y = 0;
                    rot = Quaternion.LookRotation(f, Vector3.up);
                }
                else
                {
                    //If the player is moving the the rotation will be set to the cameras direction
                    Quaternion q = Camera.main.transform.rotation;
                    transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, q.y, transform.rotation.z, q.w), 10 * Time.deltaTime);

                    rot = Quaternion.LookRotation(forward, Vector3.up);
                }
            }
            //Set rotation to X input if not in followplayer mode
            else
            {
                transform.Rotate(0, m_Camera.GetInput().x, 0);
                rot = Quaternion.LookRotation(forward, Vector3.up);
            }

            //Set velocity relative to rotation
            if (m_IsOnGround)
            {

                //Set velocity to input values if not sliding, else set to slopevelocity
                if (!GetState().Equals(MovementState.Sliding))
                {
                    m_Rigidbody.velocity = rot * new Vector3(Input.GetAxisRaw("Horizontal") * 0.7f * m_MovementSpeed, m_Rigidbody.velocity.y, Input.GetAxisRaw("Vertical") * m_MovementSpeed);
                }
                else
                    m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, rot * new Vector3(Input.GetAxisRaw("Horizontal") * 10, 0, 0) + (m_SlopeVelocity * m_SlopeSpeed), 3.0f * Time.deltaTime);
            }

            /*float dist = 1.3f;
            Debug.DrawRay(transform.position, Vector3.down * dist, Color.green);
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out m_Hit, dist))
            {
                transform.position = m_Hit.point;
                if (!m_IsOnGround)
                    transform.position = new Vector3(transform.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, transform.position.z);
                m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.transform.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, m_Rigidbody.transform.position.z));
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, transform.position.z), 1.0f);
            }
            float yDisplacement = m_Rigidbody.velocity.y;
            float xDisplacement = m_Rigidbody.velocity.x;
            if (-Mathf.Abs(xDisplacement) < yDisplacement && yDisplacement < 0)
            {
                yDisplacement = -Mathf.Abs(xDisplacement) - 0.001f;
            }
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, yDisplacement, m_Rigidbody.velocity.z);*/
        }
    }

    void JumpUpdate()
    {
        if (m_IsOnGround && Input.GetKeyDown(KeyCode.Space) && (m_State != MovementState.WallClimbing) && (m_State != MovementState.LedgeClimbing))
        {
            //m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpForce, m_Rigidbody.velocity.z);
        }
    }

    void CheckState()
    {
        if (!GetState().Equals(MovementState.WallClimbing) && !GetState().Equals(MovementState.LedgeClimbing))
        {
            //Checks what current movementstate the player should be in
            if (IsOnGround())
            {
                if (!SlopeCheck(m_MaxAngle, 1.5f))
                {
                    if (m_Rigidbody.velocity.magnitude > 1)
                        SetState(MovementState.Moving);
                    else
                        SetState(MovementState.Idle);
                }
                else
                    SetState(MovementState.Sliding);
                m_releasedClimb = false;
                m_IsOnGround = true;
                SetGravity(false);
            }
            else
            {
                m_IsOnGround = false;
                SetGravity(true);
                if (m_Rigidbody.velocity.y > 0)
                    SetState(MovementState.Jumping);
                else
                    SetState(MovementState.Falling);

            }
        }
        DrawCast(transform.position, transform.forward, 10);
    }

    bool IsOnGround()
    {
        float offsetCenter = 0.3f;
        float offsetHeight = 0.5f;
        float distance = 0.6f;

        Vector3 tempV = Vector3.zero;
        Vector3 v = new Vector3(transform.position.x, m_Collider.bounds.min.y + offsetHeight, transform.position.z);

        for (int i = 0; i < 4; i++)
        {
            switch(i)
            {
                case (0):
                    tempV = new Vector3(transform.forward.x * offsetCenter, 0, transform.forward.z * offsetCenter);
                    break;

                case (1):
                    tempV = -tempV;
                    break;

                case (2):
                    tempV = new Vector3(transform.right.x * offsetCenter, 0, transform.right.z * offsetCenter);
                    break;

                case (3):
                    tempV = -tempV;
                    break;

                default:
                    break;
            }
            // use 
            if (DrawCast(tempV + v, Vector3.down, out m_Hit, distance, m_StandableLayers))
            {
                if (!GetState().Equals(MovementState.Sliding))
                    m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, m_Rigidbody.position.z));
                return true;
            }
        }

        return false;
    }

    bool SlopeCheck(float angle, float raydist)
    {
        if (DrawCast(transform.position, Vector3.down, out m_Hit, raydist))
        {
            //Set slopevelocity to be relative to hit normal
            m_SlopeVelocity = new Vector3(m_Hit.normal.x, -m_Hit.normal.y, m_Hit.normal.z);
            Debug.DrawRay(transform.position, m_SlopeVelocity);

            //Check if current angle between upwards vector and normal is equal to or greater than given value
            float a = Vector3.Angle(Vector3.up, m_Hit.normal);
            if (a >= angle)
                return true;
        }

        return false;
    }

    public void CheckForClimbingAreas()
    {
        if ((GetState().Equals(MovementState.Jumping) || GetState().Equals(MovementState.Falling)) && !m_releasedClimb)
        {
            Vector3 endpointR = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckR.position);
            Vector3 endpointL = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckL.position);
            Debug.DrawLine(m_climbRaycheckR.position, endpointR, Color.blue);
            Debug.DrawLine(m_climbRaycheckL.position, endpointL, Color.blue);

            // Shoot two rays from above head
            RaycastHit hitR;
            RaycastHit hitL;
            if (
                Physics.Linecast(m_climbRaycheckR.position, endpointR, out hitR)
                &&
                Physics.Linecast(m_climbRaycheckL.position, endpointL, out hitL))
            {
                // Did both hit ledge trigger?
                if (hitL.collider.gameObject.tag.Equals("ClimbLedge") && hitR.collider.gameObject.tag.Equals("ClimbLedge"))
                {
                    // Start climbing at climb target closest to a point between raycasts
                    ActivateLedgeClimbing(hitR.collider.gameObject.GetComponent<ClimbTargetCollection>().GetClosestTarget(transform.position));
                }
                // Did both hit wall trigger?
                else if (hitL.collider.gameObject.tag.Equals("ClimbWall") && hitR.collider.gameObject.tag.Equals("ClimbWall"))
                {
                    ActivateWallClimbing(hitL.collider.gameObject.transform.rotation);
                }
            }
        }
    }

    // Go into wall climbing mode
    public void ActivateWallClimbing(Quaternion wallRotaion)
    {
        SetState(MovementState.WallClimbing);
        m_controllerWallClimbing.enabled = true;
        m_controllerWallClimbing.InitiateClimb(wallRotaion);
        SetKinematic(true);
        SetGravity(false);
    }

    // Go out of wall climbing mode
    public void DeactivateWallClimbing()
    {
        SetState(MovementState.Falling);
        if (m_controllerWallClimbing.enabled == true)
            m_controllerWallClimbing.enabled = false;
        m_releasedClimb = true;
        SetKinematic(false);
        SetGravity(true);
    }

    // Go into ledge climbing mode
    public void ActivateLedgeClimbing(ClimbTarget initialTarget)
    {
        SetState(MovementState.LedgeClimbing);
        m_controllerLedgeClimbing.enabled = true;
        m_controllerLedgeClimbing.InitiateClimb(initialTarget);
        SetKinematic(true);
        SetGravity(false);
    }

    // Go out of ledge climbing mode
    public void DeactivateLedgeClimbing()
    {
        SetState(MovementState.Falling);
        if (m_controllerLedgeClimbing.enabled == true)
            m_controllerLedgeClimbing.enabled = false;
        m_releasedClimb = true;
        SetKinematic(false);
        SetGravity(true);
    }

    //Collection of raycast-functions that also draw the ray
    bool DrawCast(Vector3 position, Vector3 direction, float distance, int layermask)
    {
        Color col = Color.green;
        bool hit = false;
        if (Physics.Raycast(position, direction, distance, layermask))
        {
            hit = true;
        }
        if (hit)
            col = Color.red;
        Debug.DrawRay(position, direction, col);

        return hit;
    }

    bool DrawCast(Vector3 position, Vector3 direction, float distance)
    {
        Color col = Color.green;
        bool hit = false;
        if (Physics.Raycast(position, direction, distance))
        {
            hit = true;
        }
        if (hit)
            col = Color.red;
        Debug.DrawRay(position, direction, col);

        return hit;
    }

    bool DrawCast(Vector3 position, Vector3 direction, out RaycastHit info, float distance)
    {
        Color col = Color.green;
        bool hit = false;
        if (Physics.Raycast(position, direction, out info, distance))
        {
            hit = true;
        }
        if (hit)
            col = Color.red;
        Debug.DrawRay(position, direction, col);

        return hit;
    }

    bool DrawCast(Vector3 position, Vector3 direction, out RaycastHit info, float distance, int layermask)
    {
        Color col = Color.green;
        bool hit = false;
        Ray ray = new Ray(position, direction);
        if (Physics.Raycast(ray, out info, distance, layermask))
        {
            hit = true;
        }
        if (hit)
            col = Color.red;
        Debug.DrawRay(position, direction, col);

        return hit;
    }

    void SetState(MovementState newState)
    {
        m_State = newState;
    }

    public MovementState GetState()
    {
        return m_State;
    }

    void SetGravity(bool state)
    {
        m_Rigidbody.useGravity = state;
    }

    void SetKinematic(bool state)
    {
        m_Rigidbody.isKinematic = state;
    }

    public Vector3 GetVelocity()
    {
        return m_Rigidbody.velocity;
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void ResetValues()
    {
        m_Rigidbody.velocity = Vector3.zero;
    }

    public void SetPaused(bool state)
    {
        m_IsPaused = state;
    }

    public bool GetIsPaused()
    {
        return m_IsPaused;
    }
}
