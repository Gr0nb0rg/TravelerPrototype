using UnityEngine;
using System.Collections;

public class ControllerCheckpoint : MonoBehaviour
{
    //Public vars
    public int m_CurrentCheckpoint;
    public Transform[] m_Checkpoints;

    //Component vars
    ControllerPlayer m_Player;

    //Paused vars
    bool m_IsPaused = false;

	void Start()
    {
        m_Player = GetComponent<ControllerPlayer>();

        //Find checkpoints
        var checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        if (checkpoints.Length > 0)
        {
            m_Checkpoints = new Transform[checkpoints.Length];
            for (int i = 0; i < checkpoints.Length; i++)
            {
                m_Checkpoints[i] = checkpoints[i].transform;
            }

            //Sort checkpoints
            if (checkpoints.Length > 1)
            {
                for (int write = 0; write < checkpoints.Length; write++)
                {
                    for (int sort = 0; sort < checkpoints.Length - 1; sort++)
                    {
                        if (m_Checkpoints[sort].GetComponent<Checkpoint>().m_ID > m_Checkpoints[sort + 1].GetComponent<Checkpoint>().m_ID)
                        {
                            Transform temp = m_Checkpoints[sort + 1];
                            m_Checkpoints[sort + 1] = m_Checkpoints[sort];
                            m_Checkpoints[sort] = temp;

                        }
                    }
                }
            }
        }
        else
            Debug.Log("Couldn't find any checkpoints!");
	}
	
	void Update()
    {
        if (!m_IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                m_Player.SetPosition(m_Checkpoints[m_CurrentCheckpoint].position);
                m_Player.ResetValues();
            }
        }
    }

    public void SetCurrent(int id)
    {
        m_CurrentCheckpoint = id;
        Debug.Log("Current checkpoint set to: " + m_CurrentCheckpoint);
    }

    public Transform GetCurrentCheckpoint()
    {
        return m_Checkpoints[m_CurrentCheckpoint];
    }

    public int GetCurrent()
    {
        return m_CurrentCheckpoint;
    }

    public void SetPaused(bool state)
    {
        m_IsPaused = state;
    }

    public bool GetIsPaused()
    {
        return m_IsPaused;
    }
}
