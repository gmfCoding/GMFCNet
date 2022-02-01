using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMFCNet.Packets;

namespace Example.Packets
{
    public class TimePacket : ClientPacket
    {
        public DateTime time { get; private set; }
        public TimePacket()
        {
        }

        public TimePacket(DateTime time)
        {
            this.time = time;
        }
        public override void Read(BinaryReader br)
        {
            time = DateTime.FromBinary(br.ReadInt64());
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(time.ToBinary());
        }
    }
}
