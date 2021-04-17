using System;

public class BombController : HexagonController
{
    #region Public Fields

    public Action Exploded;

    #endregion
    #region Private Fields
    
    private Bomb bomb;
    private BombView bombView => (BombView) m_baseView;

    #endregion
    
    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    /// <param name="parameters"></param>
    public override void Initialize(params object[] parameters)
    {
        bomb = (Bomb) parameters[0];

        GameManager.Instance.GetBoardController().Moved += OnMoved;

        base.Initialize(parameters);
    }

    /// <summary>
    /// This function called when player moved.
    /// </summary>
    private void OnMoved()
    {
        bomb.DecreaseCounter();

        if (bomb.Counter <= 0)
        {
            Exploded?.Invoke();
            GameManager.Instance.GameOver(1);
        }
    }

    /// <summary>
    /// This function returns left count.
    /// </summary>
    /// <returns></returns>
    public int GetCounter()
    {
        return bomb.Counter;
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetBoardController().Moved -= OnMoved;
        }
        
        base.OnDestroy();
    }
}
