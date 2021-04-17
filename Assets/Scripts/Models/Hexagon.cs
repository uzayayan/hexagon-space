using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Hexagon : BaseModel
{
    public int ColorId;
    [JsonIgnore]
    public Color Color;
    public Slot Slot;
    public bool IsBonus;

    public Hexagon(int colorId, Color color, bool isBonus)
    {
        this.ColorId = colorId;
        this.Color = color;
        this.IsBonus = isBonus;
    }

    /// <summary>
    /// This function helper for set Slott.
    /// </summary>
    /// <returns></returns>
    public void SetSlot(Slot slot)
    {
        this.Slot = slot;
    }
    
    /// <summary>
    /// This function return Coordinate.
    /// </summary>
    /// <returns></returns>
    public Coordinate GetCoordinate()
    {
        return Slot.Coordinate;
    }
}
