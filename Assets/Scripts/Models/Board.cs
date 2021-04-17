using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Board
{
    public int NextBombPoint;
    public List<Hexagon> Hexagons;

    /// <summary>
    /// This function helper for save player data.
    /// </summary>
    public void SaveData()
    {
        PlayerPrefs.SetString(CommonTypes.BOARD_DATA_KEY, DataService.ObjectToJson(this));
    }
}
