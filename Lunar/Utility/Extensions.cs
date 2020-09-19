using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lunar
{
    public static class Extensions
    {
        public static T[] Add<T>(this T[] a, T b)
        {
            if (a == null) return new T[] { b };

            T[] result = new T[a.Length + 1];

            Array.Copy(a, result, a.Length);
            result[a.Length] = b;

            return result;
        }

        public static void RemoveRange<T>(this List<T> a, List<T> b)
        {
            foreach(T value in b) { a.Remove(value); }
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }
    }
}
