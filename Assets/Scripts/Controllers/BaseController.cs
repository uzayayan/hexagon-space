using System;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    #region Public Fields

    public Action Initialized;

    #endregion
    #region Serializable Fields

    [SerializeField] protected BaseView m_baseView;

    #endregion

    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    public virtual void Initialize(params object[] parameters)
    {
        Initialized?.Invoke();
    }

    /// <summary>
    /// This function returns related View Component.
    /// </summary>
    /// <returns></returns>
    public virtual BaseView GetView()
    {
        return m_baseView;
    }
    
    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public virtual void OnDestroy() { }
}
