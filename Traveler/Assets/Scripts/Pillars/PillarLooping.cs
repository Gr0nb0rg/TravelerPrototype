using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLooping : AbstractInteractable
{
    public List<Transform> m_transforms;

    private List<Vector3> m_targets;
    private Transform startPosition;

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

    private Rigidbody m_rigidbody;

    private float m_distance = 0f;

    private Vector3 m_direction;

    private int currentNum = 1;

    void Start () {
        if (alwaysActive)
            active = true;

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

    void Update () {

        #if UNITY_EDITOR
        for (int i = 0; i < m_targets.Count - 1; i++)
        {
            if (m_targets[i] == null || m_targets[i + 1] == null)
            {
                break;
            }


            Debug.DrawLine(m_targets[i], m_targets[i + 1], Color.red);
        }
        #endif

        if (!active) return;

        if (usePhysics)
            m_rigidbody.isKinematic = false;

        if (CheckDistance())
        {
            EditTarget();
        }

        VelocityUpdate();
    }

    private void EditTarget()
    {
        if (m_targets.Count > 1 && currentNum == m_targets.Count - 1)
        {
            if (stopAtStart)
                reachedEnd = true;

            currentNum = 0;
            m_direction = m_targets[currentNum] - transform.position;
            m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        }
        else
        {
            currentNum++;

            if (stopAtStart && !alwaysActive && currentNum == 1 && reachedEnd)
            {
                active = false;
                m_rigidbody.isKinematic = true;
                return;
            }

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

        active = true;
        m_direction = m_targets[currentNum] - transform.position;
        m_distance = Vector3.Distance(transform.position, m_targets[currentNum]);
        if (usePhysics)
            m_rigidbody.isKinematic = false;
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

        Gizmos.DrawLine(m_transforms[0].position, m_transforms[m_transforms.Count - 1].position);
        Gizmos.DrawCube(m_transforms[m_transforms.Count - 1].position, new Vector3(1, 1, 1));

    }
}
