using UnityEngine;

/// <summary>
/// Common Types
/// </summary>
public static class CommonTypes
{
    //Hexagon
    public static int HEXAGON_WIDTH = 256;
    public static int HEXAGON_HEIGHT = 221;
    
    //Touch
    public static float SWIPE_THRESHOLD = 200;
    public static float ANCHOR_ROTATE_THRESHOLD = 750;

    //Keys
    public static string SOUND_STATE_KEY = "sound_state_data";
    public static string PLAYER_DATA_KEY = "player_data";
    public static string BOARD_DATA_KEY = "board_data";
    public static string HEXAGON_GRAVITY_TWEEN_KEY = "hexagon_gravity_tween";
    public static string ANCHOR_POINT_TWEEN_KEY = "anchor_point_tween";
}

/// <summary>
/// Game Utils
/// </summary>
public static class GameUtils
{
    /// <summary>
    /// This function return additive coordinates by direction.
    /// </summary>
    /// <param name="invert"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Coordinate[] GetNeighboursIndexByDirection(bool invert, Direction direction)
    {
        switch (direction)
        {
            case Direction.UpRight:
                return UpRightNeighbours(invert);
            case Direction.Right:
                return RightNeighbours(invert);
            case Direction.DownRight:
                return DownRightNeighbours(invert);
            case Direction.Down:
                return DownRightNeighbours(invert);
            case Direction.DownLeft:
                return DownLeftNeighbours(invert);
            case Direction.Left:
                return LeftNeighbours(invert);
            case Direction.UpLeft:
                return UpLeftNeighbours(invert);
            case Direction.Up:
                return UpLeftNeighbours(invert);
        }

        return null;
    }
    
    /// <summary>
    /// This function return additive coordinates for near neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] NearNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(0, -1),
                new Coordinate(1, -1),
                new Coordinate(1, 0),
                new Coordinate(0, 1),
                new Coordinate(-1, 0),
                new Coordinate(-1, -1)
            }; 
        }
        
        return new []  
        {
            new Coordinate(0, -1),
            new Coordinate(1, 0),
            new Coordinate(1, 1),
            new Coordinate(0, 1),
            new Coordinate(-1, 1),
            new Coordinate(-1, 0)
        }; 
    }
    
    /// <summary>
    /// This function return additive coordinates for up right neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] UpRightNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(0, -1),
                new Coordinate(1, -1)
            };
        }

        return new []  
        {
            new Coordinate(0, -1),
            new Coordinate(1, 0)
        }; 
    }
    
    /// <summary>
    /// This function return additive coordinates for right neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] RightNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(1, -1),
                new Coordinate(1, 0)
            };
        }

        return new []  
        {
            new Coordinate(1, 0),
            new Coordinate(1, 1)
        };
    }
    
    /// <summary>
    /// This function return additive coordinates for down right neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] DownRightNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(1, 0),
                new Coordinate(0, 1)
            };
        }

        return new []  
        {
            new Coordinate(1, 1),
            new Coordinate(0, 1)
        }; 
    }
    
    /// <summary>
    /// This function return additive coordinates for down left neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] DownLeftNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(0, 1),
                new Coordinate(-1, 0)
            };
        }

        return new []  
        {
            new Coordinate(0, 1),
            new Coordinate(-1, 1)
        }; 
    }
    
    /// <summary>
    /// This function return additive coordinates for left neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] LeftNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(-1, 0),
                new Coordinate(-1, -1)
            };
        }

        return new []  
        {
            new Coordinate(-1, 1),
            new Coordinate(-1, 0)
        };
    }
    
    /// <summary>
    /// This function return additive coordinates for up left neighbours.
    /// </summary>
    /// <param name="invert"></param>
    /// <returns></returns>
    public static Coordinate[] UpLeftNeighbours(bool invert)
    {
        if (!invert)
        {
            return new []  
            {
                new Coordinate(-1, -1),
                new Coordinate(0, -1)
            };
        }
        
        return new []  
        {
            new Coordinate(-1, 0),
            new Coordinate(0, -1)
        }; 
    }
}

/// <summary>
/// Math Utils
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// This function return angle between two vector.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float CalculateAngle(Vector2 from, Vector2 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    /// <summary>
    /// This function return center point of many vectors.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    public static Vector2 CalculateCenterPoint(Vector2 pos, Vector2[] positions)
    {
        Vector2 totalPosition = pos;

        foreach (Vector2 position in positions)
        {
            totalPosition += position;
        }

        return totalPosition / (positions.Length + 1);
    }
}

#region Enums

public enum Direction
{
    UpRight = 0,
    Right = 1,
    DownRight = 2,
    Down = 3,
    DownLeft = 4,
    Left = 5,
    UpLeft = 6,
    Up = 7
}

public enum SoundType
{
    Match,
    Rotate,
    Click,
    Bomb
}

#endregion

