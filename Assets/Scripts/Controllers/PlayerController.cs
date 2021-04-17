using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class PlayerController : BaseController
{
    #region Private Fields

    private Player player;
    private PlayerView playerView => (PlayerView) m_baseView;
    private List<HexagonController> selectedHexagonControllers;
 
    #endregion
    
    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    /// 0 = Player
    public override void Initialize(params object[] parameters)
    {
        player = (Player) parameters[0];
        
        TouchManager.Instance.Swipe += OnSwipe;
        GameManager.Instance.GetBoardController().Moved += OnMoved;
        GameManager.Instance.GetBoardController().Matched += OnAnyHexagonsMatched;
        
        base.Initialize(parameters);
    }

    /// <summary>
    /// This function called when Player swipe on screen.
    /// </summary>
    /// <param name="direction"></param>
    private void OnSwipe(Direction direction)
    {
        if(GameManager.Instance.GetBoardController().IsLocked())
            return;

        if(selectedHexagonControllers == null || selectedHexagonControllers.Count == 0)
            return;
        
        Vector2 inputPosition = Vector2.zero;
        Canvas canvas = InterfaceManager.Instance.GetCanvas();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out inputPosition);

        if(Mathf.Abs(inputPosition.y - playerView.GetAnchorPoint().localPosition.y) > CommonTypes.ANCHOR_ROTATE_THRESHOLD)
            return;
        
        direction = inputPosition.x < playerView.GetAnchorPoint().localPosition.x ? direction == Direction.Up ? Direction.Down : Direction.Up : direction;

        playerView.RotateAnchorPoint(direction, OnAnchorStepCompleted);
        Debug.Log($"Player Swiped : {direction}");
    }

    /// <summary>
    /// This function called when any hexagons matched.
    /// </summary>
    private void OnAnyHexagonsMatched(int score)
    {
        if(GameManager.IsGameOver())
            return;
        
        foreach (HexagonController selectedHexagonController in selectedHexagonControllers)
        {
            selectedHexagonController.SetSelectedState(false);
        }
        
        player.IncreaseScore(score);
        Debug.Log($"Player Score Increased : {player.Score}");
    }
    
    /// <summary>
    /// This function called when player moved.
    /// </summary>
    private void OnMoved()
    {
        player.IncreaseMove();
        Debug.Log($"Player Move Count Increased : {player.Move}");
    }
    
    /// <summary>
    /// This function called when completed Anchor Point step.
    /// </summary>
    /// <param name="direction"></param>
    private void OnAnchorStepCompleted(Direction direction)
    {
        Coordinate[] coordinates = selectedHexagonControllers.Select(x => x.GetCoordinate()).ToArray();
        
        switch (direction)
        {
            case Direction.Down:
                
                for (int i = 0; i < 3; i++)
                {
                    SlotController targetSlotController = GameManager.Instance.GetBoardController().GetSlotControllerByCoordinate(coordinates[Mathf.Abs(i + 1) % 3]);
                    selectedHexagonControllers[i].SetSlotController(targetSlotController);
                }
                
                break;
            case Direction.Up:
                
                for (int i = 3; i > 0; i--)
                {
                    SlotController targetSlotController = GameManager.Instance.GetBoardController().GetSlotControllerByCoordinate(coordinates[Mathf.Abs(i + 1) % 3]);
                    selectedHexagonControllers[i - 1].SetSlotController(targetSlotController);
                }
                
                break;
        }
        
        Debug.Log($"Anchor Step Completed. Direction : {direction}");

        SoundManager.Instance.Play(SoundType.Rotate);
        GameManager.Instance.GetBoardController().CheckHexagons(selectedHexagonControllers, true).ContinueWith(isCompleted =>
        {
            if (isCompleted)
            {
                selectedHexagonControllers.Clear();
                Debug.Log($"Player Selected Hexagons is Completed!");
            }
        });
    }

    /// <summary>
    /// This function helper for set selected hexagons.
    /// </summary>
    public void SetSelectedHexagons(HexagonController hexagonController, List<HexagonController> selectedHexagonControllers)
    {
        //De-select already selected hexagons.
        if (this.selectedHexagonControllers != null)
        {
            foreach (HexagonController selectedHexagonController in this.selectedHexagonControllers)
            {
                selectedHexagonController.SetSelectedState(false);
            }
        }

        selectedHexagonControllers.Insert(0, hexagonController);
        this.selectedHexagonControllers = selectedHexagonControllers;

        foreach (HexagonController selectedHexagonController in this.selectedHexagonControllers)
        {
            selectedHexagonController.SetSelectedState(true);
        }
    }
    
    /// <summary>
    /// This function return related Player component.
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// This function returns player score.
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return player.Score;
    }
    
    /// <summary>
    /// This function return related Player View component.
    /// </summary>
    /// <returns></returns>
    public PlayerView GetPlayerView()
    {
        return playerView;
    }

    /// <summary>
    /// This function helper for save data on this component.
    /// </summary>
    public void SaveData()
    {
        if (!GameManager.IsGameOver())
        {
            player.SaveData();
            
            Debug.Log("Player Data is Saved.");
        }
    }

    /// <summary>
    /// This function called when game pause state changed.
    /// </summary>
    /// <param name="pauseStatus"></param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
            SaveData();
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.Swipe -= OnSwipe;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetBoardController().Moved -= OnMoved;
            GameManager.Instance.GetBoardController().Matched -= OnAnyHexagonsMatched;
        }
        
        SaveData();

        base.OnDestroy();
    }
}
