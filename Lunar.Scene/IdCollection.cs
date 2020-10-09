using System.Collections;
using System.Collections.Generic;

namespace Lunar
{
    public class IdCollection : IEnumerable<uint>
    {
        public List<uint> Values { get; set; }

        public IdCollection()
        {
            Values = new List<uint>();
        }
      
        public uint GetId() // Generates a unique id and adds it to the List
        {
            for (uint n = 1; n < uint.MaxValue; n++)
                if (!Values.Contains(n)) { Values.Add(n); return n; }
            return 0;
        }

        public void Remove(uint id) { Values.Remove(id); }
        public bool Contains(uint id) => Values.Contains(id);

        public IEnumerator<uint> GetEnumerator() => ((IEnumerable<uint>)Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();
    }
}
