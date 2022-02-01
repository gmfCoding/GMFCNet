using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Packets;
using GMFCNet;
using GMFCNet.Packets;

namespace Example.Client
{
    class ClientPacketHandler
    {
        Client client;

        public ClientPacketHandler(Client client)
        {
            this.client = client;
        }

        public void TimePacket(TimePacket packet)
        {
            Console.WriteLine($"Time on server is: {packet.time}");
        }

        internal void ExitPacket(ExitPacket obj)
        {
            
        }

        internal void EmptyPacket(Packet obj)
        {

        }

        internal void InvalidPacket(InvalidPacket obj)
        {
            Console.WriteLine(obj.message);
        }

        internal void MessagePacket(MessagePacket obj)
        {
            Console.WriteLine(obj.message);
        }
    }
}
