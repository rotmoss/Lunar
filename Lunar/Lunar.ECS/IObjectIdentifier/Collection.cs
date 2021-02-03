using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.ECS
{
    public class Collection<T> where T : IObjectIdentifier
    {
        private static List<uint> Ids;

        public List<T> Entries { get => _entries; }
        private List<T> _entries;

        public Dictionary<uint, T> EntryById { get => _entryById; }
        private Dictionary<uint, T> _entryById;

        public Dictionary<uint, List<T>> EntryByOwner { get => _entryByOwner; }
        private Dictionary<uint, List<T>> _entryByOwner;

        public Dictionary<uint, IObjectIdentifier> OwnerById { get => _ownerById; }
        private Dictionary<uint, IObjectIdentifier> _ownerById;

        public T GetEntryById(uint id) => EntryById.ContainsKey(id) ? EntryById[id] : default;
        public List<T> GetEntriesByOwner(uint owner) => EntryByOwner.ContainsKey(owner) ? EntryByOwner[owner] : default;
        public T GetEntryByParent(uint owner) => EntryByOwner.ContainsKey(owner) ? EntryByOwner[owner].FirstOrDefault() : default;
        public IObjectIdentifier GetOwnerById(uint id) => OwnerById.ContainsKey(id) ? OwnerById[id] : default;

        public Collection()
        {
            _entries = new List<T>();
            _entryById = new Dictionary<uint, T>();
            _entryByOwner = new Dictionary<uint, List<T>>();
        }

        /// <summary>
        /// Adds a <seealso cref="T"/> to the collection
        /// </summary>
        /// <param name="owner">The entry's owner. <seealso cref="Entry.Dipsose()"/> will be invoked on <seealso cref="ObjectIdentifier.Disposed"/></param>
        /// <param name="entry">The entry to be added</param>
        public void AddEntry(IObjectIdentifier owner, T entry)
        {
            entry.Id = GetId();
            entry.Parent = owner.Id;
            entry.Disposed += entry.OnOwnerDispose;

            if (!EntryByOwner.ContainsKey(owner.Id)) EntryByOwner.Add(owner.Id, new List<T>());
            EntryByOwner[owner.Id].Add(entry);
            EntryById.Add(entry.Id, entry);
            OwnerById.Add(entry.Id, owner);

            Entries.Add(entry);
        }

        /// <summary>
        /// Removes a <seealso cref="T"/> from the collection
        /// </summary>
        /// <returns></returns>
        public void RemoveEntry(T entry)
        {
            Entries.Remove(entry);
            EntryById.Remove(entry.Id);
            EntryByOwner.Remove(entry.Parent);
            ReleaseId(entry.Id);
        }

        /// <summary>
        /// Generates a unique id and adds it to the List of ids
        /// </summary>
        /// <returns></returns>
        public uint GetId()
        {
            for (uint n = 1; n < uint.MaxValue; n++)
                if (!Ids.Contains(n)) { Ids.Add(n); return n; }
            return 0;
        }

        /// <summary>
        /// Generates a unique id and adds it to the List of ids
        /// </summary>
        /// <returns></returns>
        public void ReleaseId(uint id)
        {
            Ids.Remove(id);
        }

        public void Dispose() 
        { 
            while(Entries.Count > 0)
                Entries[^1].Dispose();

            Entries.Clear();
            EntryById.Clear();
            EntryByOwner.Clear();         
        }
    }
}
