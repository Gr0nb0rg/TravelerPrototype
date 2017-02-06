using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/04
*/

/*
    SUMMARY
    Handles alternative navigation that lets player move from side to side when holding onto a ledge
    Climbs up from a ledge when up is pressed and player can stand on top of ledge
    Drops player from wall when space is pressed
*/

public class ControllerLedgeClimbing : MonoBehaviour
{
    // Editor options
    [SerializeField]
    private bool m_updateRootEachFrame = false; // For editor purposes only
    // Time it takes to reach a new point
    [SerializeField]
    private float m_transitionDuration = 0.4f;

    // Component vars
    private ControllerPlayer m_playerController;
    private ClimbingIK m_climbingIK;
    private CapsuleCollider m_collider;
    private ClimbTarget m_currentClimbTarget = null;

    // Reference values
    private float m_playerHeight;
    private float m_playerWidth;

    // Vault variables
    [SerializeField]
    private LayerMask m_vaultCheckMask;
    private Vector3 m_vaultpos;


    private bool m_inTransition = false;

    void Start()
    {
        m_collider = GetComponent<CapsuleCollider>();
        m_playerHeight = m_collider.bounds.size.y;
        m_playerWidth = m_collider.bounds.size.x;
        m_playerController = GetComponent<ControllerPlayer>();
        m_climbingIK = transform.Find("Ethan").GetComponent<ClimbingIK>();
        this.enabled = false;
    }

    void Update()
    {
        UpdateMovement();
    }

    public void InitiateClimb(ClimbTarget initialTarget)
    {
        if (initialTarget != null)
        {
            m_currentClimbTarget = initialTarget;
            transform.position = m_currentClimbTarget.GetRootTarget().transform.position;
            transform.rotation = m_currentClimbTarget.GetRootTarget().transform.rotation;

            // Set IK Positions
            m_climbingIK.SetAllClimbTargets(m_currentClimbTarget);
            m_climbingIK.m_ikActive = true;
        }
    }

    public void UpdateMovement()
    {
        //m_currentClimbTarget.GetLeftNeighbour();
        //m_currentClimbTarget.GetRightNeighbour();

        // For editing root position in realtime
        if (m_updateRootEachFrame)
        {
            transform.position = m_currentClimbTarget.GetRootTarget().transform.position;
            transform.rotation = m_currentClimbTarget.GetRootTarget().transform.rotation;
        }

        // Find input direction
        float moveDirection = 0;
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= 1;
        }

        // Move in direction
        if ((moveDirection != 0) && !m_inTransition)
        {
            if (moveDirection == 1)
            {
                MoveToTarget(m_currentClimbTarget.GetRightNeighbour());
            }
            else
            {
                MoveToTarget(m_currentClimbTarget.GetLeftNeighbour());
            }
        }

        // Drop
        if (Input.GetKey(KeyCode.S) && !m_inTransition)
        {
            m_climbingIK.m_ikActive = false;
            // NOTE: Assumes climbTarget roots are only rotated in x
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            m_playerController.DeactivateLedgeClimbing();
        }

