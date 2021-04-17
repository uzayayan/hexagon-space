using UnityEngine;
using System.Collections.Generic;

public class UILayoutFitter : MonoBehaviour
{
    #region Serializable Fields

    [Header("Transforms")]
    [SerializeField] private Transform m_target;

    #endregion
    
    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    public void Initialize()
    {
        AdjustLayout();
    }

    /// <summary>
    /// This function helper for adjust layout size and position.
    /// </summary>
    private void AdjustLayout() 
    {
        RectTransform children = m_target.GetComponentInChildren<RectTransform>();

        Vector3 centerPosition = Vector2.zero;
        float min_x, max_x, min_y, max_y;
        min_x = max_x = transform.localPosition.x;
        min_y = max_y = transform.localPosition.y;

        foreach (RectTransform child in children) 
        {
            Vector2 scale = child.sizeDelta;
            float temp_min_x, temp_max_x, temp_min_y, temp_max_y;

            temp_min_x = child.localPosition.x - (scale.x / 2);
            temp_max_x = child.localPosition.x + (scale.x / 2);
            temp_min_y = child.localPosition.y - (scale.y / 2);
            temp_max_y = child.localPosition.y + (scale.y / 2);

            centerPosition += child.localPosition;

            if (temp_min_x < min_x)
                min_x = temp_min_x;
            if (temp_max_x > max_x)
                max_x = temp_max_x;

            if (temp_min_y < min_y)
                min_y = temp_min_y;
            if (temp_max_y > max_y)
                max_y = temp_max_y;
        }

        centerPosition /= m_target.childCount;

        GetComponent<RectTransform>().sizeDelta = new Vector2(max_x - min_x, max_y - min_y);
        transform.localPosition = centerPosition;

        List<Transform> childs = new List<Transform>();

        foreach (RectTransform child in children) 
        {
            childs.Add(child);
        }

        foreach (Transform child in childs)
        {
            child.SetParent(transform, true);
        }

        transform.localPosition = Vector3.zero;
    }
}
