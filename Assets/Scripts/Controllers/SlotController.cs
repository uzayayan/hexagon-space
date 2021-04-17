using UnityEngine;

public class SlotController : BaseController
{
    #region Private Fieelds

    private Slot slot;
    private HexagonController hexagonController;

    #endregion

    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    public override void Initialize(params object[] parameters)
    {
        this.slot = (Slot) parameters[0];
        
        base.Initialize(parameters);
    }

    /// <summary>
    /// This function helper for set Hexagon controller.
    /// </summary>
    public void SetHexagonController(HexagonController hexagonController)
    {
        this.hexagonController = hexagonController;
    }

    /// <summary>
    /// This function returns related Hexagon controller.
    /// </summary>
    public HexagonController GetHexagonController()
    {
        return hexagonController;
    }
    
    /// <summary>
    /// This function returns related Slot.
    /// </summary>
    /// <returns></returns>
    public Slot GetSlot()
    {
        return slot;
    }
    
    /// <summary>
    /// This function return coordinate.
    /// </summary>
    /// <returns></returns>
    public Coordinate GetCoordinate()
    {
        return slot.Coordinate;
    }
}
