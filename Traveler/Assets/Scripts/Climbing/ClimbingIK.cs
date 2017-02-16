using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: m_IkWeight73m_IkWeight65428m_IkWeight
    Last Edited: 2m_IkWeightm_IkWeight7/m_IkWeight2/m_IkWeight4
*/

/*
    SUMMARY
    Places Hands, Feet and other IK limbs at specified positions
    Rotates Hands, Feet and other IK limbs to specified rotations
*/

public class ClimbingIK : MonoBehaviour {
    private Animator m_animator;
    public bool m_ikActive = false;

    public float m_leftIkWeight = 1;
    public float m_rightIkWeight = 1;

    public Transform m_rightHandTarget = null;
    public Transform m_leftHandTarget = null;
    public Transform m_rightFootTarget = null;
    public Transform m_leftFootTarget = null;

    public Transform m_rightArmHint = null;
    public Transform m_leftArmHint = null;
    public Transform m_rightLegHint = null;
    public Transform m_leftLegHint = null;

    public Transform m_root = null;

    public Transform m_lookObj = null;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
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
                    //m_animator.SetLookAtWeight(m_IkWeight);
                    //m_animator.SetLookAtPosition(m_lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (m_rightHandTarget != null)
                {
                    if (m_rightArmHint != null)
                        SetHintPosIK(AvatarIKHint.RightElbow, m_rightArmHint.position, m_rightIkWeight);
                    SetPosIK(AvatarIKGoal.RightHand, m_rightHandTarget.position, m_rightIkWeight);
                    SetRotIK(AvatarIKGoal.RightHand, m_rightHandTarget.rotation, m_rightIkWeight);
                }
                // Set the left hand target position and rotation, if one has been assigned
                if (m_leftHandTarget != null)
                {
                    if (m_leftArmHint != null)
                        SetHintPosIK(AvatarIKHint.LeftElbow, m_leftArmHint.position, m_leftIkWeight);
                    SetPosIK(AvatarIKGoal.LeftHand, m_leftHandTarget.position, m_leftIkWeight);
                    SetRotIK(AvatarIKGoal.LeftHand, m_leftHandTarget.rotation, m_leftIkWeight);
                }
                // Set the left foot target position and rotation, if one has been assigned
                if (m_leftFootTarget != null)
                {
                    if (m_leftLegHint != null)
                        SetHintPosIK(AvatarIKHint.LeftKnee, m_leftLegHint.position, m_leftIkWeight);
                    SetPosIK(AvatarIKGoal.LeftFoot, m_leftFootTarget.position, m_leftIkWeight);
                    SetRotIK(AvatarIKGoal.LeftFoot, m_leftFootTarget.rotation, m_leftIkWeight);
                }
                // Set the right foot target position and rotation, if one has been assigned
                if (m_rightFootTarget != null)
                {
                    if (m_rightLegHint != null)
                        SetHintPosIK(AvatarIKHint.RightKnee, m_rightLegHint.position, m_rightIkWeight);
                    SetPosIK(AvatarIKGoal.RightFoot, m_rightFootTarget.position, m_rightIkWeight);
                    SetRotIK(AvatarIKGoal.RightFoot, m_rightFootTarget.rotation, m_rightIkWeight);
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
                //m_animator.SetLookAtWeight(0);
            }
        }
    }

    public void SetLeftClimbTargets(ClimbTarget newTarget)
    {
        m_leftHandTarget = newTarget.GetLeftHandTarget();
        m_leftFootTarget = newTarget.GetLeftFootTarget();
        m_leftArmHint = newTarget.GetLeftArmHint();
        m_leftLegHint = newTarget.GetLeftLegHint();
        m_root.position = (m_root.transform.position + newTarget.GetRootTarget().position) / 2;
        m_root.rotation = newTarget.GetRootTarget().rotation;
    }

    public void SetRightClimbTargets(ClimbTarget newTarget)
    {
        m_rightHandTarget = newTarget.GetRightHandTarget();
        m_rightFootTarget = newTarget.GetRightFootTarget();
        m_rightArmHint = newTarget.GetRightArmHint();
        m_rightLegHint = newTarget.GetRightLegHint();
        m_root.position = (m_root.transform.position + newTarget.GetRootTarget().position) / 2;
        m_root.rotation = newTarget.GetRootTarget().rotation;

    }

    public void SetAllClimbTargets(ClimbTarget newTarget)
    {
        m_rightHandTarget = newTarget.GetRightHandTarget();
        m_leftHandTarget = newTarget.GetLeftHandTarget();
        m_rightFootTarget = newTarget.GetRightFootTarget();
        m_leftFootTarget = newTarget.GetLeftFootTarget();
        m_rightArmHint = newTarget.GetRightArmHint();
        m_leftArmHint = newTarget.GetLeftArmHint();
        m_rightLegHint = newTarget.GetRightLegHint();
        m_leftLegHint = newTarget.GetLeftLegHint();
        m_root = newTarget.GetRootTarget();
    }

    public void SetHintPosIK(AvatarIKHint hint, Vector3 targetPos, float weight)
    {
        m_animator.SetIKHintPosition(hint, targetPos);
        m_animator.SetIKHintPositionWeight(hint, weight);
    }

    public void SetPosIK(AvatarIKGoal goal, Vector3 targetPos, float weight)
    {
        m_animator.SetIKPosition(goal, targetPos);
        m_animator.SetIKPositionWeight(goal, weight);
    }

    public void SetRotIK(AvatarIKGoal goal, Quaternion targetRot, float weight)
    {
        m_animator.SetIKRotationWeight(goal, weight);
        m_animator.SetIKRotation(goal, targetRot);
    }

    //public float GetLeftIKWeight()
    //{
    //    return m_leftIkWeight;
    //}

    //public float GetRightIKWeight()
    //{
    //    return m_rightIkWeight;
    //}
}
