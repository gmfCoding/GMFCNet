using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMFCNet.Packets;

namespace GMFCNet
{
    public class PacketManager
    {
        public class PacketInfo {
            public readonly Type type;
            public readonly int packetID;
            public readonly Action<Packet> callback;

            public PacketInfo(Type packet, int packetID, Action<Packet> callback)
            {
                this.type = packet;
                this.packetID = packetID;
                this.callback = callback;
            }
        }

        List<PacketInfo> packetTypes = new List<PacketInfo>();

        public int GetID(Packet packet)
        {
            foreach (var item in packetTypes)
            {
                if (item.type == packet.GetType())
                {
                    return item.packetID;
                }
            }
            return 0;
            //return packetTypes.Where(x => x.type.GetType() == packet.GetType()).Select(x => x.packetID).FirstOrDefault();
        }

        public PacketInfo GetByID(int id)
        {
            return packetTypes.Where(x => x.packetID == id).Select(x => x).FirstOrDefault();
        }

        Action<Packet> GetCallback(Packet packet)
        {
            return null;
        }

        public void Register<T>(Action<T> recieveCallback) where T : Packet
        {
            packetTypes.Add(new PacketInfo(typeof(T), packetTypes.Count + 1, (a) => { recieveCallback((T)a); }));
        }
    }
}
