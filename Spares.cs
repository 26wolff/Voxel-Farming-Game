using Microsoft.Xna.Framework;

namespace Program
{
    public struct Vector3Int
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // You can also add methods for common operations (e.g., addition, subtraction)
        public static Vector3Int operator +(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3Int operator -(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3Int operator +(Vector3Int a, int b)
        {
            return new Vector3Int(a.X + b, a.Y + b, a.Z + b);
        }
        public static Vector3Int operator -(Vector3Int a, int b)
        {
            return new Vector3Int(a.X - b, a.Y - b, a.Z - b);
        }
        public static Vector3Int operator -(int a, Vector3Int b)
        {
            return new Vector3Int(a - b.X, a - b.Y, a - b.Z);
        }

        public override string ToString()
        {
            return $"{{X:{X} Y:{Y} Z:{Z}}}";
        }
    }
    public static class Spare
    {
        public static bool Bound(int v, int min = 0, int max = 0)
        {
            return !(v < min || v > max);
        }
    }

}