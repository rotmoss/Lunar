using System.Collections;
using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    public interface ITreeItem
    {
        event Action Disposed;

        ITreeItem Ancestor { get; set; }
        ITreeItem Parent { get; set; }

        Guid Id { get; set; }
        string Name { get; set; }
        bool Enabled { get; set; }

        void Dispose();    
        bool Equals(ITreeItem obj);
    }
}