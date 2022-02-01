using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Packets;
using GMFCNet.Packets;

namespace Example.Server
{
    class ServerPacketHandler
    {
        Server server;

        public ServerPacketHandler(Server server)
        {
            this.server = server;
        }

        public void TimePacket(TimePacket packet)
        {
            server.Send(new TimePacket(DateTime.Now), packet.ClientID);
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
