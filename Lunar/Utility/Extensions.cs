using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public static class Extensions
    {
        public static float[] Multiply(this float[] a, float b) 
        {
            for (int i = 0; i < a.Length; i++) 
            { a[i] *= b; }

            return a;
        }

        public static T[] Add<T>(this T[] a, T b)
        {
            T[] result = new T[a.Length + 1];

            Array.Copy(a, result, a.Length);
            result[a.Length] = b;

            return result;
        }

        public static void RemoveRange<T>(this List<T> a, List<T> b)
        {
            foreach(T value in b) { a.Remove(value); }
        }
    }
}
