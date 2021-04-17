using System;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] protected BaseController m_baseController;

    #endregion

    /// <summary>
    /// Awake
    /// </summary>
    protected virtual void Awake()
    {
        m_baseController.Initialized += OnControllerInitialized;
    }

    /// <summary>
    /// Start
    /// </summary>
    protected virtual void Start() { }

    /// <summary>
    /// This function called when related controller initialized.
    /// </summary>
    public virtual void OnControllerInitialized(){}

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public virtual void OnDestroy()
    {
        m_baseController.Initialized -= OnControllerInitialized;
    }
}
