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
    private ControllerPlayer m_playerController;
    //private CapsuleCollider m_playerCollider;

    // Reference values
    private float m_playerHeight;
    private float m_playerWidth;

    private enum Direction{
        UP, DOWN, LEFT, RIGHT
    }

    void Start()
    {
        m_playerController = GetComponent<ControllerPlayer>();
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
            m_playerController.DeactivateWallClimbing();
        }
    }

    bool DirectionAvailable(Direction dir)
    {
        bool retVal = false;
        switch (dir)
        {
            case Direction.UP:
                Vector3 originT = (transform.up.normalized * (m_playerHeight / 2)) + transform.position;
                Vector3 endpointTop = (transform.up.normalized * m_wallEdgeLimit) + originT;
                Vector3 endpointForwardT = (transform.forward.normalized * m_distFromWall) + endpointTop;

                // Check for obstacles above player
                if(m_showFindingRays)
                    Debug.DrawLine(originT, endpointTop, Color.red);
                if (!Physics.Linecast(originT, endpointTop, m_climbWallMask ^ int.MaxValue))
                {
                    // Check for climb wall
                    if (m_showFindingRays)
                        Debug.DrawLine(endpointTop, endpointForwardT, Color.red);
                    if (Physics.Linecast(endpointTop, endpointForwardT, m_climbWallMask))
                    {
                        // Hit a climb wall
                        retVal = true;
                    }
                }
                break;
            case Direction.DOWN:
                Vector3 originB = (-transform.up.normalized * (m_playerHeight / 2)) + transform.position;
                Vector3 endpointBottom = (-transform.up.normalized * m_wallEdgeLimit) + originB;
                Vector3 endpointForwardB = (transform.forward.normalized * m_distFromWall) + endpointBottom;

                // Check for obstacles below player
                if (m_showFindingRays)
                    Debug.DrawLine(originB, endpointBottom, Color.red);
                if (!Physics.Linecast(originB, endpointBottom))
                {
                    // Check for climb wall
                    if(m_showFindingRays)
                        Debug.DrawLine(endpointBottom, endpointForwardB, Color.red);
                    if (Physics.Linecast(endpointBottom, endpointForwardB, m_climbWallMask))
                    {
                        // Hit a climb wall
                        retVal = true;
                    }
                }
                break;
            case Direction.LEFT:
                Vector3 originL = (Vector3.Cross(transform.forward, transform.up).normalized * (m_playerWidth / 2)) + transform.position;
                Vector3 endpointLeft = (Vector3.Cross(transform.forward, transform.up).normalized * m_wallEdgeLimit) + originL;
                Vector3 endpointForwardL = (transform.forward.normalized * m_distFromWall) + endpointLeft;

                // Check for obstacles adjacent to player
                if (m_showFindingRays)
                    Debug.DrawLine(originL, endpointLeft, Color.red);
                if (!Physics.Linecast(originL, endpointLeft))
                {
                    // Check for climb wall
                    if (m_showFindingRays)
                        Debug.DrawLine(endpointLeft, endpointForwardL, Color.red);
                    if (Physics.Linecast(endpointLeft, endpointForwardL, m_climbWallMask))
                    {
                        // Hit a climb wall
                        retVal = true;
                    }
                }
                break;
            case Direction.RIGHT:
                Vector3 originR = (Vector3.Cross(transform.forward, transform.up).normalized * -(m_playerWidth / 2)) + transform.position;
                Vector3 endpointRight = (Vector3.Cross(transform.forward, transform.up).normalized * -m_wallEdgeLimit) + originR;
                Vector3 endpointForwardR = (transform.forward.normalized * m_distFromWall) + endpointRight;

                // Check for obstacles adjacent to player
                if (m_showFindingRays)
                    Debug.DrawLine(originR, endpointRight, Color.red);
                if (!Physics.Linecast(originR, endpointRight))
                {
                    // Check for climb wall
                    if (m_showFindingRays)
                        Debug.DrawLine(endpointRight, endpointForwardR, Color.red);
                    if (Physics.Linecast(endpointRight, endpointForwardR, m_climbWallMask))
                    {
                        // Hit a climb wall
                        retVal = true;
                    }
                }
                break;
            default:
                break;
        }
        return retVal;
    }

    public float DistanceFromWall { get { return m_distFromWall; } }
}
