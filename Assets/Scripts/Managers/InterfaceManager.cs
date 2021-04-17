using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager>
{
    #region Serializable Fields

    [Header("Transforms")]
    [SerializeField] private Transform m_playArea;

    [Header("Panels")] 
    [SerializeField] private UIGameOverPanel m_gameOverpanel;
    
    [Header("Misc")]
    [SerializeField] private UILayoutFitter m_layoutFitter;
    [SerializeField] private Camera m_camera;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private Toggle m_musicToggle;

    #endregion

    /// <summary>
    /// Start
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        GameManager.Instance.Started += OnGameStarted;
        
        m_musicToggle.isOn = PlayerPrefs.GetInt(CommonTypes.SOUND_STATE_KEY) == 0;
    }

    /// <summary>
    /// This function called when game is started.
    /// </summary>
    private void OnGameStarted()
    {
        m_layoutFitter.Initialize();
    }

    /// <summary>
    /// This function helper for open game over panel.
    /// </summary>
    public void OpenGameOverPanel(float delay)
    {
        _= m_gameOverpanel.Initialize(delay);
    }

    /// <summary>
    /// This function helper for draw debris particle on the screen.
    /// </summary>
    public void DrawDebrisParticle(Color color, Vector2 position)
    {
        ParticleSystem createdDebris = Instantiate(GameManager.Instance.GetGameSettings().DebrisPrefab, GetCanvas().transform);
        createdDebris.transform.localPosition = position;
        createdDebris.startColor = color;
        
        createdDebris.Play();
    }
    
    /// <summary>
    /// This function helper for draw explosion particle on the screen.
    /// </summary>
    public void DrawExplosionParticle()
    {
        ParticleSystem createdExplosion = Instantiate(GameManager.Instance.GetGameSettings().ExplosionPrefab, GetCanvas().transform);
        createdExplosion.transform.localPosition = Vector3.zero;
        
        createdExplosion.Play();
    }
    
    /// <summary>
    /// This function helper for draw floating text on the screen.
    /// </summary>
    public void DrawFloatingText(string text, Vector2 position)
    {
        if(GameManager.IsGameOver())
            return;
        
        TMP_Text createdFloatingText = Instantiate(GameManager.Instance.GetGameSettings().FloatingTextPrefab, GetCanvas().transform);

        createdFloatingText.text = text;
        createdFloatingText.transform.localPosition = position;

        Sequence sequence = DOTween.Sequence();

        sequence.Join(createdFloatingText.transform.DOScale(Vector3.one, 0.5F));
        sequence.Join(createdFloatingText.transform.DOLocalMoveY(createdFloatingText.transform.localPosition.y + 250, 1));
        sequence.Join(createdFloatingText.DOFade(0,0.5F).SetDelay(0.5F));

        sequence.OnComplete(() =>
        {
            Destroy(createdFloatingText.gameObject);
        });
        
        sequence.Play();
    }

    /// <summary>
    /// This function returns out screen of Y axis.
    /// </summary>
    /// <param name="height"></param>
    public float GetOutScreenYAxis(float height)
    {
        return (m_canvas.GetComponent<RectTransform>().sizeDelta.y / 2) + (height / 2);
    }

    /// <summary>
    /// This function returns play area.
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayArea()
    {
        return m_playArea;
    }
    
    /// <summary>
    /// This function returns related camera.
    /// </summary>
    /// <returns></returns>
    public Canvas GetCamera()
    {
        return m_canvas;
    }
    
    /// <summary>
    /// This function returns related canvas.
    /// </summary>
    /// <returns></returns>
    public Canvas GetCanvas()
    {
        return m_canvas;
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    protected override void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Started -= OnGameStarted;
        }
        
        base.OnDestroy();
    }
}
