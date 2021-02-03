using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.ECS
{
    public interface IObjectIdentifier : IDisposable
    {
        public event EventHandler Disposed;

        public uint Id { get; set; }
        public uint Parent { get; set; }
        public bool Enabled { get; set; }

        public void OnOwnerDispose(object sender, EventArgs e);
    }
}
