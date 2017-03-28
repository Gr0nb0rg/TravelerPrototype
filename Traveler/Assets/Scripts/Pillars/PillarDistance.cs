using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PillarDistance : AbstractInteractable
{

    public List<Transform> m_transforms;
    public List<float> m_times;

    private List<PillarTarget> m_targets;
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
    private float m_time = 0;

    private Vector3 m_direction;

    private int currentNum = 1;

    void Start()
    {
        m_targets = new List<PillarTarget>();
        try
        {
            m_time = m_times[0];
        }
        catch (Exception)
        {
            m_time = 0;   
            Debug.Log("ADD TIME");
        }
        for (int i = 0; i < m_transforms.Count; i++)
        {
            try
            {
                m_targets.Add(new PillarTarget(m_transforms[i].position, m_times[i]));
            }
            catch (Exception e)
            {
                m_targets.Add(new PillarTarget(m_transforms[i].position, 0));
                Debug.Log("Exception in adding pillartargets: " + e);
            }
            
        }

        m_rigidbody = GetComponent<Rigidbody>();

        if (rotateToTarget)
        {
            transform.LookAt(m_targets[currentNum].m_position);
        }
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
        m_direction = m_targets[currentNum].m_position - transform.position;

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

        if (m_time > 0)
        {
            m_time -= Time.deltaTime;
            return;
        }

        if (CheckDistance())
        {
            EditTarget();
            if(m_time > 0)
                return;
        }
        if (!active) return;

        VelocityUpdate();
    }

    private void EditTarget()
    {
        if (m_targets.Count > 1 && currentNum < m_targets.Count && !goingBack)
        {
            currentNum++;
            m_time = m_targets[currentNum - 1].m_time;
            if (currentNum < m_targets.Count)
            {  
                if (rotateToTarget)
                    transform.LookAt(m_targets[currentNum].m_position);

                m_direction = m_targets[currentNum].m_position - transform.position;
                m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
            }
        }
        else if (reverse)
        {
            currentNum--;
            if (currentNum == -1 && !alwaysActive)
            {
                m_time = m_targets[currentNum + 1].m_time;
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

            m_time = m_targets[currentNum + 1].m_time;
            m_direction = m_targets[currentNum].m_position - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);

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
                transform.LookAt(m_targets[currentNum].m_position);

            m_direction = m_targets[currentNum].m_position - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
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

        m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);

        if (inRange && m_distance > 0.4f)
        {
            inRange = false;
        }
        return false;
    }

    public override void Interact(){

        Debug.Log("Activate!");

        if (active || end) return;

        if (useOnce && hasActivated)
            return;

        hasActivated = true;
        active = true;
        m_direction = m_targets[currentNum].m_position - transform.position;
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
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
