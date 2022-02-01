using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMFCNet.Packets
{
    public class EmptyPacket : Packet
    {
        public override void Read(BinaryReader br)
        {
        }

        public override void Write(BinaryWriter bw)
        {
        }
    }
}
