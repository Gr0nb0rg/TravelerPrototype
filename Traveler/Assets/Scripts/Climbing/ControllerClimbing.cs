using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerWallClimbing))]
[RequireComponent(typeof(ControllerLedgeClimbing))]
public class ControllerClimbing : MonoBehaviour {

    public enum ClimbingState
    {
        NotClimbing,
        LedgeClimbing,
        WallClimbing,
        ReleasedWall,
        ReleasedLedge
    }

    private ClimbingState m_currentState;
    private ControllerPlayer m_controllerPlayer;
    private ControllerWallClimbing m_controllerWallClimbing;
    private ControllerLedgeClimbing m_controllerLedgeClimbing;

    private CapsuleCollider m_Collider;

    [SerializeField]
    private float m_climbCheckRayDist;
    [SerializeField]
    private Transform m_climbRaycheckR;
    [SerializeField]
    private Transform m_climbRaycheckL;
    [SerializeField]
    private LayerMask m_climbCheckLayers;

    void Start () {
        m_controllerPlayer = GetComponent<ControllerPlayer>();
        m_controllerWallClimbing = GetComponent<ControllerWallClimbing>();
        m_controllerLedgeClimbing = GetComponent<ControllerLedgeClimbing>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_currentState = ClimbingState.NotClimbing;
    }

