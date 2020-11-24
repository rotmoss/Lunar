using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Math
{
    public static class Functions
    {
        public static float Normalize(float x, float min, float max)
        {
            return (x - min) / (max - min);
        }
        public static float Sigmoid(float value, float xMultiple = 1, float yMultiple = 1, float xOffset = 0, float yOffset = 0)
        {
            float k = MathF.Exp(value * xMultiple) * yMultiple;
            return (k / (1 + xOffset + k)) + yOffset;
        }
    }
}
