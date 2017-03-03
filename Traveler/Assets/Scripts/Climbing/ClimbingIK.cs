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
    Places Hands, Feet and other IK limbs at specified positions
    Rotates Hands, Feet and other IK limbs to specified rotations
    Animates transitions when climbing
*/

public class ClimbingIK : MonoBehaviour {
    // Components
    private Animator m_animator;
    private bool m_ikActive = false;

    private float m_lookIkWeight = 1;
    private float m_leftIkWeight = 1;
    private float m_rightIkWeight = 1;

    // Actual climb target transforms player is currenly at
    // TODO: Save current climb target instead
    private Transform m_rightHandTarget;
    private Transform m_leftHandTarget;
    private Transform m_rightFootTarget;
    private Transform m_leftFootTarget;

    private Transform m_root;

    private Transform m_rightArmHint;
    private Transform m_leftArmHint;
    private Transform m_rightLegHint;
    private Transform m_leftLegHint;

    private Transform m_lookObj;

    // Animation
    private enum TransitionStyle { LINEAR, SMOOTH, SPRING, OVERSHOOT, UPWARDCURVE }
    private int m_activeRoutines = 0;

    // These transforms are moved to animate climbing transitions, represents the transform of extremities and root
    private Transform m_animatedLeftHand;
    private Transform m_animatedLeftFoot;
    private Transform m_animatedLeftArmHint;
    private Transform m_animatedLeftLegHint;

    private Transform m_animatedRoot;

    private Transform m_animatedRightHand;
    private Transform m_animatedRightFoot;
    private Transform m_animatedRightArmHint;
    private Transform m_animatedRightLegHint;

