namespace Lunar.OpenGL
{
    public interface IVec 
    {
        float[] GetValues();
        IVec Convert(int length);
    }

    public struct Vec1 : IVec
    {
        public static Vec1 Zero = new Vec1 { x = 0 };
        public static readonly int Size = 4;

        public float x;

        public Vec1(float x) {
            this.x = x;
        }
        
        public float[] GetValues() => new float[] { x };

        public static implicit operator Vec2(Vec1 v) => new Vec2(v.x, 0);
        public static implicit operator Vec3(Vec1 v) => new Vec3(v.x, 0, 0);
        public static implicit operator Vec4(Vec1 v) => new Vec4(v.x, 0, 0, 0);

        public IVec Convert(int length) 
        {
            if(length == 2) return new Vec2(x, 0);
            if(length == 3) return new Vec3(x, 0, 0);
            if(length == 4) return new Vec4(x, 0, 0, 0);

            return this;
        } 
    }
   
    public struct Vec2 : IVec
    {
        public static Vec2 Zero = new Vec2 { x = 0, y = 0 };
        public static readonly int Size = 8;

        public float x;
        public float y;

        public Vec2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public float[] GetValues() => new float[] { x, y, };

        public static implicit operator Vec1(Vec2 v) => new Vec1(v.x);
        public static implicit operator Vec3(Vec2 v) => new Vec3(v.x, v.y, 0);
        public static implicit operator Vec4(Vec2 v) => new Vec4(v.x, v.y, 0, 0);

        public IVec Convert(int length) 
        {
            if(length == 1) return new Vec1(x);
            if(length == 3) return new Vec3(x, y, 0);
            if(length == 4) return new Vec4(x, y, 0, 0);

            return this;
        } 
    }
 
    public struct Vec3 : IVec
    {
        public static Vec3 Zero = new Vec3 { x = 0, y = 0, z = 0 };
        public static readonly int Size = 12;

        public float x;
        public float y;
        public float z;

        public Vec3(float x, float y, float z) 
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float[] GetValues() => new float[] { x, y, z };

        public static implicit operator Vec1(Vec3 v) => new Vec1(v.x);
        public static implicit operator Vec2(Vec3 v) => new Vec2(v.x, v.y);
        public static implicit operator Vec4(Vec3 v) => new Vec4(v.x, v.y, v.z, 0);

        public IVec Convert(int length) 
        {
            if(length == 1) return new Vec1(x);
            if(length == 2) return new Vec2(x, y);
            if(length == 4) return new Vec4(x, y, z, 0);

            return this;
        } 
    }

    public struct Vec4 : IVec
    {
        public static Vec4 Zero = new Vec4 { x = 0, y = 0, z = 0, w = 0 };
        public static readonly int Size = 16;

        public float x;
        public float y;
        public float z;
        public float w;

        public Vec4(float x, float y, float z, float w) 
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float[] GetValues() => new float[] { x, y, z, w };

        public static implicit operator Vec1(Vec4 v) => new Vec1(v.x);
        public static implicit operator Vec2(Vec4 v) => new Vec2(v.x, v.y);
        public static implicit operator Vec3(Vec4 v) => new Vec3(v.x, v.y, v.z);

        public IVec Convert(int length) 
        {
            if(length == 1) return new Vec1(x);
            if(length == 2) return new Vec2(x, y);
            if(length == 3) return new Vec3(x, y, z);

            return this;
        } 
    }
}