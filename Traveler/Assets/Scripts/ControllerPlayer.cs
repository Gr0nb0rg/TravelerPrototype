using UnityEngine;
using System.Collections;

public class ControllerPlayer : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Running,
        Grabbing,
        Climbing,
        Sliding
    }

    //Public vars
    public MovementState m_State = MovementState.Idle;
    public float m_MovementSpeed = 15.0f;
    public float m_JumpForce = 200.0f;
    public float m_MaxAngle = 45.0f;
    public float m_SlopeSpeed = 25.0f;

    //Component vars
    Rigidbody m_Rigidbody;
    Collider m_Collider;

    //Jump vars
    bool m_IsOnGround = false;

    //Movement vars
    RaycastHit m_Hit;
    Vector3 m_SlopeVelocity;

	void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponentInChildren<CapsuleCollider>();
	}
	
	void Update()
    {  
        CheckState();

        HorizontalUpdate();
        JumpUpdate();

        if (transform.position.y < -20)
            transform.position = new Vector3(0, 10, 0);
	}

    void HorizontalUpdate()
    {
        //Get forward rotation
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);

        //Set velocity relative to rotation
        if (m_IsOnGround)
        {
            //Set velocity to input values if not sliding, else set to slopevelocity
            if (!GetState().Equals(MovementState.Sliding))
                m_Rigidbody.velocity = rot * new Vector3(Input.GetAxisRaw("Horizontal") * m_MovementSpeed, m_Rigidbody.velocity.y, Input.GetAxisRaw("Vertical") * m_MovementSpeed);

            else
                m_Rigidbody.velocity = m_SlopeVelocity * m_SlopeSpeed;
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

    void JumpUpdate()
    {
        if (m_IsOnGround && Input.GetKeyDown(KeyCode.Space))
            m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
    }

    void CheckState()
    {
        //Checks what current movementstate the player should be in
        if (IsOnGround())
        {
            if (!SlopeCheck(m_MaxAngle, 1.3f))
            {
                if (m_Rigidbody.velocity.magnitude > 1)
                    SetState(MovementState.Moving);
                else
                    SetState(MovementState.Idle);
            }
            else
                SetState(MovementState.Sliding);

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

    bool IsOnGround()
    {
        float offsetCenter = 0.3f;
        float offsetHeight = 0.5f;
        float distance = 0.6f;

        Vector3 tempV = Vector3.zero;
        Vector3 v = new Vector3(transform.position.x, m_Collider.bounds.min.y + offsetHeight, transform.position.z);
        Ray ray = new Ray();

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
            ray = new Ray(tempV + v, Vector3.down);
            Debug.DrawRay(tempV + v, Vector3.down * distance, Color.red);
            if (Physics.Raycast(ray, out m_Hit, distance))
            {
                //if (!m_IsOnGround)
                //transform.position = new Vector3(transform.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, transform.position.z);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, transform.position.z), 1.0f);
                //Vector3 pos = transform.position;
                //pos.y += -m_Hit.normal.x * Mathf.Abs(m_Rigidbody.velocity.x) * Time.deltaTime * (m_Rigidbody.velocity.x - m_Hit.normal.x > 0 ? 1 : -1);
                //transform.position = pos;
                if (!GetState().Equals(MovementState.Sliding))
                    m_Rigidbody.MovePosition(new Vector3(m_Rigidbody.position.x, m_Hit.point.y + m_Collider.bounds.size.y / 2, m_Rigidbody.position.z));
                return true;
            }
        }

        return false;
    }

    bool SlopeCheck(float angle, float raydist)
    {
        Debug.DrawRay(transform.position, Vector3.down * raydist, Color.blue);
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out m_Hit, raydist))
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

    void SetState(MovementState newState)
    {
        m_State = newState;
    }

    void SetGravity(bool state)
    {
        m_Rigidbody.useGravity = state;
    }

    public MovementState GetState()
    {
        return m_State;
    }

    public Vector3 GetVelocity()
    {
        return m_Rigidbody.velocity;
    }
}
