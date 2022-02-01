using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMFCNet.Packets
{
    public abstract class Packet
    {
        public abstract void Write(BinaryWriter bw);
        public abstract void Read(BinaryReader br);
    }
}
