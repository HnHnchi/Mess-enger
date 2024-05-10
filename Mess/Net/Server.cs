using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Mess.Net.IO;
using MessServer.Net.IO;

namespace Mess.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;
        PacketBuilder _packetbuilder;
        public event Action connectedEvent;
        public event Action msgRecivedEvent;
        public event Action UserDisconnectEvent;
        public Server()
        {
            _client = new TcpClient();
        }
        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
               



            }

        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1: connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgRecivedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectEvent?.Invoke();
                            break;
                        default: Console.WriteLine("ah yes..");
                            break;

                    }
                }
            });
        }
        public void SendMessageToServer(string message) { 
        var messagePacket = new PacketBuilder();
            messagePacket.WriteMessage(message);
            messagePacket.WriteOpCode(5);
            _client.Client.Send(messagePacket.GetPacketBytes());
        
        }
    }
}


