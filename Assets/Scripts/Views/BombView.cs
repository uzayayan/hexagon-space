using TMPro;
using UnityEngine;

public class BombView : HexagonView
{
    #region Serializable Fields

    [Header("Texts")]
    [SerializeField] private TMP_Text m_counterText;

    #endregion
    #region Private Fields

    private BombController bombController => (BombController) m_baseController;

    #endregion
    
    /// <summary>
    /// This function called when related controller initialized.
    /// </summary>
    public override void OnControllerInitialized()
    {
        bombController.Exploded += OnExploded;
        GameManager.Instance.GetBoardController().Moved += OnMoved;
        
        base.OnControllerInitialized();
    }

    /// <summary>
    /// This function helper for updates some fields on this component.
    /// </summary>
    protected override void UpdateContent()
    {
        m_image.color = bombController.GetColor();
        m_counterText.text = bombController.GetCounter().ToString("N0");

        if (GameManager.Instance.IsDebugMode())
        {
            m_coordinateText.text = bombController.GetCoordinate().ToString();
        }
    }

    /// <summary>
    /// This function called when player moved.
    /// </summary>
    private void OnMoved()
    {
        UpdateContent();
    }
    
    /// <summary>
    /// This function called when bomb exploded.
    /// </summary>
    private void OnExploded()
    {
        SoundManager.Instance.Play(SoundType.Bomb);
        InterfaceManager.Instance.DrawExplosionParticle();
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        bombController.Exploded -= OnExploded;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetBoardController().Moved -= OnMoved;
        }
        
        base.OnDestroy();
    }
}
