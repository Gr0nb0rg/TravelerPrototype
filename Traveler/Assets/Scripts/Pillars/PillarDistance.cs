using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
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
    public bool useOnce;

    public float initalSpeed;
    public float minSpeed;

    private bool goingBack = false;
    private bool m_pillarInMotion = false;
    private bool active = false;
    private bool inRange = false;
    private bool hasActivated = false;
    private bool end = false;

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
        if (!active) return;

        VelocityUpdate();
    }

    private void EditTarget()
    {
        if (m_targets.Count > 1 && currentNum < m_targets.Count && !goingBack)
        {
            currentNum++;

            if (currentNum < m_targets.Count)
            {
                if (rotateToTarget)
                    transform.LookAt(m_targets[currentNum]);

                m_direction = m_targets[currentNum] - transform.position;
                m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
            }
        }
        else if (reverse)
        {
            currentNum--;
            if (currentNum == -1 && !alwaysActive)
            {
                active = false;
                m_rigidbody.velocity = Vector3.zero;
                m_distance = 0;
                currentNum = 1;
                m_rigidbody.isKinematic = true;
                goingBack = false;
                return;
            }
            else if(currentNum == -1)
            {
                active = false;
                currentNum = 1;
                m_rigidbody.isKinematic = true;
                goingBack = false;
            }
            m_direction = m_targets[currentNum] - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);

        }

        if (currentNum == m_targets.Count && !reverse)
        {
            active = false;
            m_rigidbody.velocity = Vector3.zero;
            m_distance = 0;
            m_rigidbody.isKinematic = true;
            if (!alwaysActive)
                end = true;
            return;
        }
        else if(currentNum == m_targets.Count && reverse)
        {
            currentNum = m_targets.Count - 2;
            goingBack = true;

            if (rotateToTarget)
                transform.LookAt(m_targets[currentNum]);

            m_direction = m_targets[currentNum] - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        }   
    }

    private bool CheckDistance()
    {
        if (m_distance < 0.4f && !inRange)
        {
            m_distance = 0f;
            inRange = true;
            return true;
        }

        m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);

        if (inRange && m_distance > 0.4f)
        {
            inRange = false;
        }
        return false;
    }

    public override void Interact(){

        if (active || end) return;

        if (useOnce && hasActivated)
            return;

        hasActivated = true;
        active = true;
        m_direction = m_targets[currentNum] - transform.position;
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        if (usePhysics)
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


    void OnDrawGizmos()
    {
        if (alwaysActive && !reverse)
            reverse = true;

        for (int i = 0; i < m_transforms.Count - 1; i++)
        {
            if (m_transforms[i] == null || m_transforms[i + 1] == null)
            {
                break;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_transforms[i].position, m_transforms[i + 1].position);
            Gizmos.DrawCube(m_transforms[i].position, new Vector3(1, 1, 1));
        }
        
        Gizmos.DrawCube(m_transforms[m_transforms.Count - 1].position, new Vector3(1, 1, 1));

    }
}
