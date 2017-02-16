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
    Represents a position for player and IK positioning when climbing ledges
*/

[ExecuteInEditMode]
public class ClimbTarget : MonoBehaviour {
    // PUBLIC 
    public bool m_refreshPoints;

    // PRIVATE SERALIZEABLE
    [SerializeField]
    private ClimbTarget m_rightNeighbour = null;
    [SerializeField]
    private ClimbTarget m_leftNeighbour = null;

    // PRIVATE
    private Transform m_rightHandTarget = null;
    private Transform m_leftHandTarget = null;
    private Transform m_rightFootTarget = null;
    private Transform m_leftFootTarget = null;

    private Transform m_rightArmHint = null;
    private Transform m_leftArmHint = null;
    private Transform m_rightLegHint = null;
    private Transform m_leftLegHint = null;

    private Transform m_root = null;

    #if UNITY_EDITOR
    void Update () {
        if (m_refreshPoints)
        {
            if ((m_rightHandTarget = transform.Find("rightHand")) == null)
                print("rightHand not found");
            if ((m_leftHandTarget = transform.Find("leftHand")) == null)
                print("leftHand not found");
            if ((m_rightFootTarget = transform.Find("rightFoot")) == null)
                print("rightFoot not found");
            if ((m_leftFootTarget = transform.Find("leftFoot")) == null)
                print("leftFoot not found");

            if ((m_rightArmHint = transform.Find("rightArmHint")) == null)
                print("rightArmHint not found");
            if ((m_leftArmHint = transform.Find("leftArmHint")) == null)
                print("leftArmHint not found");
            if ((m_rightLegHint = transform.Find("rightLegHint")) == null)
                print("rightLegHint not found");
            if ((m_leftLegHint = transform.Find("leftLegHint")) == null)
                print("leftLegHint not found");

            if ((m_root = transform.Find("root")) == null)
                print("root not found");

            m_refreshPoints = false;
        }
    }
    #endif

    public ClimbTarget GetLeftNeighbour() { return m_leftNeighbour; }
    public ClimbTarget GetRightNeighbour() { return m_rightNeighbour; }

    public Transform GetRightHandTarget() { return m_rightHandTarget; }
    public Transform GetLeftHandTarget() { return m_leftHandTarget; }
    public Transform GetRightFootTarget() { return m_rightFootTarget; }
    public Transform GetLeftFootTarget() { return m_leftFootTarget; }
    public Transform GetRightArmHint() { return m_rightArmHint; }
    public Transform GetLeftArmHint() { return m_leftArmHint; }
    public Transform GetRightLegHint() { return m_rightLegHint; }
    public Transform GetLeftLegHint() { return m_leftLegHint; }
    public Transform GetRootTarget() { return m_root; }
}
