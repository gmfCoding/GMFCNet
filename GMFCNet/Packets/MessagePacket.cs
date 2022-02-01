using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMFCNet.Packets
{
    public class MessagePacket : Packet
    {
        public string message { get; private set; }
        public MessagePacket()
        {
            message = "";
        }

        public MessagePacket(string message)
        {
            this.message = message == null ? "" : message;
        }

        public override void Read(BinaryReader br)
        {
            message = br.ReadString();
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(message);
        }
    }
}
