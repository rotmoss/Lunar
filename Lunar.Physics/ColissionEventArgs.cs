using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Physics
{
    public class ColissionEventArgs : EventArgs
    {
        public bool movable;
        public Collider collider;
        public Collider reciever;
        public Side side;
    }
}
