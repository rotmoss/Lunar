using System;

namespace Lunar.ECS
{
    public abstract class ObjectIdentifier<T> : IObjectIdentifier where T : ObjectIdentifier<T>
    {
        public static Collection<T> Collection { get => _collection; }
        private static Collection<T> _collection;

        public event EventHandler Disposed;

        public uint Id { get => _id; set => _id = value; }
        private uint _id;

        public uint Parent { get => _owner; set => _owner = value; }
        private uint _owner;

        public bool Enabled { get => _collection.GetOwnerById(Id).Enabled ? _enabled : false; set => _enabled = value; }
        private bool _enabled;

        public abstract void DerivedDispose();

        public void Dispose()
        {
            Disposed?.Invoke(this, new EventArgs());
            DerivedDispose();

            _collection.RemoveEntry((T)this);
        }

        public void OnOwnerDispose(object sender, EventArgs e) => Dispose();   
    }
}
