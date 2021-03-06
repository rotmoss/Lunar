using System;
using Lunar.OpenGL;

namespace Lunar 
{
    public static class Extentions 
    {
        public static float[] ConcatArrays(params float[][] list)
        {
            int lenghtSum = 0;

            for(int i = 0; i < list.Length; i++) {
                lenghtSum += list[i].Length;
            }

            var result = new float[lenghtSum];
            int offset = 0;

            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }

            return result;
        }

        public static void Add<T>(ref T[] a, T b) where T : struct
        {
            T[] newArray = new T[a.Length + 1];
            Array.Copy(a, newArray, a.Length);
            newArray[^1] = b;
            a = newArray;
        }

        public static IVec ToVec(this float[] a) 
        {
            if(a.Length == 1) return new Vec1(a[0]);
            else if(a.Length == 2) return new Vec2(a[0], a[1]);
            else if(a.Length == 3) return new Vec3(a[0], a[1], a[2]);
            else if(a.Length == 4) return new Vec4(a[0], a[1], a[2], a[3]);
            else return null;
        }
    }
}