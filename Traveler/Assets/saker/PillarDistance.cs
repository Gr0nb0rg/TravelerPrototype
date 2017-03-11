using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
[ExecuteInEditMode]
public class PillarDistance : AbstractInteractable
{

    public List<Transform> m_transforms;

    private List<Vector3> m_targets;
    private Transform startPosition;
  
    public bool rotateToTarget;
    public bool reverse;
    public bool usePhysics;
    public bool useMinSpeed;
    public bool alwaysActive;

    public float initalSpeed;
    public float minSpeed;


    private bool m_pillarInMotion = false;
    private bool active = false;
    private bool inRange = false;
    private bool goingBack = false;

    private Rigidbody m_rigidbody;

    private float m_distance = 0f;

    private Vector3 m_direction;

    private int currentNum = 1;

    void Start (){
        m_targets = new List<Vector3>();

        foreach (Transform t in m_transforms)
        {
            m_targets.Add(t.position);
        }

        m_rigidbody = GetComponent<Rigidbody>();

        if (rotateToTarget)
        {
            transform.LookAt(m_targets[currentNum]);
        }
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        m_direction = m_targets[currentNum] - transform.position;

    }

    void Update()
    {
        #if UNITY_EDITOR
        if (m_targets.Count >= 2)
        {
            for (int i = 0; i < m_targets.Count - 1; i++)
            {
                if (m_targets[i] == null || m_targets[i + 1] == null){
                    break;
                }


                Debug.DrawLine(m_targets[i], m_targets[i + 1], Color.red);
            }
        }
        #endif


        /*if (Input.GetKeyDown(KeyCode.E) && !active)
        {
            currentNum = 1;
            active = true;
            if(usePhysics)
                m_rigidbody.isKinematic = false;
        }*/
        if (alwaysActive)
        {
            active = true;
            if (usePhysics)
                m_rigidbody.isKinematic = false;
        }

        if (!active) return;

        if (CheckDistance())
        {
            EditTarget();
        }

        VelocityUpdate();
    }

    private void EditTarget()
    {
        Debug.Log("EditTarget");
        if (m_targets.Count > 1 && currentNum != m_targets.Count - 1 && !goingBack)
        {
            
            currentNum++;
            Debug.Log("increase currentNum to: " + currentNum);
            if (rotateToTarget)
              transform.LookAt(m_targets[currentNum]);

            m_direction = m_targets[currentNum] - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        }
        else if (reverse)
        {

            goingBack = true;
            currentNum--;
            if (currentNum == -1)
            {
                Debug.Log("Reset");
                active = false;
                m_rigidbody.velocity = Vector3.zero;
                m_distance = 0;
                m_direction = Vector3.zero;
                currentNum = 1;
                goingBack = false;
                m_rigidbody.isKinematic = true;
            }
            Debug.Log("decrease currentNum to: " + currentNum);
            m_direction = m_targets[currentNum] - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
            Debug.Log("Dir: " + m_direction + " Dist: " + m_distance);

        }
    }

    private bool CheckDistance()
    {
        if (m_distance < 0.4f && !inRange)
        {
            Debug.Log("Inrange");
            m_distance = 0f;
            inRange = true;
            return true;
        }

        m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);

        if (inRange && m_distance > 0.5f)
        {
            Debug.Log("Not Inrange");
            inRange = false;
        }

        Debug.Log("Check distance false");
        return false;
    }

    public override void Interact(){

        if (active) return;


        currentNum = 1;
        active = true;
        if(usePhysics)
            m_rigidbody.isKinematic = false;
    }

    private void VelocityUpdate(){
       

        var speed = (initalSpeed*m_distance)/25;

        if (speed < minSpeed/25 && useMinSpeed && m_distance != 0)
            speed = minSpeed/25;

        if (usePhysics)
        {
            m_rigidbody.velocity = (m_direction * speed);
            return;
        }
        else
        {
            transform.position += (m_direction*speed*Time.deltaTime);
        }
    }

}
