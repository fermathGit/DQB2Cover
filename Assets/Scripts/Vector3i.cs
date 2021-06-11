using UnityEngine;
using System;


namespace Soultia.Util
{
    [Serializable]
    public struct Vector3i
    {
        public static readonly Vector3i zero = new Vector3i(0, 0, 0);
        public static readonly Vector3i one = new Vector3i(1, 1, 1);
        public static readonly Vector3i forward = new Vector3i(0, 0, 1);
        public static readonly Vector3i back = new Vector3i(0, 0, -1);
        public static readonly Vector3i up = new Vector3i(0, 1, 0);
        public static readonly Vector3i down = new Vector3i(0, -1, 0);
        public static readonly Vector3i left = new Vector3i(-1, 0, 0);
        public static readonly Vector3i right = new Vector3i(1, 0, 0);

        public static readonly Vector3i[] directions = new Vector3i[] {
            forward, back,
            right, left,
            up, down
        };

        public int x, y, z;

        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3i(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vector3i(Vector3 pos)
        {
            this.x = Mathf.FloorToInt(pos.x);
            this.y = Mathf.FloorToInt(pos.y);
            this.z = Mathf.FloorToInt(pos.z);
        }

        ///////////////////////////////////////////////////////////////
        public static Vector3i Mul(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3i Div(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3i Min(Vector3i a, Vector3i b)
        {
            return Process(a, b, Mathf.Min);
        }

        public static Vector3i Max(Vector3i a, Vector3i b)
        {
            return Process(a, b, Mathf.Max);
        }

        public static Vector3i Abs(Vector3i a)
        {
            return Process(a, Mathf.Abs);
        }

        public static Vector3i Floor(Vector3 v)
        {
            return v.ProcessTo3i(Mathf.FloorToInt);
        }

        public static Vector3i Ceil(Vector3 v)
        {
            return v.ProcessTo3i(Mathf.CeilToInt);
        }

        public static Vector3i Round(Vector3 v)
        {
            return v.ProcessTo3i(Mathf.RoundToInt);
        }

        public static Vector3i Process(Vector3i v, Func<int, int> func)
        {
            v.x = func(v.x);
            v.y = func(v.y);
            v.z = func(v.z);
            return v;
        }

        public static Vector3i Process(Vector3i a, Vector3i b, Func<int, int, int> func)
        {
            a.x = func(a.x, b.x);
            a.y = func(a.y, b.y);
            a.z = func(a.z, b.z);
            return a;
        }

        ////////////////////////////////////////////////////////
        public static Vector3i operator -(Vector3i a)
        {
            return new Vector3i(-a.x, -a.y, -a.z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3i operator *(Vector3i v, int factor)
        {
            return new Vector3i(v.x * factor, v.y * factor, v.z * factor);
        }

        public static Vector3i operator /(Vector3i v, int factor)
        {
            return new Vector3i(v.x / factor, v.y / factor, v.z / factor);
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return Mul(a, b);
        }

        public static Vector3i operator /(Vector3i a, Vector3i b)
        {
            return Div(a, b);
        }

        ////////////////////////////////////////////////////////
        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.x == b.x &&
                   a.y == b.y &&
                   a.z == b.z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return a.x != b.x ||
                   a.y != b.y ||
                   a.z != b.z;
        }

        public static implicit operator Vector3(Vector3i v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        ////////////////////////////////////////////////////////
        public override bool Equals(object other)
        {
            if (other is Vector3i == false) return false;
            Vector3i vector = (Vector3i)other;
            return x == vector.x &&
                   y == vector.y &&
                   z == vector.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }

        public override string ToString()
        {
            return string.Format("Vector3i({0} {1} {2})", x, y, z);
        }
    }
}