    public void UpdateClimbing() {
        if (m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.Falling)
        || m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.Jumping)
        || m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.Climbing))
        {
            switch (m_currentState)
            {
                case ClimbingState.NotClimbing:
                    CheckForClimbWalls();
                    CheckForLedges();
                    break;
                case ClimbingState.LedgeClimbing:
                    CheckForWallFromLedge();
                    break;
                case ClimbingState.WallClimbing:
                    CheckForLedgesFromWall();
                    break;
                case ClimbingState.ReleasedWall:
                    //CheckForLedgesFromWall();
                    break;
                case ClimbingState.ReleasedLedge:
                    CheckForClimbWalls();
                    break;
                default:
                    break;
            }
        }
    }

    public void CheckForLedgesFromWall() {
        // Shoot a ray up from above head
        if (Input.GetKey(KeyCode.W)) {
            Vector3 origin = transform.position + transform.forward * 0.2f;
            Debug.DrawLine(origin, origin + new Vector3(0, m_Collider.bounds.size.y * 0.5f, 0), Color.blue);
            RaycastHit hitUP;
            if (Physics.Linecast(origin, origin + new Vector3(0, m_Collider.bounds.size.y * 0.5f, 0), out hitUP, m_climbCheckLayers))
            {
                if (hitUP.collider.gameObject.tag.Equals("ClimbLedge"))
                {
                    if(Vector3.Angle(hitUP.transform.forward, transform.forward) < 10)
                    {
                        SwitchToLedgeClimb(hitUP.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.D)) {
            Vector3 origin = transform.position + (transform.forward * 0.2f) + new Vector3(0,m_Collider.bounds.size.y * 0.35f,0);
            Debug.DrawLine(origin, origin + new Vector3(m_Collider.bounds.size.x * 0.8f, 0, 0), Color.blue);
            RaycastHit hitR;
            if (Physics.Linecast(origin, origin + new Vector3(m_Collider.bounds.size.x * 0.8f, 0, 0), out hitR, m_climbCheckLayers))
            {
                if (hitR.collider.gameObject.tag.Equals("ClimbLedge"))
                {
                    if (Vector3.Angle(hitR.transform.forward, transform.forward) > 45)
                    {
                        SwitchToLedgeClimb(hitR.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
                    }
                }
            }
        }
    }

    public void CheckForLedges()
    {
        Vector3 endpointR = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckR.position);
        Vector3 endpointL = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckL.position);
        Debug.DrawLine(m_climbRaycheckR.position, endpointR, Color.blue);
        Debug.DrawLine(m_climbRaycheckL.position, endpointL, Color.blue);

        // Shoot two rays forward from above head
        RaycastHit hitR;
        RaycastHit hitL;

        if (Physics.Linecast(m_climbRaycheckR.position, endpointR, out hitR, m_climbCheckLayers)
            &&
            Physics.Linecast(m_climbRaycheckL.position, endpointL, out hitL, m_climbCheckLayers))
        {
            // Did both hit ledge trigger?
            if (hitL.collider.gameObject.tag.Equals("ClimbLedge") && hitR.collider.gameObject.tag.Equals("ClimbLedge"))
            {
                // Switch to
                if (m_currentState == ClimbingState.WallClimbing)
                {
                    SwitchToLedgeClimb(hitR.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
                }
                // Activate
                else if (m_currentState == ClimbingState.NotClimbing)
                {
                    // Start climbing at climb target closest to a point between raycasts
                    ActivateLedgeClimbing(hitR.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
                }
            }
        }
    }

    public void CheckForClimbWalls() {
        Vector3 endpointR = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckR.position);
        Vector3 endpointL = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckL.position);
        Debug.DrawLine(m_climbRaycheckR.position, endpointR, Color.blue);
        Debug.DrawLine(m_climbRaycheckL.position, endpointL, Color.blue);
        // Shoot two rays forward from above head
        RaycastHit hitR;
        RaycastHit hitL;
        if (Physics.Linecast(m_climbRaycheckR.position, endpointR, out hitR, m_climbCheckLayers)
            &&
            Physics.Linecast(m_climbRaycheckL.position, endpointL, out hitL, m_climbCheckLayers))
        {
            // Did both hit wall trigger?
            if (hitL.collider.gameObject.tag.Equals("ClimbWall") && hitR.collider.gameObject.tag.Equals("ClimbWall"))
            {
                // Switch to
                if (m_currentState.Equals(ClimbingState.LedgeClimbing))
                {
                    SwitchToWallClimb(hitL.collider.gameObject.transform);
                }
                // Activate
                else if (m_currentState.Equals(ClimbingState.NotClimbing) || m_currentState.Equals(ClimbingState.ReleasedLedge))
                {
                    if (Vector3.Distance(hitL.point, m_climbRaycheckL.position) <= m_controllerWallClimbing.DistanceFromWall)
                        ActivateWallClimbing(hitL.collider.gameObject.transform);
                }
            }
        }
    }

    public void CheckForWallFromLedge() {

    }

    //private void CheckForClimbingAreas()
    //{
    //    //if ((GetState().Equals(MovementState.Jumping) || GetState().Equals(MovementState.Falling) || GetState().Equals(MovementState.WallClimbing) || GetState().Equals(MovementState.LedgeClimbing)))
    //    //{
    //        Vector3 endpointR = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckR.position);
    //        Vector3 endpointL = ((transform.forward.normalized * m_climbCheckRayDist) + m_climbRaycheckL.position);
    //        Debug.DrawLine(m_climbRaycheckR.position, endpointR, Color.blue);
    //        Debug.DrawLine(m_climbRaycheckL.position, endpointL, Color.blue);
            
    //        // Shoot two rays forward from above head
    //        RaycastHit hitR;
    //        RaycastHit hitL;

    //        if (Physics.Linecast(m_climbRaycheckR.position, endpointR, out hitR, m_climbCheckLayers)
    //            &&
    //            Physics.Linecast(m_climbRaycheckL.position, endpointL, out hitL, m_climbCheckLayers))
    //        {
    //            // Did both hit ledge trigger?
    //            if ((hitL.collider.gameObject.tag.Equals("ClimbLedge") && hitR.collider.gameObject.tag.Equals("ClimbLedge")) && m_findLedges)
    //            {
    //                // Switch to
    //                if (m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.WallClimbing))
    //                {
    //                    SwitchToLedgeClimb(hitR.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
    //                }
    //                // Activate
    //                else if (!m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.LedgeClimbing))
    //                {
    //                    // Start climbing at climb target closest to a point between raycasts
    //                    ActivateLedgeClimbing(hitR.collider.gameObject.GetComponent<LedgeEdge>().GetClosestTarget(transform.position));
    //                }
    //            }
    //            // Did both hit wall trigger?
    //            else if ((hitL.collider.gameObject.tag.Equals("ClimbWall") && hitR.collider.gameObject.tag.Equals("ClimbWall")) && m_findWalls)
    //            {
    //                // Switch to
    //                if (m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.LedgeClimbing))
    //                {
    //                    SwitchToWallClimb(hitL.collider.gameObject.transform);
    //                }
    //                // Activate
    //                else if (!m_controllerPlayer.GetState().Equals(ControllerPlayer.MovementState.WallClimbing))
    //                {
    //                    if (Vector3.Distance(hitL.point, m_climbRaycheckL.position) <= m_controllerWallClimbing.DistanceFromWall)
    //                        ActivateWallClimbing(hitL.collider.gameObject.transform);
    //                }
    //            }
    //        }
    //    //}
    //}

    // Go into wall climbing mode
    public void ActivateWallClimbing(Transform wall)
    {
        //transform.parent = wall;
        m_currentState = ClimbingState.WallClimbing;
        m_controllerWallClimbing.enabled = true;
        m_controllerWallClimbing.InitiateClimb(wall.rotation);
        m_controllerPlayer.ActivateClimbing();
    }

    // Go out of wall climbing mode
    public void DeactivateWallClimbing()
    {
        //transform.parent = null;
        m_currentState = ClimbingState.ReleasedWall;
        m_controllerWallClimbing.enabled = false;
        m_controllerPlayer.DeactivateClimbing();
    }

    // Go from ledgeclimbing to wall climbing
    private void SwitchToWallClimb(Transform wall)
    {
        transform.parent = null;
        m_currentState = ClimbingState.WallClimbing;
        m_controllerLedgeClimbing.enabled = false;
        m_controllerWallClimbing.enabled = true;
        m_controllerWallClimbing.InitiateClimb(wall.rotation);
    }

    // Go into ledge climbing mode
    public void ActivateLedgeClimbing(ClimbTarget initialTarget)
    {
        m_currentState = ClimbingState.LedgeClimbing;
        transform.parent = initialTarget.transform;
        m_controllerLedgeClimbing.enabled = true;
        m_controllerLedgeClimbing.InitiateClimb(initialTarget);
        m_controllerPlayer.ActivateClimbing();
    }

    // Go out of ledge climbing mode
    public void DeactivateLedgeClimbing()
    {
        m_currentState = ClimbingState.ReleasedLedge;
        transform.parent = null;
        m_controllerLedgeClimbing.enabled = false;
        m_controllerPlayer.DeactivateClimbing();
    }

    // Go from wall climbing to ledge climbing
    private void SwitchToLedgeClimb(ClimbTarget initialTarget)
    {
        //print("SWITCH");
        m_currentState = ClimbingState.LedgeClimbing;
        m_controllerWallClimbing.enabled = false;
        m_controllerLedgeClimbing.enabled = true;
        m_controllerLedgeClimbing.InitiateClimb(initialTarget);
    }

    public ClimbingState State { set { m_currentState = value; } }
}
