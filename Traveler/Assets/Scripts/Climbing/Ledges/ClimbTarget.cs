using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281
    Last Edited: 2017/02/21
*/

/*
    SUMMARY
    Represents a position for player and IK positioning when climbing ledges
*/
[System.Serializable]
public class ClimbTarget : MonoBehaviour {
    // PRIVATE
    [SerializeField]
    private ClimbTarget m_rightNeighbour = null;
    [SerializeField]
    private ClimbTarget m_leftNeighbour = null;
    [SerializeField]
    private ClimbTarget m_prefabToSample = null;

    private Transform m_rightHandTarget = null;
    private Transform m_leftHandTarget = null;
    private Transform m_rightFootTarget = null;
    private Transform m_leftFootTarget = null;

    private Transform m_rightArmHint = null;
    private Transform m_leftArmHint = null;
    private Transform m_rightLegHint = null;
    private Transform m_leftLegHint = null;

    private Transform m_root = null;

    private bool m_showTargetsInInspector = false;

    void Awake()
    {
        //AssignCustomPositions();
        AssignPositions();
    }

    public void AssignPositions()
    {
        if (m_prefabToSample == null)
            AssignCustomPositions();
        else
            AssignPrefabPositions();
    }

    private void AssignPrefabPositions()
    {
        AssignCustomPositions();

        m_rightHandTarget.localPosition = m_prefabToSample.transform.FindChild("rightHand").localPosition;
        m_rightHandTarget.localRotation = m_prefabToSample.transform.FindChild("rightHand").localRotation;

        m_leftHandTarget.localPosition = m_prefabToSample.transform.FindChild("leftHand").localPosition;
        m_leftHandTarget.localRotation = m_prefabToSample.transform.FindChild("leftHand").localRotation;

        m_rightFootTarget.localPosition = m_prefabToSample.transform.FindChild("rightFoot").localPosition;
        m_rightFootTarget.localRotation = m_prefabToSample.transform.FindChild("rightFoot").localRotation;

        m_leftFootTarget.localPosition = m_prefabToSample.transform.FindChild("leftFoot").localPosition;
        m_leftFootTarget.localRotation = m_prefabToSample.transform.FindChild("leftFoot").localRotation;


        m_rightArmHint.localPosition = m_prefabToSample.transform.FindChild("rightArmHint").localPosition;
        m_rightArmHint.localRotation = m_prefabToSample.transform.FindChild("rightArmHint").localRotation;

        m_leftArmHint.localPosition = m_prefabToSample.transform.FindChild("leftArmHint").localPosition;
        m_leftArmHint.localRotation = m_prefabToSample.transform.FindChild("leftArmHint").localRotation;

        m_rightLegHint.localPosition = m_prefabToSample.transform.FindChild("rightLegHint").localPosition;
        m_rightLegHint.localRotation = m_prefabToSample.transform.FindChild("rightLegHint").localRotation;

        m_leftLegHint.localPosition = m_prefabToSample.transform.FindChild("leftLegHint").localPosition;
        m_leftLegHint.localRotation = m_prefabToSample.transform.FindChild("leftLegHint").localRotation;

        m_root.localPosition = m_prefabToSample.transform.FindChild("root").localPosition;
        m_root.localRotation = m_prefabToSample.transform.FindChild("root").localRotation;
    }

    private void AssignCustomPositions()
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
    }

    void OnDrawGizmosSelected()
    {
        //AssignPositions();
        //print("DREW");
        DrawSelf();
        DrawNeighbours();
        if(m_showTargetsInInspector)
            DrawTargets();
    }

    void DrawSelf()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 cubeDimensions = new Vector3(0.1f, 0.1f, 0.1f);

        Gizmos.color = new Color(1, 0, 1, 1);
        Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);

        Gizmos.color = new Color(1, 0, 1, 0.4f);
        Gizmos.DrawCube(Vector3.zero, cubeDimensions);
    }

    void DrawNeighbours()
    {
        if(m_leftNeighbour != null)
        {
            Gizmos.matrix = m_leftNeighbour.transform.localToWorldMatrix;
            Vector3 cubeDimensions = new Vector3(0.1f, 0.1f, 0.1f);

            Gizmos.color = new Color(1, 0, 1, 0.2f);
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);

            Gizmos.color = new Color(1, 0, 1, 0.2f);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_rightNeighbour != null)
        {
            Gizmos.matrix = m_rightNeighbour.transform.localToWorldMatrix;
            Vector3 cubeDimensions = new Vector3(0.1f, 0.1f, 0.1f);

            Gizmos.color = new Color(1, 0, 1, 0.2f);
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);

            Gizmos.color = new Color(1, 0, 1, 0.2f);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
    }

    public void DrawTargets()
    {
        //  private Transform m_rightHandTarget = null;
        //  private Transform m_leftHandTarget = null;
        //  private Transform m_rightFootTarget = null;
        //  private Transform m_leftFootTarget = null;

        //  private Transform m_rightArmHint = null;
        //  private Transform m_leftArmHint = null;
        //  private Transform m_rightLegHint = null;
        //  private Transform m_leftLegHint = null;

        Vector3 cubeDimensions = new Vector3(0.1f, 0.1f, 0.1f);
        Gizmos.color = Gizmos.color = new Color(0, 0, 1, 1f);

        if (m_rightHandTarget != null)
        {

            Gizmos.matrix = m_rightHandTarget.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_leftHandTarget != null)
        {
            Gizmos.matrix = m_leftHandTarget.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_rightFootTarget != null)
        {

            Gizmos.matrix = m_rightFootTarget.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_leftFootTarget != null)
        {

            Gizmos.matrix = m_leftFootTarget.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }

        if (m_root != null)
        {
            Gizmos.matrix = m_root.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions * 2);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions * 2);
        }

        Gizmos.color = Gizmos.color = new Color(0, 0, 1, 0.5f);
        if (m_rightArmHint != null)
        {

            Gizmos.matrix = m_rightArmHint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_leftArmHint != null)
        {

            Gizmos.matrix = m_leftArmHint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_rightLegHint != null)
        {

            Gizmos.matrix = m_rightLegHint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
        if (m_leftLegHint != null)
        {

            Gizmos.matrix = m_leftLegHint.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, cubeDimensions);
            Gizmos.DrawCube(Vector3.zero, cubeDimensions);
        }
    }

    public ClimbTarget LeftNeigbour { get { return m_leftNeighbour; } set { m_leftNeighbour = value; } }
    public ClimbTarget RightNeigbour { get { return m_rightNeighbour; } set { m_rightNeighbour = value; } }

    public Transform RightHandTarget { get { return m_rightHandTarget; } }
    public Transform LeftHandTarget { get { return m_leftHandTarget; } }
    public Transform RightFootTarget { get { return m_rightFootTarget; } }
    public Transform LeftFootTarget { get { return m_leftFootTarget; } }
    public Transform RightArmHint { get { return m_rightArmHint; } }
    public Transform LeftArmHint { get { return m_leftArmHint; } }
    public Transform RightLegHint { get { return m_rightLegHint; } }
    public Transform LeftLegHint { get { return m_leftLegHint; } }
    public Transform RootTarget { get { return m_root; } }

    public bool ShowTargets { set { m_showTargetsInInspector = value; } }
}
