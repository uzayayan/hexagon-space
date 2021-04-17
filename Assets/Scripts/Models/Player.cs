using System;
using UnityEngine;

[Serializable]
public class Player : BaseModel
{
    public int Move;
    public int Score;
    
    /// <summary>
    /// This function helper for increase score.
    /// </summary>
    public void IncreaseScore(int additiveScore)
    {
        Score += additiveScore;
    }
    
    /// <summary>
    /// This function helper for increase move.
    /// </summary>
    public void IncreaseMove()
    {
        Move++;
    }

    /// <summary>
    /// This function helper for save player data.
    /// </summary>
    public void SaveData()
    {
        PlayerPrefs.SetString(CommonTypes.PLAYER_DATA_KEY, DataService.ObjectToJson(this));
    }
}
