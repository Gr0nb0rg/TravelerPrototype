﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLooping : AbstractInteractable
{
    public List<Transform> m_transforms;
    public List<float> m_times;

    public List<AbstractInteractable> signalList;

    public bool rotateToTarget;
    public bool useMinSpeed;
    public bool usePhysics;
    public bool constantSpeed;
    public bool alwaysActive;
    public bool stopAtStart;

    public float initalSpeed;
    public float minSpeed;

    private bool inRange = false;
    private bool active = false;
    private bool reachedEnd = false;

    private List<PillarTarget> m_targets;
    private Transform startPosition;

    private Rigidbody m_rigidbody;

    private float m_time;
    private float m_distance = 0f;

    private Vector3 m_direction;

    private int currentNum = 1;

    void Start () {
        if (alwaysActive)
            active = true;

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

    void Update () {
        if (!active) return;

        if (usePhysics)
            m_rigidbody.isKinematic = false;

        if (m_time > 0)
        {
            m_time -= Time.deltaTime;
            return;
        }

        if (CheckDistance())
        {
            EditTarget();
            if (m_time > 0)
                return;
        }

        VelocityUpdate();
    }

    private void EditTarget()
    {
        if (m_targets.Count > 1 && currentNum == m_targets.Count - 1)
        {
            if (stopAtStart)
                reachedEnd = true;

            m_time = m_targets[0].m_time;
            currentNum = 0;
            m_direction = m_targets[currentNum].m_position - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
        }
        else
        {
            currentNum++;
            m_time = m_targets[currentNum - 1].m_time;

            if (!alwaysActive && currentNum == 1 && reachedEnd)
            {
                active = false;
                m_rigidbody.isKinematic = true;
                return;
            }

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

    private void VelocityUpdate()
    {
        float speed;
        if (constantSpeed)
            speed = initalSpeed / 25;
        else
            speed = (initalSpeed * m_distance) / 25;

        if (speed < minSpeed / 25 && useMinSpeed && m_distance != 0 && !constantSpeed)
            speed = minSpeed / 25;

        if (usePhysics)
        {
            m_rigidbody.velocity = (m_direction * speed);
            return;
        }
        else
        {
            transform.position += (m_direction * speed * Time.deltaTime);
        }
    }

    public override void Interact()
    {
        if (active) return;

        for (int i = 0; i < signalList.Count; i++)
        {
            signalList[i].GetComponent<AbstractInteractable>().Signal();
        }

        active = true;
        m_direction = m_targets[currentNum].m_position - transform.position;
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum].m_position);
        if (usePhysics)
            m_rigidbody.isKinematic = false;
    }

    public override void Signal()
    {
        active = !active;
    }

    void OnDrawGizmos()
    {
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

        if (m_transforms.Count > 0)
        {
            Gizmos.DrawLine(m_transforms[0].position, m_transforms[m_transforms.Count - 1].position);
            Gizmos.DrawCube(m_transforms[m_transforms.Count - 1].position, new Vector3(1, 1, 1));
        }
    }
}
