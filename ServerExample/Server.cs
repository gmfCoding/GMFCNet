using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Example.Packets;
using GMFCNet;
using GMFCNet.Packets;

namespace Example.Server
{
    class Server
    {
        public Socket serverSocket { get; private set; }
        public List<ServerClient> clientSockets { get; private set; }

        public PacketManager packetManager { get; private set; }
        public ServerPacketHandler packetHandler { get; private set; }

        private const int PORT = 5000;

        public Server()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSockets = new List<ServerClient>();
            packetManager = new PacketManager();
            packetHandler = new ServerPacketHandler(this);

            packetManager.Register<EmptyPacket>(packetHandler.EmptyPacket);
            packetManager.Register<MessagePacket>(packetHandler.MessagePacket);
            packetManager.Register<InvalidPacket>(packetHandler.InvalidPacket);
            packetManager.Register<ClientPacket>(packetHandler.EmptyPacket);
            packetManager.Register<ExitPacket>(packetHandler.ExitPacket);

            packetManager.Register<TimePacket>(packetHandler.TimePacket); // Recieving a TimePacket on the server means the Client is requesting the server Time, so we should send back a TimePacket

            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;
            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            ServerClient client = new ServerClient(socket, this);
            clientSockets.Add(client);
            socket.BeginReceive(clientSockets.Last().buffer, 0, clientSockets.Last().buffer.Length, SocketFlags.None, client.ReceiveCallback, this);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }
        public void Send(Packet packet, int clientID)
        {
            var found = clientSockets.Where(x => x.clientID == clientID);
            if (found.Count() > 0)
            {
                found.First().SendPacket(packet);
            }
            else
            {
                Console.WriteLine($"Cannot find client with ID:{clientID}");
            }
        }
    }

    class ServerClient
    {
        private Server server;
        public readonly int clientID;
        public Socket clientSocket { get; private set; }
        public readonly byte[] buffer = new byte[2048];
        public static int clients { get; private set; }
        public ServerClient(Socket clientSocket, Server sever)
        {
            this.clientSocket = clientSocket;
            this.server = sever;
            clients++;
            clientID = clients;
        }

        public void ReceiveCallback(IAsyncResult AR)
        {
            Server server = (Server)AR.AsyncState;
            int received;

            try
            {
                received = clientSocket.EndReceive(AR);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ToString());
                Disconnect();
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);

            using (MemoryStream ms = new MemoryStream(recBuf))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int packetID = -1;
                    try
                    {
                        packetID = br.ReadInt32();
                        Console.WriteLine($"[Server]Recieved packet:{packetID}");
                        var info = server.packetManager.GetByID(packetID);
                        Packet instance = (Packet)Activator.CreateInstance(info.type);

                        if (instance is ClientPacket cp)
                        {
                            cp.ClientID = clientID;
                        }

                        instance.Read(br);
                        info.callback(instance);

                        if (!(instance is ClientPacket))
                        {
                            SendPacket(new EmptyPacket());
                        }
                    }
                    catch (Exception)
                    {
                        SendPacket(new InvalidPacket($"The packet:{packetID} that was sent is invalid!"));
                        Console.WriteLine($"Recieved an invalid packet:{packetID} from:{clientID}");
                    }
                }
            }

            if (clientSocket != null)
            {
                try
                {
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, server);
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    Console.WriteLine(se.ToString());
                    Disconnect();
                }
                
            }
            
        }

        private void SendBytes(byte[] bytes)
        {
            try
            {
                clientSocket.Send(bytes);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ToString());
                Disconnect();
            }
            catch (NullReferenceException)
            {
                Disconnect();
            }
        }

        public void SendPacket(Packet packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(server.packetManager.GetID(packet));
                    packet.Write(bw);
                }
                ms.Flush();
                SendBytes(ms.GetBuffer());
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"Disconnected Client:{clientID}");
            server.clientSockets.Remove(this);
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }
        }
    }
}
