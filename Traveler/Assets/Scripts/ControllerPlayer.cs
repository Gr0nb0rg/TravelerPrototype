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

    //Component vars
    Rigidbody m_Rigidbody;
    Collider m_Collider;

    //Jump vars
    bool m_IsOnGround = false;

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
	}

    void HorizontalUpdate()
    {
        m_Rigidbody.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * m_MovementSpeed, m_Rigidbody.velocity.y, Input.GetAxisRaw("Vertical") * m_MovementSpeed);
    }

    void JumpUpdate()
    {
        if (m_IsOnGround && Input.GetKeyDown(KeyCode.Space))
            m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
    }

    void CheckState()
    {
        if (IsOnGround())
        {
            if (m_Rigidbody.velocity.magnitude > 1)
                SetState(MovementState.Moving);
            else
                SetState(MovementState.Idle);
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
            if (Physics.Raycast(ray, distance))
            {
                return true;
            }
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
}
