using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIGameOverPanel : MonoBehaviour
{
    #region Serializable Fields

    [Header("Texts")]
    [SerializeField] private TMP_Text m_scoreText;

    #endregion
    
    /// <summary>
    /// This function helper for initialize this component. 
    /// </summary>
    public async UniTask Initialize(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
        UpdateContent();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// This function helper for updates some fields on this component.
    /// </summary>
    private void UpdateContent()
    {
        m_scoreText.text = GameManager.Instance.GetScore().ToString("N0");
    }

    /// <summary>
    /// This function helper for restart game.
    /// </summary>
    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }
}
