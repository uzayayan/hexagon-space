using System;

[Serializable]
public class Slot : BaseModel
{
    public Coordinate Coordinate;

    public Slot(Coordinate coordinate)
    {
        this.Coordinate = coordinate;
    }
}
