using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public class Transform
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float h;

        public Transform(float x = 0, float y = 0, float z = 0, float w = 0, float h = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.h = h;
        }

        public static Transform operator +(Transform a, Transform b) => new Transform(a.x + b.x, a.y + b.y, a.z + b.z, a.w * b.w, a.h * b.h);

        public Matrix4x4f ToMatrix4x4f()
        {
            Matrix4x4f value = Matrix4x4f.Identity;
            value.Translate(x, y, z);
            value.Scale(w, h, 1);
            return value;
        }
    }
}
