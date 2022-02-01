using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMFCNet.Packets
{
    public class InvalidPacket : MessagePacket
    {
        public InvalidPacket() : base("")
        {
                
        }

        public InvalidPacket(string message) : base(message)
        {

        }
    }
}
