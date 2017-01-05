using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState
{
    InGame,
    Paused
}

public enum MenuState
{
    Main,
    Options,
    Instructions
}

public class ControllerUI : MonoBehaviour
{
    //Public vars
    public GameState m_GameState = GameState.InGame;
    public MenuState m_MenuState = MenuState.Main;

    //Pause vars
    bool m_IsPaused = false;

    //Component vars
    ControllerPlayer m_Player;
    ControllerCamera m_Camera;
    ControllerCheckpoint m_Checkpoints;
    //Bow m_PlayerBow;

    //Toggle objects
    GameObject m_CameraInfo;
    GameObject m_PausedInfo;

	void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<ControllerPlayer>();
        m_Camera = Camera.main.GetComponent<ControllerCamera>();
        m_Checkpoints = GameObject.Find("Player").GetComponent<ControllerCheckpoint>();
        //m_PlayerBow = m_Player.transform.FindChild("Bow").GetComponent<Bow>();

        //Toggle objects
        m_CameraInfo = GameObject.Find("CameraInfo");
        m_PausedInfo = GameObject.Find("PausedInfo");
        m_PausedInfo.SetActive(false);

        Time.timeScale = 1;
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
        {
            Time.timeScale = 1;
            SetGameState(GameState.InGame);
        }
        else
        {
            Time.timeScale = 0;
            SetGameState(GameState.Paused);
        }

        m_Player.SetPaused(m_IsPaused);
        //m_PlayerBow.SetPaused(m_IsPaused);
        m_Camera.SetPaused(m_IsPaused);
        m_Checkpoints.SetPaused(m_IsPaused);

        m_PausedInfo.SetActive(m_IsPaused);
        m_CameraInfo.SetActive(!m_IsPaused);
    }

    public bool GetIsPaused()
    {
        return m_IsPaused;
    }

    void SetGameState(GameState newState)
    {
        m_GameState = newState;
    }

    public GameState GetGameState()
    {
        return m_GameState;
    }

    void SetMenuState(MenuState newState)
    {
        m_MenuState = newState;
    }

    public MenuState GetMenuState()
    {
        return m_MenuState;
    }

    void LoadScene(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(index);
        else
            Debug.Log("ERROR, loadscene index is either negative or out of range: " + index);
    }

    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
