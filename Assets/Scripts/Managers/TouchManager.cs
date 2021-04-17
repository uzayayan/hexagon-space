using System;
using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    #region Public Fields

    public Action<Direction> Swipe;

    #endregion
    #region Private Fields

    private bool isSwiped;
    private Vector2 pressedPosition;

    #endregion

    /// <summary>
    /// This function called per frame.
    /// </summary>
    private void Update()
    {
        SwipeDetection();
    }

    /// <summary>
    /// This function helper for detect swipe.
    /// </summary>
    private void SwipeDetection()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isSwiped = false;
            pressedPosition = Vector2.zero;
        }
        
        if(isSwiped)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            pressedPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(pressedPosition, Input.mousePosition) > CommonTypes.SWIPE_THRESHOLD)
            {
                Vector2 direction = (pressedPosition - (Vector2) Input.mousePosition).normalized;

                if (direction.y > 0)
                {
                    Debug.Log("Swipe Down.");
                    Swipe?.Invoke(Direction.Down);
                }
                else
                {
                    Debug.Log("Swipe Up.");
                    Swipe?.Invoke(Direction.Up);
                }

                isSwiped = true;
            }
        }
    }
}
