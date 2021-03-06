using System.ComponentModel.Design;
using System;
using System.Collections.Generic;

namespace Lunar.ECS
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> The item present in the collection </typeparam>
    public interface ITree<T> where T : ITreeItem 
    {
        /// <summary>
        /// Adds an item to the Collection
        /// </summary>
        /// <param name="item"></param>
        void Add(T item, ITreeItem parent);

        /// <summary>
        /// Adds an Removes to the Collection
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);

        public T this[Guid i] { get; }
        public T this[string s] { get; }
    }
}