using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class HexagonController : BaseController
{
    #region Public Fields
    
    public Action<bool> SelectedStateChanged;
    public Action<SlotController> SlotChanged;

    #endregion
    #region SerializableFields

    [SerializeField] private bool m_isSelected;

    #endregion
    #region Private Fields

    private Hexagon hexagon;
    private HexagonView hexagonView => (HexagonView) m_baseView;
    private SlotController slotController;
    
    #endregion
    
    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    /// <param name="parameters"></param>
    public override void Initialize(params object[] parameters)
    {
        hexagon = (Hexagon) parameters[0];
        SetSlotController((SlotController) parameters[1]);

        base.Initialize(parameters);
    }
    
    /// <summary>
    /// This function called when pointer clicked on this component.
    /// </summary>
    public void OnMouseUpAsButton()
    {
        if(GameManager.Instance.GetBoardController().IsLocked())
            return;
        
        Direction direction = GetDirectionByTouchPosition(Input.mousePosition);
        List<HexagonController> neighboursHexagonControllers = GetNeighboursHexagonControllers(direction);
        
        if(neighboursHexagonControllers.Count < 2)
            return;
        
        GameManager.Instance.GetPlayerController().GetPlayerView().SetAnchorPointPosition(MathUtils.CalculateCenterPoint(GetSlotPosition(), neighboursHexagonControllers.Select(x=> x.GetSlotPosition()).ToArray()));
        GameManager.Instance.GetPlayerController().SetSelectedHexagons(this, neighboursHexagonControllers);
        SoundManager.Instance.Play(SoundType.Click);
        
        Debug.Log($"Player Clicked to Hexagon. Touch Direction : {direction} Color ID : {GetColorId()} Coordinate : {GetCoordinate()}");
    }

    /// <summary>
    /// This function returns neighbours Hexagon Controllers by direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public List<HexagonController> GetNeighboursHexagonControllers(Direction direction)
    {
        int failedCount = 0;
        List<HexagonController> neighboursHexagonControllers = new List<HexagonController>();

        while (true)
        {
            bool isFailed = false;
            Coordinate[] neighboursAdditivePosition = GameUtils.GetNeighboursIndexByDirection(GetCoordinate().x % 2 != 0, (Direction) (((int) direction + failedCount) % 8));

            neighboursHexagonControllers.Clear();

            foreach (Coordinate neighbourAdditivePosition in neighboursAdditivePosition)
            {
                Coordinate targetCoordinate = GetCoordinate() + neighbourAdditivePosition;

                HexagonController neighbourHexagonController = GameManager.Instance.GetBoardController().GetHexagonControllerByCoordinate(targetCoordinate, true);

                if (neighbourHexagonController == null)
                {
                    isFailed = true;
                    failedCount++;
                    break;
                }
                
                neighboursHexagonControllers.Add(neighbourHexagonController);
            }
            
            if(failedCount >= 8)
                break;
            
            if(!isFailed)
                break;
        }

        failedCount = 0;
        return neighboursHexagonControllers;
    }

    /// <summary>
    /// This function returns direction by touch position.
    /// </summary>
    /// <param name="touchPosition"></param>
    /// <returns></returns>
    public Direction GetDirectionByTouchPosition(Vector2 touchPosition)
    {
        float angle = 0;
        Vector2 direction = Vector2.zero;
        Vector2 inputPosition = Vector2.zero;
        Canvas canvas = InterfaceManager.Instance.GetCanvas();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touchPosition, canvas.worldCamera, out inputPosition);
        
        direction = (GetSlotPosition() - inputPosition).normalized;
        angle = 360 - MathUtils.CalculateAngle(direction, transform.forward);
        
        return  (Direction) (int) ((angle - 22.5F) / 45);
    }

    /// <summary>
    /// This function helper for set Slot Controller.
    /// </summary>
    /// <returns></returns>
    public void SetSlotController(SlotController slotController)
    {
        this.slotController = slotController;
        hexagon.SetSlot(slotController.GetSlot());
        slotController.SetHexagonController(this);
        
        SlotChanged?.Invoke(slotController);
    }
    
    /// <summary>
    /// This function helper for set selected state of this component.
    /// </summary>
    /// <param name="value"></param>
    public void SetSelectedState(bool value)
    {
        m_isSelected = value;
        
        SelectedStateChanged?.Invoke(m_isSelected);
    }

    /// <summary>
    /// This function returns related Hexagon Component.
    /// </summary>
    /// <returns></returns>
    public Hexagon GetHexagon()
    {
        return hexagon;
    }

    /// <summary>
    /// This function returns related Slot Controller.
    /// </summary>
    /// <returns></returns>
    public SlotController GetSlotController()
    {
        return slotController;
    }
    
    /// <summary>
    /// This function return coordinate.
    /// </summary>
    /// <returns></returns>
    public Coordinate GetCoordinate()
    {
        return hexagon.GetCoordinate();
    }
    
    /// <summary>
    /// This function return color id of Hexagon.
    /// </summary>
    /// <returns></returns>
    public int GetColorId()
    {
        return hexagon.ColorId;
    }
    
    /// <summary>
    /// This function return color of Hexagon.
    /// </summary>
    /// <returns></returns>
    public Color GetColor()
    {
        return hexagon.Color;
    }

    /// <summary>
    /// This function return position of Hexagon.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetSlotPosition()
    {
        return slotController.transform.localPosition;
    }

    /// <summary>
    /// This function returns selected state of this component.
    /// </summary>
    /// <returns></returns>
    public bool GetSelectedState()
    {
        return m_isSelected;
    }

    /// <summary>
    /// This function return true if this hexagon can be take desired color id from near neighbours.
    /// </summary>
    /// <param name="targetColorId"></param>
    /// <param name="extractHexagonCoordinates"></param>
    /// <returns></returns>
    public bool CanBeColor(int targetColorId, Coordinate[] extractHexagonCoordinates)
    {
        Coordinate[] nearNeighboursAdditiveCoordinates = GameUtils.NearNeighbours(GetCoordinate().x % 2 != 0);
        BoardController boardController = GameManager.Instance.GetBoardController();
        List<HexagonController> nearNeighboursHexagonControllers = new List<HexagonController>();

        foreach (Coordinate additiveCoordinate in nearNeighboursAdditiveCoordinates)
        {
            HexagonController neighboursHexagonController = boardController.GetHexagonControllerByCoordinate(GetCoordinate() + additiveCoordinate, true);
            
            if(neighboursHexagonController == null)
                continue;
            
            if(extractHexagonCoordinates.Contains(neighboursHexagonController.GetCoordinate()))
                continue;

            nearNeighboursHexagonControllers.Add(neighboursHexagonController);
        }

        return nearNeighboursHexagonControllers.Any(x=> x.GetColorId() == targetColorId);
    }
    
    /// <summary>
    /// This function return true if this hexagon is bonus.
    /// </summary>
    /// <returns></returns>
    public bool IsBonus()
    {
        return hexagon.IsBonus;
    }
}
