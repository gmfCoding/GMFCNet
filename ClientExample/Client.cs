using Example.Packets;
using GMFCNet;
using GMFCNet.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Example.Client
{
    class Client
    {
        private Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public PacketManager packetManager { get; private set; }
        public ClientPacketHandler packetHandler { get; private set; }

        public Client(IPAddress serverIP, int port)
        {
            packetManager = new PacketManager();
            packetHandler = new ClientPacketHandler(this);

            packetManager.Register<EmptyPacket>(packetHandler.EmptyPacket);
            packetManager.Register<MessagePacket>(packetHandler.MessagePacket);
            packetManager.Register<InvalidPacket>(packetHandler.InvalidPacket);
            packetManager.Register<ClientPacket>(packetHandler.EmptyPacket);
            packetManager.Register<ExitPacket>(packetHandler.ExitPacket);

            packetManager.Register<TimePacket>(packetHandler.TimePacket);
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(serverIP, port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");

            RequestLoop();
        }

        private void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");

            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }

        private void SendRequest()
        {
            Console.Write("Send a request: ");
            string request = Console.ReadLine();

            if (request == "time")
            {
                SendPacket(new TimePacket());
            }

            if (request == "msg")
            {
                Console.WriteLine("Write the message you would like to send to the server:");
                string msg = Console.ReadLine();
                SendPacket(new MessagePacket(msg));
            }

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        private void SendBytes(byte[] bytes)
        {
            ClientSocket.Send(bytes, 0, bytes.Length, SocketFlags.None);
        }

        private void SendPacket(Packet packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    int packetID = packetManager.GetID(packet);
                    Console.WriteLine($"[Client]Sending packet:{packetID}");
                    bw.Write(packetID);
                    packet.Write(bw);
                }
                ms.Flush();
                SendBytes(ms.GetBuffer());
            }
        }

        private void Exit()
        {
            SendPacket(new ExitPacket());
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            System.Threading.Thread.CurrentThread.Abort();
        }

        private void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            using (MemoryStream ms =new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int packetID = br.ReadInt32();

                    var info = packetManager.GetByID(packetID);
                    Packet instance = (Packet)Activator.CreateInstance(info.type);
                    instance.Read(br);
                    info.callback(instance);
                }
            }
        }
    }
}
