using TMPro;
using System;
using DG.Tweening;
using UnityEngine;

public class PlayerView : BaseView
{
    #region Serializable Fields

    [Header("Transforms")]
    [SerializeField] private Transform m_anchorPoint;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text m_scoreText;
    [SerializeField] private TMP_Text m_moveText;

    #endregion
    #region Private Fields

    private PlayerController playerController => (PlayerController) m_baseController;

    #endregion

    /// <summary>
    /// This function called when related controller initialized.
    /// </summary>
    public override void OnControllerInitialized()
    {
        GameManager.Instance.GetBoardController().Moved += OnMoved;
        GameManager.Instance.GetBoardController().Matched += OnAnyHexagonsMatched;

        UpdateContent();
        
        base.OnControllerInitialized();
    }

    /// <summary>
    /// This function helper for updates some fields on this component.
    /// </summary>
    private void UpdateContent()
    {
        Player player = playerController.GetPlayer();

        m_moveText.text = player.Move.ToString();
        m_scoreText.text = player.Score.ToString();
    }

    /// <summary>
    /// This function helper for rotate selected hexagons around related anchor.
    /// </summary>
    public void RotateAnchorPoint(Direction direction, Action<Direction> RotateCompleted)
    {
        int directionMultiplier = direction == Direction.Up ? 1 : -1;
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(m_anchorPoint.DORotate(new Vector3(0, 0, 120 * directionMultiplier), 0.25F, RotateMode.LocalAxisAdd).OnComplete(()=> RotateCompleted?.Invoke(direction)));
        sequence.Append(m_anchorPoint.DORotate(new Vector3(0, 0, 120 * directionMultiplier), 0.25F, RotateMode.LocalAxisAdd).OnComplete(()=> RotateCompleted?.Invoke(direction)));
        sequence.Append(m_anchorPoint.DORotate(new Vector3(0, 0, 120 * directionMultiplier), 0.25F, RotateMode.LocalAxisAdd).OnComplete(()=> RotateCompleted?.Invoke(direction)));

        sequence.SetId(CommonTypes.ANCHOR_POINT_TWEEN_KEY);
        sequence.Play();
    }
    
    /// <summary>
    /// This function called when any hexagons matched.
    /// </summary>
    private void OnAnyHexagonsMatched(int score)
    {
        SetAnchorPointActiveState(false);
        UpdateContent();
    }
    
    /// <summary>
    /// This function called when player moved.
    /// </summary>
    private void OnMoved()
    {
        UpdateContent();
    }
    
    /// <summary>
    /// This function helper for set position of Anchor Point.
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetAnchorPointPosition(Vector2 targetPosition)
    {
        SetAnchorPointActiveState(true);
        m_anchorPoint.localPosition = targetPosition;
    }

    /// <summary>
    /// This function helper for set visible state of Anchor Point.
    /// </summary>
    /// <param name="state"></param>
    public void SetAnchorPointActiveState(bool state)
    {
        m_anchorPoint.gameObject.SetActive(state);
    }

    /// <summary>
    /// This function returns related Anchor Point.
    /// </summary>
    /// <returns></returns>
    public Transform GetAnchorPoint()
    {
        return m_anchorPoint;
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetBoardController().Moved -= OnMoved;
            GameManager.Instance.GetBoardController().Matched -= OnAnyHexagonsMatched;
        }
        
        base.OnDestroy();
    }
}
