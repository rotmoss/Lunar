using Silk.NET.Maths;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace Lunar.ECS 
{
    public class Transform : Component<Transform>, INotifyPropertyChanged
    {
        public static Transform Zero = new Transform();

        public Vector3D<float> Position { get => _position; set { _position = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position")); } }
        private Vector3D<float> _position;

        public Vector3D<float> Scale { get => _scale; set { _scale = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Scale")); } }
        private Vector3D<float> _scale;

        public event PropertyChangedEventHandler PropertyChanged;

        public Transform(float x = 0, float y = 0, float z = 0, float w = 1, float h = 1, float d = 1)
        {
            _position = new Vector3D<float>(x, y, z);
            _scale = new Vector3D<float>(w, h, d);
        }
        public Transform(Vector3D<float> position, Vector3D<float> scale)
        {
            _position = position;
            _scale = scale;
        }

        public static Transform operator +(Transform a, Transform b) 
        {
            return new Transform(a._position + b._position, a._scale);
        }

        public static Transform GetLocalTransform(Guid id) 
        {
            return Collection[id];
        }

        public static Transform GetGlobalTransform(Guid id)
        {
            Transform t = Collection[id];

            foreach (Guid parent in Gameobject.Collection[id].GetParents())
                t += Collection[parent];

            return t;
        }
    }
}