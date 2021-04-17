using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HexagonView : BaseView
{
    #region Serializable Fields

    [Header("Images")]
    [SerializeField] protected Image m_image;
    [SerializeField] protected Image m_outline;
    [SerializeField] private Image m_star;
    
    [Header("Texts")]
    [SerializeField] protected TMP_Text m_coordinateText;
    
    [Header("Misc")]
    [SerializeField] private Canvas m_canvas;

    #endregion
    #region Private Fields

    private HexagonController hexagonController => (HexagonController) m_baseController;

    #endregion
    
    /// <summary>
    /// This function called when related controller initialized.
    /// </summary>
    public override void OnControllerInitialized()
    {
        hexagonController.SlotChanged += OnSlotChanged;
        hexagonController.SelectedStateChanged += OnSelectedStateChanged;
        
        UpdateContent();
        
        base.OnControllerInitialized();
    }

    /// <summary>
    /// This function called when related Hexagon's coordinate changed.
    /// </summary>
    /// <param name="slotController"></param>
    private void OnSlotChanged(SlotController slotController)
    {
        if (GameManager.Instance.IsDebugMode())
        {
            m_coordinateText.text = hexagonController.GetCoordinate().ToString();
        }
        
        if(!hexagonController.GetSelectedState())
            transform.SetParent(hexagonController.GetSlotController().transform, true);
    }

    /// <summary>
    /// This function called when selected state changed.
    /// </summary>
    private void OnSelectedStateChanged(bool state)
    {
        ChangeFocusState(state);

        if (state)
        {
            transform.SetParent(GameManager.Instance.GetPlayerController().GetPlayerView().GetAnchorPoint());
        }
        else
        {
            transform.SetParent(hexagonController.GetSlotController().transform);
            DrawDefaultLayer();
        }
    }
    
    /// <summary>
    /// This function helper for updates some fields on this component.
    /// </summary>
    protected virtual void UpdateContent()
    {
        m_image.color = hexagonController.GetColor();
        m_star.gameObject.SetActive(hexagonController.IsBonus());

        if (GameManager.Instance.IsDebugMode())
        {
            m_coordinateText.text = hexagonController.GetCoordinate().ToString();
        }
    }

    /// <summary>
    /// This function helper for this component move to default layer.
    /// </summary>
    /// <param name="withAnimation"></param>
    /// <param name="delay"></param>
    public void DrawDefaultLayer(bool withAnimation = false, float delay = 0)
    {
        if (withAnimation)
        {
            transform.DOLocalMove(Vector3.zero, GameManager.Instance.GetGameSettings().Gravity).SetId(CommonTypes.HEXAGON_GRAVITY_TWEEN_KEY).SetEase(Ease.Linear).SetDelay(delay);
        }
        else
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// This function helper for change state of related outline.
    /// </summary>
    /// <param name="state"></param>
    private void ChangeFocusState(bool state)
    {
        m_canvas.overrideSorting = state;
        m_canvas.sortingOrder = state ? 10 : 0;
        m_outline.gameObject.SetActive(state);
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>    
    public override void OnDestroy()
    {
        hexagonController.SlotChanged -= OnSlotChanged;
        hexagonController.SelectedStateChanged -= OnSelectedStateChanged;

        base.OnDestroy();
    }
}
