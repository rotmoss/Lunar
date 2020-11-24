using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunar.Editor
{
    public class Field<T> where T : struct
    {
        public TextBlock Name;
        public Variable<T>[] Vars;

        public Field(string name, params Variable<T>[] vars)
        {
            Name = new TextBlock { Text = name, FontSize = 14, Margin = new Thickness(4, 2, 4, 2) };
            Vars = vars;
        }
    }

    public class Variable<T> where T : struct
    {
        public TextBlock Name;
        public TextBox Input;

        public Variable(string name)
        {
            Name = new TextBlock { Text = name, FontSize = 14, Margin = new Thickness(4, 2, 4, 2) };
            Input = new TextBox { };
        }

        public bool TryGetValue(out T value)
        {
            if (typeof(T) == typeof(float))
            {
                float tempValue;
                bool result = float.TryParse(Input.Text, out tempValue);
                value = (T)(object)tempValue;

                return result;
            }
            if (typeof(T) == typeof(int))
            {
                int tempValue;
                bool result = int.TryParse(Input.Text, out tempValue);
                value = (T)(object)tempValue;

                return result;
            }
            if (typeof(T) == typeof(byte))
            {
                byte tempValue;
                bool result = byte.TryParse(Input.Text, out tempValue);
                value = (T)(object)tempValue;

                return result;
            }
            if (typeof(T) == typeof(uint))
            {
                uint tempValue;
                bool result = uint.TryParse(Input.Text, out tempValue);
                value = (T)(object)tempValue;

                return result;
            }
            if (typeof(T) == typeof(string))
            {
                value = (T)(object)Input.Text;
                return true;
            }

            value = default;
            return false;
        }
    }
}
