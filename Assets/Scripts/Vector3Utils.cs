using UnityEngine;
using System.Collections;
using System;

namespace Soultia.Util
{
    public static class Vector3Utils
    {
        public static Vector3 Mul(this Vector3 a, Vector3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            return a;
        }

        public static Vector3 Div(this Vector3 a, Vector3 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            a.z /= b.z;
            return a;
        }

        public static Vector3 Process(this Vector3 v, Func<float, float> func)
        {
            v.x = func(v.x);
            v.y = func(v.y);
            v.z = func(v.z);
            return v;
        }

        public static Vector3i ProcessTo3i(this Vector3 v, Func<float, int> func)
        {
            Vector3i vi;
            vi.x = func(v.x);
            vi.y = func(v.y);
            vi.z = func(v.z);
            return vi;
        }
    }
}