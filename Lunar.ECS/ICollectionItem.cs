using System.Collections;
using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public interface ICollectionItem
    {
        event Action Disposed;

        ICollectionItem Ancestor { get; set; }
        ICollectionItem Parent { get; set; }

        Guid Id { get; set; }
        string Name { get; set; }
        bool Enabled { get; set; }

        void Dispose();    
        bool Equals(ICollectionItem obj);
    }
}