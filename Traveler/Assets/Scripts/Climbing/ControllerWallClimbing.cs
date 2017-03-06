using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/03/02
*/

/*
    SUMMARY
    Handles alternative navigation that lets player move around on a flat wall according to input
    Drops player from wall when space is pressed
*/
[RequireComponent(typeof(ControllerPlayer))]
[RequireComponent(typeof(CapsuleCollider))]
public class ControllerWallClimbing : MonoBehaviour {

    // PRIVATE SERALIZABLES
    [SerializeField]
    private float m_distFromWall;
    [SerializeField]
    private float m_wallEdgeLimit;
    [SerializeField]
    private float m_climbSpeed;
    [SerializeField]
    private float m_rotationAdjustmentSpeed;
    [SerializeField]
    private bool m_showFindingRays = false;
    [SerializeField]
    private bool m_showMovementDirectionRays = false;

    // Mask to be assigned in editor
    [SerializeField]
    private LayerMask m_climbWallMask;

    // Component variables
    private ControllerClimbing m_controllerClimbing;
    //private CapsuleCollider m_playerCollider;

    // Reference values
    private float m_playerHeight;
    private float m_playerWidth;

    private enum Direction{
        UP, DOWN, LEFT, RIGHT
    }

    void Start()
    {
        m_controllerClimbing = GetComponent<ControllerClimbing>();
        CapsuleCollider playerCollider = GetComponent<CapsuleCollider>();
        m_playerWidth = playerCollider.bounds.size.x;
        m_playerHeight = playerCollider.bounds.size.y;
    }

    void Update()
    {
        UpdateMovement();
    }

    public void InitiateClimb(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    void UpdateMovement()
    {
        // Create movement direction based on player input
        Vector3 positionChange = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            if (DirectionAvailable(Direction.UP))
            {
                Vector3 originU = (transform.up.normalized);
                if (m_showMovementDirectionRays)
                    Debug.DrawLine(transform.position, transform.position + originU, Color.red);
                positionChange += originU;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (DirectionAvailable(Direction.LEFT))
            {
                Vector3 originL = (Vector3.Cross(transform.forward, transform.up).normalized);
                if (m_showMovementDirectionRays)
                    Debug.DrawLine(transform.position, transform.position + originL, Color.red);
                positionChange += originL;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (DirectionAvailable(Direction.DOWN))
            {
                Vector3 originD = (-transform.up.normalized);
                if (m_showMovementDirectionRays)
                    Debug.DrawLine(transform.position, transform.position + originD, Color.red);
                positionChange += originD;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (DirectionAvailable(Direction.RIGHT))
            {
                Vector3 originR = (-Vector3.Cross(transform.forward, transform.up).normalized);
                if(m_showMovementDirectionRays)
                    Debug.DrawLine(transform.position, transform.position+originR, Color.red);
                positionChange += originR;
            }
        }

        // Move player in combined input direction
        if(!(positionChange.x == 0 && positionChange.y == 0))
            transform.position = Vector3.MoveTowards(transform.position, transform.position + positionChange, m_climbSpeed * Time.deltaTime);

        // Release wall when going down and out from a wall
        //if (Input.GetKey(KeyCode.S) && !DirectionAvailable(Direction.DOWN))
        //{
        //    m_playerController.DeactivateWallClimbing();
        //}

        if (Input.GetKey(KeyCode.Space))
        {
            m_controllerClimbing.DeactivateWallClimbing();
        }
    }

    bool DirectionAvailable(Direction dir)
    {
        bool retVal = false;
        Vector3 sideOffset = transform.right * m_playerWidth * 0.5f;
        Vector3 forwardOffset = transform.forward.normalized * m_distFromWall;

        Vector3 originT = (transform.up.normalized * (m_playerHeight * 0.35f)) + transform.position;
        Vector3 originB = (-transform.up.normalized * (m_playerHeight * 0.5f)) + transform.position;
        Vector3 offsetLR = transform.up.normalized * 0.05f;
        Vector3 offsetUD = transform.right.normalized * 0.05f;

        switch (dir)
        {
            case Direction.UP:
                // Check for obstacles above player
                if (m_showFindingRays) {
                    Debug.DrawLine(originT + sideOffset - offsetUD, originT + sideOffset + forwardOffset - offsetUD, Color.red);
                    Debug.DrawLine(originT - sideOffset + offsetUD, originT - sideOffset + forwardOffset + offsetUD, Color.red);
                }
                    
                if ( Physics.Linecast(originT + sideOffset - offsetUD, originT + sideOffset + forwardOffset - offsetUD, m_climbWallMask)
                &&   Physics.Linecast(originT - sideOffset + offsetUD, originT - sideOffset + forwardOffset + offsetUD, m_climbWallMask))
                {
                    retVal = true;
                }
                break;
            case Direction.DOWN:
                // Check for obstacles above player
                if (m_showFindingRays)
                {
                    Debug.DrawLine(originB + sideOffset - offsetUD, originB + sideOffset + forwardOffset - offsetUD, Color.red);
                    Debug.DrawLine(originB - sideOffset + offsetUD, originB - sideOffset + forwardOffset + offsetUD, Color.red);
                }

                if (Physics.Linecast(originB + sideOffset - offsetUD, originB + sideOffset + forwardOffset - offsetUD, m_climbWallMask)
                && Physics.Linecast(originB - sideOffset + offsetUD, originB - sideOffset + forwardOffset + offsetUD, m_climbWallMask))
                {
                    retVal = true;
                }
                break;
            case Direction.LEFT:
                if (m_showFindingRays)
                {
                    Debug.DrawLine(originT - sideOffset - offsetLR, originT - sideOffset + forwardOffset - offsetLR, Color.red);
                    Debug.DrawLine(originB - sideOffset + offsetLR, originB - sideOffset + forwardOffset + offsetLR, Color.red);
                }

                if (Physics.Linecast(originT - sideOffset - offsetLR, originT - sideOffset + forwardOffset - offsetLR, m_climbWallMask)
                && Physics.Linecast(originB - sideOffset + offsetLR, originB - sideOffset + forwardOffset + offsetLR, m_climbWallMask))
                {
                    retVal = true;
                }
                break;
            case Direction.RIGHT:
                if (m_showFindingRays)
                {
                    Debug.DrawLine(originT + sideOffset - offsetLR, originT + sideOffset + forwardOffset - offsetLR, Color.red);
                    Debug.DrawLine(originB + sideOffset + offsetLR, originB + sideOffset + forwardOffset + offsetLR, Color.red);
                }

                if (Physics.Linecast(originT + sideOffset - offsetLR, originT + sideOffset + forwardOffset - offsetLR, m_climbWallMask)
                && Physics.Linecast(originB + sideOffset + offsetLR, originB + sideOffset + forwardOffset + offsetLR, m_climbWallMask))
                {
                    retVal = true;
                }
                break;
            default:
                break;
        }
        return retVal;
    }

    public float DistanceFromWall { get { return m_distFromWall; } }
}
