using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using MessServer.Net.IO;

namespace MessServer
{
    class Client
    {
        public String Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public PacketReader _packetReader;
        private Thread _clientThread;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            byte opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]: Client has connected username:{Username}");

            // Start a new thread for client processing
            _clientThread = new Thread(Process);
            _clientThread.Start();
        }

        private void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: message : {msg}");
                            Program.BroadcastMessage($"[{Username}]: {msg} '{DateTime.Now.ToString("HH:mm")}' ");

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{UID.ToString()} disconnected");
                    Program.DisconnectMessage(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}






