using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Public Fields
    
    public static bool isGameOver;

    public Action Started;
    public Action Over;

    #endregion
    #region Serializable Fields

    [Header("General")]
    [SerializeField] private bool m_isDebugMode;
    [SerializeField] private GameSettings m_gameSettings;
    
    [Header("References")]
    [SerializeField] private PlayerController m_playerController;
    [SerializeField] private BoardController m_boardController;

    #endregion

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        Debug.Log("Game is Starting...");

        Application.targetFrameRate = 60;

        isGameOver = false;
        
        Board board = DataService.LoadObjectWithKey<Board>(CommonTypes.BOARD_DATA_KEY);
        Player player = DataService.LoadObjectWithKey<Player>(CommonTypes.PLAYER_DATA_KEY);

        m_playerController.Initialize(player);
        m_boardController.Initialize(board);
        
        Started?.Invoke();
        
        Debug.Log("Game is Successfully Started!");
    }

    /// <summary>
    /// This function helper for over the game.
    /// </summary>
    public void GameOver(float panelDelay)
    {
        PlayerPrefs.SetString(CommonTypes.BOARD_DATA_KEY, "");
        PlayerPrefs.SetString(CommonTypes.PLAYER_DATA_KEY, "");

        isGameOver = true;
        InterfaceManager.Instance.OpenGameOverPanel(panelDelay);
        
        Over?.Invoke();
        
        Debug.Log("Game Over!");
    }
    
    /// <summary>
    /// This function helper for restart game.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
        Debug.Log("Game is Restarted.");
    }

    /// <summary>
    /// This function returns player score.
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return GetPlayerController().GetScore();
    }

    /// <summary>
    /// This function returns related Player Controller.
    /// </summary>
    /// <returns></returns>
    public PlayerController GetPlayerController()
    {
        return m_playerController;
    }
    
    /// <summary>
    /// This function returns related Board Controller.
    /// </summary>
    /// <returns></returns>
    public BoardController GetBoardController()
    {
        return m_boardController;
    }

    /// <summary>
    /// This function returns related Game Settings.
    /// </summary>
    /// <returns></returns>
    public GameSettings GetGameSettings()
    {
        return m_gameSettings;
    }
    
    /// <summary>
    /// This function returns true if game is debug mode.
    /// </summary>
    /// <returns></returns>
    public bool IsDebugMode()
    {
        return m_isDebugMode;
    }

    /// <summary>
    /// This function return true if game is over.
    /// </summary>
    /// <returns></returns>
    public static bool IsGameOver()
    {
        return isGameOver;
    }
}
