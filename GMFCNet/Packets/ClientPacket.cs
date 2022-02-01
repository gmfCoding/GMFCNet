using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMFCNet.Packets
{
    public class ClientPacket : Packet
    {
        private int clientID = -1;

        public int ClientID { get => clientID; set { if (clientID == -1) { clientID = value; } } }

        public override void Read(BinaryReader br)
        {

        }

        public override void Write(BinaryWriter bw)
        {

        }
    }
}
