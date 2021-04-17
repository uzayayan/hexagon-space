public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    #region Operators
    
    public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.x + b.x, a.y + b.y);
    
    public static bool operator ==(Coordinate coordinate01, Coordinate coordinate02) => coordinate01.Equals(coordinate02);
    
    public static bool operator !=(Coordinate coordinate01, Coordinate coordinate02) => !(coordinate01 == coordinate02);
    
    public bool Equals(Coordinate other) => x == other.x && y == other.y;

    public override bool Equals(object obj) => obj is Coordinate other && Equals(other);
    
    public override int GetHashCode() => (x * 397) ^ y;

    public override string ToString() => $"({x},{y})";

    #endregion
}
