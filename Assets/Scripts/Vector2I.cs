// Vector2I.cs

using System;

[Serializable]
public struct Vector2I
{
    //
    // public vars
    public int x;
    public int y;

    // 
    // constructors
    public Vector2I(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    //
    // ops
    public static Vector2I operator +(Vector2I left, Vector2I right)
    {
        return new Vector2I(left.x + right.x, left.y + right.y);
    }
    public static Vector2I operator -(Vector2I left, Vector2I right)
    {
        return new Vector2I(left.x - right.x, left.y - right.y);
    }

    public static bool operator ==(Vector2I left, Vector2I right)
    {
        return (left.x == right.x && left.y == right.y);
    }
    public static bool operator !=(Vector2I left, Vector2I right)
    {
        return !(left == right);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public int rectArea()
    {
        return x * y;
    }

    //
    // util methods
    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
