using UnityEngine;
using System.Collections;

public enum GameState
{
    InGame,
    Paused
}

public class ControllerUI : MonoBehaviour
{
    //Public vars
    public GameState m_State = GameState.InGame;

    //Pause vars
    bool m_IsPaused = false;

    //Component vars
    ControllerPlayer m_Player;
    ControllerCamera m_Camera;
    ControllerCheckpoint m_Checkpoints;

    //Toggle objects
    GameObject m_CameraInfo;
    GameObject m_PausedInfo;

	void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        m_Camera = Camera.main.GetComponent<ControllerCamera>();
        m_Checkpoints = GameObject.Find("Player").GetComponent<ControllerCheckpoint>();

        //Toggle objects
        m_CameraInfo = GameObject.Find("CameraInfo");
        m_PausedInfo = GameObject.Find("PausedInfo");
        m_PausedInfo.SetActive(false);
	}
	
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePaused();

        if (GetIsPaused())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TogglePaused()
    {
        m_IsPaused = !m_IsPaused;

        if (!GetIsPaused())
            Time.timeScale = 1;
        else
            Time.timeScale = 0;

        m_Player.SetPaused(m_IsPaused);
        m_Camera.SetPaused(m_IsPaused);
        m_Checkpoints.SetPaused(m_IsPaused);

        m_PausedInfo.SetActive(m_IsPaused);
        m_CameraInfo.SetActive(!m_IsPaused);
    }

    public bool GetIsPaused()
    {
        return m_IsPaused;
    }

    void SetState(GameState newState)
    {
        m_State = newState;
    }
}