        // Vault
        if (Input.GetKey(KeyCode.Space) && !m_inTransition)
        {
            if (CanVault())
            {
                Vault();
            }
        }
    }

    // NOTE: Optimise to check once
    // Reset check when player switched point
    // Possible problems with moving pillars?
    private bool CanVault()
    {
        // Left shoulder line check
        Vector3 playerTopL = new Vector3(transform.position.x, transform.position.y + (m_playerHeight / 2), transform.position.z);
        Vector3 topWithOffsetL = ((-transform.right.normalized * (m_playerWidth / 2)) + playerTopL);
        Vector3 checkOriginL = ((-transform.forward.normalized * (m_playerWidth / 2)) + topWithOffsetL);
        Vector3 endpointTopL = ((Vector3.up.normalized * (m_playerHeight * 0.75f)) + checkOriginL);
        Vector3 endpointForwardL = ((transform.forward.normalized * (m_playerWidth * 2)) + endpointTopL);
        Vector3 endpointDiveL = ((-Vector3.up.normalized * (m_playerHeight)) + endpointForwardL);
        // Right shoulder line check
        Vector3 playerTopR = new Vector3(transform.position.x, transform.position.y + (m_playerHeight / 2), transform.position.z);
        Vector3 topWithOffsetR = ((transform.right.normalized * (m_playerWidth / 2)) + playerTopR);
        Vector3 checkOriginR = ((-transform.forward.normalized * (m_playerWidth / 2)) + topWithOffsetR);
        Vector3 endpointTopR = ((Vector3.up.normalized * (m_playerHeight * 0.75f)) + checkOriginR);
        Vector3 endpointForwardR = ((transform.forward.normalized * (m_playerWidth * 2)) + endpointTopR);
        Vector3 endpointDiveR = ((-Vector3.up.normalized * (m_playerHeight)) + endpointForwardR);
        // Mid line check
        Vector3 playerTop = new Vector3(transform.position.x, transform.position.y + (m_playerHeight / 2), transform.position.z);
        Vector3 checkOrigin = ((-transform.forward.normalized * (m_playerWidth / 2)) + playerTop);
        Vector3 endpointTop = ((Vector3.up.normalized * (m_playerHeight * 0.75f)) + checkOrigin);
        Vector3 endpointForward = ((transform.forward.normalized * (m_playerWidth * 2)) + endpointTop);
        Vector3 endpointDive = ((-Vector3.up.normalized * (m_playerHeight)) + endpointForward);

        // LEFT
        Debug.DrawLine(checkOriginL, endpointTopL, Color.blue);
        Debug.DrawLine(endpointTopL, endpointForwardL, Color.blue);
        Debug.DrawLine(endpointForwardL, endpointDiveL, Color.blue);

        // RIGHT
        Debug.DrawLine(checkOriginR, endpointTopR, Color.blue);
        Debug.DrawLine(endpointTopR, endpointForwardR, Color.blue);
        Debug.DrawLine(endpointForwardR, endpointDiveR, Color.blue);

        // MID
        Debug.DrawLine(checkOrigin, endpointTop, Color.blue);
        Debug.DrawLine(endpointTop, endpointForward, Color.blue);
        Debug.DrawLine(endpointForward, endpointDive, Color.blue);

        // Mid Check
        if (!Physics.Linecast(checkOrigin, endpointTop))
            if (!Physics.Linecast(endpointTop, endpointForward))
            {
                RaycastHit mHit;
                if (Physics.Linecast(endpointForward, endpointDive, out mHit, m_vaultCheckMask))
                {
                    m_vaultpos = new Vector3(mHit.point.x, mHit.point.y + (m_playerHeight/2), mHit.point.z);
                    // Left check
                    if (!Physics.Linecast(checkOriginL, endpointTopL))
                        if (!Physics.Linecast(endpointTopL, endpointForwardL))
                        {
                            RaycastHit lHit;
                            if (Physics.Linecast(endpointForwardL, endpointDiveL, out lHit, m_vaultCheckMask))
                            {
                                // Right check
                                if (!Physics.Linecast(checkOriginR, endpointTopR))
                                    if (!Physics.Linecast(endpointTopR, endpointForwardR))
                                    {
                                        RaycastHit rHit;
                                        if (Physics.Linecast(endpointForwardR, endpointDiveR, out rHit, m_vaultCheckMask))
                                        {
                                            return true;
                                        }
                                    }
                            }
                        }
                }
            }
        // At lest one ray did not hit final target
        return false;
    }

    private void Vault()
    {
        // NOTE: Assumes climbTarget roots are only rotated in x
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.position = m_vaultpos;
        m_climbingIK.m_ikActive = false;
        m_playerController.DeactivateLedgeClimbing();
    }

    // NOTE: Should stop all interpolations and release player
    // From use when player is climbing a pillar and the pillar goes into the ground or in similar situations
    private void InterruptTransition()
    {
        //StopCoroutine(MoveAnimation());
    }

    private void MoveToTarget(ClimbTarget newTarget)
    {
        print("MoveToTarget");
        StartCoroutine(Move(newTarget));
        //MoveRightAnimation(newTarget);

        //m_currentClimbTarget = newTarget;
        //m_climbingIK.SetAllClimbTargets(m_currentClimbTarget);
        //transform.position = m_currentClimbTarget.GetRootTarget().transform.position;
    }

    IEnumerator Move(ClimbTarget newTarget)
    {
        m_inTransition = true;

        yield return new WaitForSeconds(m_transitionDuration);
        m_currentClimbTarget = newTarget;
        m_climbingIK.SetAllClimbTargets(m_currentClimbTarget);
        transform.position = m_currentClimbTarget.GetRootTarget().transform.position;
        transform.rotation = m_currentClimbTarget.GetRootTarget().transform.rotation;

        m_inTransition = false;
        // Move half limbs fully in direction
        // Move root halfway
        // Move other half fully in direction

    }

    //IEnumerator MoveLeftAnimation()
    //{
    //    m_climbingIK.set
    //    // Move half limbs fully in direction
    //    // Move root halfway
    //    // Move other half fully in direction

    //}

    //IEnumerator MoveLeftAnimation()
    //{
    //    m_climbingIK.set
    //    // Move half limbs fully in direction
    //    // Move root halfway
    //    // Move other half fully in direction

    //}

    //IEnumerator StepRight(ClimbTarget newTarget)
    //{
    //    StartCoroutine(LerpFloat(1.0f, 0.5f, m_climbingIK.m_rightIkWeight, m_transitionDuration));
    //}

    //IEnumerator MoveRightAnimation(ClimbTarget newTarget)
    //{
    //    m_inTransition = true;

    //    // Move right side
    //    yield return LerpFloat(1.0f, 0.5f, m_climbingIK.m_rightIkWeight, m_transitionDuration);
    //    m_climbingIK.SetRightClimbTargets(newTarget);
    //    yield return LerpFloat(0.5f, 1.0f, m_climbingIK.m_rightIkWeight, m_transitionDuration);

    //    // Move left side
    //    yield return LerpFloat(1.0f, 0.5f, m_climbingIK.m_leftIkWeight, m_transitionDuration);
    //    m_climbingIK.SetLeftClimbTargets(newTarget);
    //    yield return LerpFloat(0.5f, 1.0f, m_climbingIK.m_leftIkWeight, m_transitionDuration);

    //    m_inTransition = false;
    //}

    //IEnumerator LerpFloat(float from, float to, float value, float duration)
    //{
    //    for (float t = 0.0f; t < duration; t += Time.deltaTime)
    //    {
    //        value = Mathf.Lerp(from, to, t / duration);
    //        yield return null;
    //    }
    //    value = to;
    //}

    // Regular Climbing State
    // Climb from side to side
    // Detect edges
    // Wall Climbing State
    // Go to regular when on top and bottom edge
    // Find Edge
    // Vault
    // Drop

}