    // Tweaking variables
    #region Tweaking field
    // HANDS
    [Header("====================Hands====================")]
    [SerializeField]
    private TransitionStyle m_leadingHandStyle;
    [SerializeField]
    private TransitionStyle m_followingHandStyle;
    [Header("Arc reference points")]
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_handArcSample1 = 0.25f;
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_handArcSample2 = 0.75f;
    [Header("Arc height for reference points (Percent of distance to target)")]
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.4f)]
    [SerializeField]
    private float m_handArcHeigh1 = 0.25f;
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.4f)]
    [SerializeField]
    private float m_handArcHeigh2 = 0.25f;
    [Header("Transition Speeds")]
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_leadingHandTransitionSpeed = 1.9f;
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_followingHandTransitionSpeed = 1.9f;
    [Header("Style specific")]
    [Tooltip("Decrease to loosen the spring bounce")]
    [Range(0.3f, 10.0f)]
    [SerializeField]
    private float m_handSpringTension = 1.9f;
    [Tooltip("Increase to overshoot further")]
    [Range(0.0f, 4.4f)]
    [SerializeField]
    private float m_handOvershootFactor = 1.9f;

    // ROOT
    [Header("====================Root====================")]
    [SerializeField]
    private TransitionStyle m_rootStyle;
    [Header("Arc reference points")]
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_rootArcSample1 = 0.25f;
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_rootArcSample2 = 0.75f;
    [Header("Arc height for reference points")]
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.5f)]
    [SerializeField]
    private float m_rootArcHeigh1 = 0.15f;
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.5f)]
    [SerializeField]
    private float m_rootArcHeigh2 = 0.15f;
    [Header("Transition Speed")]
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_rootTransitionSpeed = 2.5f;
    [Header("Style specific")]
    [Tooltip("Decrease to loosen the spring bounce")]
    [Range(0.3f, 10.0f)]
    [SerializeField]
    private float m_rootSpringTension = 1.9f;
    [Tooltip("Increase to overshoot further")]
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_rootOvershootFactor = 1.9f;

    // FEET
    [Header("====================Feet====================")]
    [SerializeField]
    private TransitionStyle m_leadingFootStyle;
    [SerializeField]
    private TransitionStyle m_followingFootStyle;

    [Header("Arc reference points")]
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_footArcSample1 = 0.25f;
    [Tooltip("0 at origin point and 1 at target")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_footArcSample2 = 0.75f;

    [Header("Arc height for reference points")]
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.4f)]
    [SerializeField]
    private float m_footArcHeigh1 = 0.25f;
    [Tooltip("Height straight up from between points")]
    [Range(0.0f, 0.4f)]
    [SerializeField]
    private float m_footArcHeigh2 = 0.25f;

    [Header("Transition Speeds")]
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_leadingFootTransitionSpeed = 1.9f;
    [Range(1.0f, 4.4f)]
    [SerializeField]
    private float m_followingFootTransitionSpeed = 1.9f;
    [Header("Style specific")]
    [Tooltip("Decrease to loosen the spring bounce")]
    [Range(0.3f, 10.0f)]
    [SerializeField]
    private float m_footSpringTension = 1.9f;
    [Range(0.0f, 4.4f)]
    [Tooltip("Increase to overshoot further")]
    [SerializeField]
    private float m_footOvershootFactor = 1.9f;

    // General
    [Header("====================Wait time in seconds====================")]
    [SerializeField]
    private float m_beforeRootFollows;
    [SerializeField]
    private float m_beforeFollowingSideFollows;

    #endregion

    void Start()
    {
        m_animator = GetComponent<Animator>();
        CreateAnimationTransforms();
    }

    // Callback for calculating IK
    void OnAnimatorIK()
    {
        if (m_animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (m_ikActive)
            {
                // Set the look target position, if one has been assigned
                if (m_lookObj != null)
                {
                    m_animator.SetLookAtWeight(m_lookIkWeight);
                    m_animator.SetLookAtPosition(m_lookObj.position);
                }

                // Make root mimic the animated root transform
                if (m_animatedRoot != null) {
                    transform.parent.transform.position = m_animatedRoot.position;
                    transform.parent.transform.rotation = m_animatedRoot.rotation;
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (m_animatedRightHand != null)
                {
                    if (m_animatedRightArmHint != null)
                        SetHintPosIK(AvatarIKHint.RightElbow, m_animatedRightArmHint.position, m_rightIkWeight);
                    SetPosIK(AvatarIKGoal.RightHand, m_animatedRightHand.position, m_rightIkWeight);
                    SetRotIK(AvatarIKGoal.RightHand, m_animatedRightHand.rotation, m_rightIkWeight);
                }
                // Set the left hand target position and rotation, if one has been assigned
                if (m_animatedLeftHand != null)
                {
                    if (m_animatedLeftArmHint != null)
                        SetHintPosIK(AvatarIKHint.LeftElbow, m_animatedLeftArmHint.position, m_leftIkWeight);
                    SetPosIK(AvatarIKGoal.LeftHand, m_animatedLeftHand.position, m_leftIkWeight);
                    SetRotIK(AvatarIKGoal.LeftHand, m_animatedLeftHand.rotation, m_leftIkWeight);
                }
                // Set the left foot target position and rotation, if one has been assigned
                if (m_animatedLeftFoot != null)
                {
                    if (m_animatedLeftLegHint != null)
                        SetHintPosIK(AvatarIKHint.LeftKnee, m_animatedLeftLegHint.position, m_leftIkWeight);
                    SetPosIK(AvatarIKGoal.LeftFoot, m_animatedLeftFoot.position, m_leftIkWeight);
                    SetRotIK(AvatarIKGoal.LeftFoot, m_animatedLeftFoot.rotation, m_leftIkWeight);
                }
                // Set the right foot target position and rotation, if one has been assigned
                if (m_animatedRightFoot != null)
                {
                    if (m_animatedRightLegHint != null)
                        SetHintPosIK(AvatarIKHint.RightKnee, m_animatedRightLegHint.position, m_rightIkWeight);
                    SetPosIK(AvatarIKGoal.RightFoot, m_animatedRightFoot.position, m_rightIkWeight);
                    SetRotIK(AvatarIKGoal.RightFoot, m_animatedRightFoot.rotation, m_rightIkWeight);
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_rightIkWeight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_rightIkWeight);

                m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_leftIkWeight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_leftIkWeight);

                m_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, m_rightIkWeight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_rightIkWeight);

                m_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, m_leftIkWeight);
                m_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_leftIkWeight);
                m_animator.SetLookAtWeight(m_lookIkWeight);
            }
        }
    }

    #region Assignment help

    /*=============================================================== Help functions for IK assignment ===============================================================*/
    private void SetHintPosIK(AvatarIKHint hint, Vector3 targetPos, float weight)
    {
        m_animator.SetIKHintPosition(hint, targetPos);
        m_animator.SetIKHintPositionWeight(hint, weight);
    }

    private void SetPosIK(AvatarIKGoal goal, Vector3 targetPos, float weight)
    {
        m_animator.SetIKPosition(goal, targetPos);
        m_animator.SetIKPositionWeight(goal, weight);
    }

    private void SetRotIK(AvatarIKGoal goal, Quaternion targetRot, float weight)
    {
        m_animator.SetIKRotationWeight(goal, weight);
        m_animator.SetIKRotation(goal, targetRot);
    }

    // Used when succesfully reaced to a new climb target
    public void SetAllClimbTargets(ClimbTarget newTarget)
    {
        m_rightHandTarget = newTarget.RightHandTarget;
        m_leftHandTarget = newTarget.LeftHandTarget;
        m_rightFootTarget = newTarget.RightFootTarget;
        m_leftFootTarget = newTarget.LeftFootTarget;
        m_rightArmHint = newTarget.RightArmHint;
        m_leftArmHint = newTarget.LeftArmHint;
        m_rightLegHint = newTarget.RightLegHint;
        m_leftLegHint = newTarget.LeftLegHint;
        m_root = newTarget.RootTarget;
        SetAnimationTransforms();
    }

    // Place empty gameobjects in loaded scene that will represent the transform of extremities when animated
    private void CreateAnimationTransforms()
    {
        GameObject ikParent = new GameObject();
        ikParent.name = "IK_Positions";
        ikParent.transform.parent = null;

        GameObject leftHandObject = new GameObject();
        leftHandObject.name = "LeftHandTranIK";
        leftHandObject.transform.parent = ikParent.transform;
        m_animatedLeftHand = leftHandObject.transform;

        GameObject rightHandObject = new GameObject();
        rightHandObject.name = "RighttHandTranIK";
        rightHandObject.transform.parent = ikParent.transform;
        m_animatedRightHand = rightHandObject.transform;

        GameObject leftFootObject = new GameObject();
        leftFootObject.name = "LeftFootTranIK";
        leftFootObject.transform.parent = ikParent.transform;
        m_animatedLeftFoot = leftFootObject.transform;

        GameObject rightFootObject = new GameObject();
        rightFootObject.name = "rightFootTranIK";
        rightFootObject.transform.parent = ikParent.transform;
        m_animatedRightFoot = rightFootObject.transform;

        GameObject leftArmHintObject = new GameObject();
        leftArmHintObject.name = "leftArmHintTranIK";
        leftArmHintObject.transform.parent = ikParent.transform;
        m_animatedLeftArmHint = leftArmHintObject.transform;

        GameObject rightArmHintObject = new GameObject();
        rightArmHintObject.name = "rightArmHintTranIK";
        rightArmHintObject.transform.parent = ikParent.transform;
        m_animatedRightArmHint = rightArmHintObject.transform;

        GameObject leftLegHintObject = new GameObject();
        leftLegHintObject.name = "leftLegHintTranIK";
        leftLegHintObject.transform.parent = ikParent.transform;
        m_animatedLeftLegHint = leftLegHintObject.transform;

        GameObject rightLegHintObject = new GameObject();
        rightLegHintObject.name = "rightLegHintTranIK";
        rightLegHintObject.transform.parent = ikParent.transform;
        m_animatedRightLegHint = rightLegHintObject.transform;

        GameObject rootObject = new GameObject();
        rootObject.name = "rootTranIK";
        rootObject.transform.parent = ikParent.transform;
        m_animatedRoot = rootObject.transform;
    }

    #endregion

    #region SwitchTargetAnimations

    /*
        SUMMARY
         - Takes a new target and intepolates the temporary "m_animated" transforms from current target to new target.
         - Keeps track of all active coroutines with "m_activeRoutines" and notifies ControllerLedgeClimbing.cs found on parent when all interpolations are finished.
         NOTES
         - Only one transition animation can be active at any given time
    */

    public IEnumerator MoveLeftAnimation(ClimbTarget newTarget)
    {
        SetAnimationTransforms();

        StartLeadingLeftRoutines(newTarget);

        yield return new WaitForSeconds(m_beforeRootFollows);
        StartRootRoutie(newTarget);
        yield return new WaitForSeconds(m_beforeFollowingSideFollows);

        StartFollowingLeftRoutines(newTarget);

        while (m_activeRoutines > 0){ /* Wait for animation to finish */ yield return null; }
        SetAllClimbTargets(newTarget);
        GetComponentInParent<ControllerLedgeClimbing>().InTransition = false;
    }

    public IEnumerator MoveRightAnimation(ClimbTarget newTarget)
    {
        SetAnimationTransforms();

        StartLeadingRightRoutines(newTarget);

        yield return new WaitForSeconds(m_beforeRootFollows);
        StartRootRoutie(newTarget);
        yield return new WaitForSeconds(m_beforeFollowingSideFollows);

        StartFollowingRightRoutines(newTarget);

        while (m_activeRoutines > 0) { /* Wait for animation to finish */ yield return null; }
        SetAllClimbTargets(newTarget);
        GetComponentInParent<ControllerLedgeClimbing>().InTransition = false;
    }

    private void SetAnimationTransforms()
    {
        m_animatedLeftHand.position = m_leftHandTarget.position;
        m_animatedLeftHand.rotation = m_leftHandTarget.rotation;
        m_animatedLeftFoot.position = m_leftFootTarget.position;
        m_animatedLeftFoot.rotation = m_leftFootTarget.rotation;
        m_animatedLeftArmHint.position = m_leftArmHint.position;
        m_animatedLeftArmHint.rotation = m_leftArmHint.rotation;
        m_animatedLeftLegHint.position = m_leftLegHint.position;
        m_animatedLeftLegHint.rotation = m_leftLegHint.rotation;

        m_animatedRoot.position = m_root.position;
        m_animatedRoot.rotation = m_root.rotation;

        m_animatedRightHand.position = m_rightHandTarget.position;
        m_animatedRightHand.rotation = m_rightHandTarget.rotation;
        m_animatedRightFoot.position = m_rightFootTarget.position;
        m_animatedRightFoot.rotation = m_rightFootTarget.rotation;
        m_animatedRightArmHint.position = m_rightArmHint.position;
        m_animatedRightArmHint.rotation = m_rightArmHint.rotation;
        m_animatedRightLegHint.position = m_rightLegHint.position;
        m_animatedRightLegHint.rotation = m_rightLegHint.rotation;

    }

    private void StartRootRoutie(ClimbTarget newTarget)
    {
        switch (m_rootStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedRoot, m_root.position, newTarget.RootTarget.position, m_rootTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedRoot, m_root.position, newTarget.RootTarget.position, m_rootTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedRoot, m_root.position, newTarget.RootTarget.position, m_rootTransitionSpeed, m_rootSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedRoot, m_root.position, newTarget.RootTarget.position, m_rootTransitionSpeed, m_rootOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedRoot, m_root.position, newTarget.RootTarget.position, m_rootTransitionSpeed, m_rootArcHeigh1, m_rootArcHeigh2, m_rootArcSample1, m_rootArcSample2));
                break;
            default:
                break;
        }
        StartCoroutine(SmootStepInterpolation(m_animatedRoot, m_root.rotation, newTarget.RootTarget.rotation, m_rootTransitionSpeed));
    }

    private void StartLeadingLeftRoutines(ClimbTarget newTarget)
    {
        switch (m_leadingHandStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_leadingHandTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_leadingHandTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_leadingHandTransitionSpeed, m_handSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed, m_handSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_leadingHandTransitionSpeed, m_handOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed, m_handOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                //StartCoroutine(UpwardCurveInterpolation(m_animatedLeftHand, m_leftHandTarget.rotation, newTarget.LeftHandTarget.rotation, m_leadingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_leadingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                StartCoroutine(SmootStepInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed));
                //StartCoroutine(UpwardCurveInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_leadingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                break;
            default:
                break;
        }

        // Rotation
        StartCoroutine(SmootStepInterpolation(m_animatedLeftHand, m_leftHandTarget.rotation, newTarget.LeftHandTarget.rotation, m_leadingHandTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedLeftArmHint, m_leftArmHint.rotation, newTarget.LeftArmHint.rotation, m_leadingHandTransitionSpeed));

        switch (m_leadingFootStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_leadingFootTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_leadingFootTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_leadingFootTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_leadingFootTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_leadingFootTransitionSpeed, m_footSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_leadingFootTransitionSpeed, m_footSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_leadingFootTransitionSpeed, m_footOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_leadingFootTransitionSpeed, m_footOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_leadingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_leadingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedLeftFoot, m_leftFootTarget.rotation, newTarget.LeftFootTarget.rotation, m_leadingFootTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedLeftLegHint, m_leftLegHint.rotation, newTarget.LeftLegHint.rotation, m_leadingFootTransitionSpeed));
    }

    private void StartFollowingLeftRoutines(ClimbTarget newTarget)
    {
        switch (m_followingHandStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_followingHandTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_followingHandTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_followingHandTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_followingHandTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_followingHandTransitionSpeed, m_handSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_followingHandTransitionSpeed, m_handSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_followingHandTransitionSpeed, m_handOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_followingHandTransitionSpeed, m_handOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_followingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_followingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedRightHand, m_rightHandTarget.rotation, newTarget.RightHandTarget.rotation, m_followingHandTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedRightArmHint, m_rightArmHint.rotation, newTarget.RightArmHint.rotation, m_followingHandTransitionSpeed));

        switch (m_followingFootStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_followingFootTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_followingFootTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_followingFootTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_followingFootTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_followingFootTransitionSpeed, m_footSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_followingFootTransitionSpeed, m_footSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_followingFootTransitionSpeed, m_footOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_followingFootTransitionSpeed, m_footOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_followingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_followingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedRightFoot, m_rightFootTarget.rotation, newTarget.RightFootTarget.rotation, m_followingFootTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedRightLegHint, m_rightLegHint.rotation, newTarget.RightLegHint.rotation, m_followingFootTransitionSpeed));
    }

    private void StartLeadingRightRoutines(ClimbTarget newTarget)
    {
        switch (m_leadingHandStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_leadingHandTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_leadingHandTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_leadingHandTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_leadingHandTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_leadingHandTransitionSpeed, m_handSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_leadingHandTransitionSpeed, m_handSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_leadingHandTransitionSpeed, m_handOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_leadingHandTransitionSpeed, m_handOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightHand, m_rightHandTarget.position, newTarget.RightHandTarget.position, m_leadingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightArmHint, m_rightArmHint.position, newTarget.RightArmHint.position, m_leadingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedRightHand, m_rightHandTarget.rotation, newTarget.RightHandTarget.rotation, m_leadingHandTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedRightArmHint, m_rightArmHint.rotation, newTarget.RightArmHint.rotation, m_leadingHandTransitionSpeed));

        switch (m_leadingFootStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_leadingFootTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_leadingFootTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_leadingFootTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_leadingFootTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_leadingFootTransitionSpeed, m_footSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_leadingFootTransitionSpeed, m_footSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_leadingFootTransitionSpeed, m_footOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_leadingFootTransitionSpeed, m_footOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightFoot, m_rightFootTarget.position, newTarget.RightFootTarget.position, m_leadingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedRightLegHint, m_rightLegHint.position, newTarget.RightLegHint.position, m_leadingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedRightFoot, m_rightFootTarget.rotation, newTarget.RightFootTarget.rotation, m_leadingFootTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedRightLegHint, m_rightLegHint.rotation, newTarget.RightLegHint.rotation, m_leadingFootTransitionSpeed));
    }

    private void StartFollowingRightRoutines(ClimbTarget newTarget)
    {
        switch (m_followingHandStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_followingHandTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_followingHandTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_followingHandTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_followingHandTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_followingHandTransitionSpeed, m_handSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_followingHandTransitionSpeed, m_handSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_followingHandTransitionSpeed, m_handOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_followingHandTransitionSpeed, m_handOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftHand, m_leftHandTarget.position, newTarget.LeftHandTarget.position, m_followingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftArmHint, m_leftArmHint.position, newTarget.LeftArmHint.position, m_followingHandTransitionSpeed, m_handArcHeigh1, m_handArcHeigh2, m_handArcSample1, m_handArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedLeftHand, m_leftHandTarget.rotation, newTarget.LeftHandTarget.rotation, m_followingHandTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedLeftArmHint, m_leftArmHint.rotation, newTarget.LeftArmHint.rotation, m_followingHandTransitionSpeed));

        switch (m_followingFootStyle)
        {
            case TransitionStyle.LINEAR:
                StartCoroutine(LinearInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_followingFootTransitionSpeed));
                StartCoroutine(LinearInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_followingFootTransitionSpeed));
                break;
            case TransitionStyle.SMOOTH:
                StartCoroutine(SmootStepInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_followingFootTransitionSpeed));
                StartCoroutine(SmootStepInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_followingFootTransitionSpeed));
                break;
            case TransitionStyle.SPRING:
                StartCoroutine(SpringInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_followingFootTransitionSpeed, m_footSpringTension));
                StartCoroutine(SpringInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_followingFootTransitionSpeed, m_footSpringTension));
                break;
            case TransitionStyle.OVERSHOOT:
                StartCoroutine(OvershootInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_followingFootTransitionSpeed, m_footOvershootFactor));
                StartCoroutine(OvershootInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_followingFootTransitionSpeed, m_footOvershootFactor));
                break;
            case TransitionStyle.UPWARDCURVE:
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftFoot, m_leftFootTarget.position, newTarget.LeftFootTarget.position, m_followingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                StartCoroutine(UpwardCurveInterpolation(m_animatedLeftLegHint, m_leftLegHint.position, newTarget.LeftLegHint.position, m_followingFootTransitionSpeed, m_footArcHeigh1, m_footArcHeigh2, m_footArcSample1, m_footArcSample2));
                break;
            default:
                break;
        }

        StartCoroutine(SmootStepInterpolation(m_animatedLeftFoot, m_leftFootTarget.rotation, newTarget.LeftFootTarget.rotation, m_followingFootTransitionSpeed));
        StartCoroutine(SmootStepInterpolation(m_animatedLeftLegHint, m_leftLegHint.rotation, newTarget.LeftLegHint.rotation, m_followingFootTransitionSpeed));
    }

    #endregion

    #region InterpolationStyles

    /*
     SUMMARY
     - Coroutines that can be used for rotation and translation when animating climbing
    */

    /*=============================================================== Translation ===============================================================*/
    private IEnumerator LinearInterpolation(Transform moveObject, Vector3 origin, Vector3 target, float speed)
    {
        m_activeRoutines++; // Active
        float time = 0;
        bool active = true;
        Vector3 startPos = origin;
        Vector3 endPos = target;

        while (active)
        {
            time += Time.deltaTime * speed;
            if (time > 1)
            {
                time = 1;
                active = false;

                break;
            }

            Vector3 pos = Vector3.Lerp(startPos, endPos, time);
            moveObject.position = pos;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    private IEnumerator SmootStepInterpolation(Transform moveObject, Vector3 origin, Vector3 target, float speed)
    {
        m_activeRoutines++; // Active
        float t1 = 0;
        bool active = true;
        Vector3 startPos = origin;
        Vector3 endPos = target;

        while (active)
        {
            t1 += Time.deltaTime * speed;

            float t = t1 * t1 * (3-2 * t1);

            if (t1 > 1)
            {
                t = 1;
                active = false;
            }

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            moveObject.position = pos;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    private IEnumerator SpringInterpolation(Transform moveObject, Vector3 origin, Vector3 target, float speed, float factor)
    {
        m_activeRoutines++; // Active
        float t1 = 0;
        bool active = true;
        Vector3 startPos = origin;
        Vector3 endPos = target;
        float duration = 0;
        float distance = Vector3.Distance(startPos, endPos);
        float maxDuration = distance / speed;

        while (active)
        {
            duration += Time.deltaTime;

            float s = speed * Time.deltaTime;

            t1 += s;

            float t = Mathf.Pow(2, -10 * t1) * Mathf.Sin((t1 - factor * 0.25f) * (2 * Mathf.PI) / factor) + 1;

            if(duration > maxDuration)
            {
                t = 1;
                active = false;
            }

            Vector3 pos = Vector3.LerpUnclamped(startPos, endPos, t);
            moveObject.position = pos;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    private IEnumerator OvershootInterpolation(Transform moveObject, Vector3 origin, Vector3 target, float speed, float tension)
    {
        m_activeRoutines++; // Active
        bool active = true;
        float t1 = 0;
        Vector3 startPos = origin;
        Vector3 endPos = target;
        float distance = Vector3.Distance(startPos, endPos);
        float duration = 0;
        float maxDuration = distance / speed;
        float ten = tension * 1.5f;

        while (active) {
            duration += Time.deltaTime;

            float s = speed * Time.deltaTime;
            float lerpSpeed = s / distance;

            t1 += lerpSpeed;

            float t = 0;

            if (t1 < 0.5f)
                t = 0.5f * a(t1 * 2, ten);
            else
                t = 0.5f * (o(t1 * 2 - 2, ten) + 2);

            if(duration > maxDuration)
            {
                active = false;
            }

            Vector3 pos = Vector3.LerpUnclamped(startPos, endPos, t);
            moveObject.position = pos;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    private IEnumerator UpwardCurveInterpolation(Transform moveObject, Vector3 origin, Vector3 target, float speed, float upshootHeight1, float upshootHeight2, float curveSample1Precent, float curveSample2Precent)
    {
        m_activeRoutines++; // Active
        // Initialization
        float t1 = 0;
        Vector3 startPos = origin;
        Vector3 targetPos = target;
        bool active = true;
        float distance = Vector3.Distance(startPos, targetPos);

        Vector3 curvePoint1 = (startPos + (targetPos - startPos) * curveSample1Precent) + Vector3.up * upshootHeight1;
        Vector3 curvePoint2 = (startPos + (targetPos - startPos) * curveSample2Precent) + Vector3.up * upshootHeight2;

        Vector3[] curvingPoints = new Vector3[4];
        curvingPoints[0] = startPos;
        curvingPoints[1] = curvePoint1;
        curvingPoints[2] = curvePoint2;
        curvingPoints[3] = targetPos;

        // Interpolation
        while (active)
        {
            float s = speed * Time.deltaTime;
            float lerpSpeed = s / distance;
            t1 += s;
            if (t1 > 1)
            {
                active = false;
                Vector3 finpos = targetPos;
                moveObject.position = finpos;
                continue;
            }
            Vector3 pos = Bez(t1, curvingPoints[0], curvingPoints[1], curvingPoints[2], curvingPoints[3]);
            moveObject.position = pos;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    /*=============================================================== Rotation Overloads ===============================================================*/

    private IEnumerator LinearInterpolation(Transform moveObject, Quaternion origin, Quaternion target, float speed)
    {
        m_activeRoutines++; // Active
        float time = 0;
        bool active = true;
        Quaternion startRot = origin;
        Quaternion endRot = target;

        while (active)
        {
            time += Time.deltaTime * speed;
            if (time > 1)
            {
                time = 1;
                active = false;

                break;
            }

            Quaternion rot = Quaternion.Lerp(startRot, endRot, time);
            moveObject.rotation = rot;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    private IEnumerator SmootStepInterpolation(Transform moveObject, Quaternion origin, Quaternion target, float speed)
    {
        m_activeRoutines++; // Active
        float t1 = 0;
        bool active = true;
        Quaternion startRot = origin;
        Quaternion endRot = target;

        while (active)
        {
            t1 += Time.deltaTime * speed;

            float t = t1 * t1 * (3 - 2 * t1);

            if (t1 > 1)
            {
                t = 1;
                active = false;
            }

            Quaternion rot = Quaternion.Lerp(startRot, endRot, t);
            moveObject.rotation = rot;
            yield return null;
        }
        m_activeRoutines--; // Not Active
    }

    /*=============================================================== Help functions ===============================================================*/

    private float a(float t, float tension)
    {
        return t * t * ((tension + 1) * t - tension);
    }

    private float o(float t, float tension)
    {
        return t * t * ((tension + 1) * t + tension);
    }

    private Vector3 Bez(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 p = u3 * p0;
        p += 3 * u2 * t * p1;
        p += 3 * u * t2 * p2;
        p += t3 * p3;

        return p;
    }

    #endregion

    // Getters and setters

    public bool IK_Active { get { return m_ikActive; } set { m_ikActive = value;  } }
    public Transform Root { get { return m_animatedRoot; } }
}
