using System;
using UnityEngine;

[Serializable]
public class Bomb : Hexagon
{
    public int Counter;
    
    public Bomb(int colorId, Color color, bool isBonus, int counter) : base(colorId, color, isBonus)
    {
        this.Counter = counter;
    }

    /// <summary>
    /// This function helper for decrease counter.
    /// </summary>
    public void DecreaseCounter()
    {
        Counter--;
    }
}
