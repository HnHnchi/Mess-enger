
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Mess.Net.IO;
using MessServer.Net.IO;

namespace MessServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;

        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            Console.WriteLine("Server started. Listening for connections...");

            while (true)
            {
                var Client = new Client(_listener.AcceptTcpClient());
                _users.Add(Client);

                BroadcastConnection();


            }
        }

        /* static void HandleClient(TcpClient tcpClient)
         {
             try
             {
                 Client client = new Client(tcpClient);
                 _users.Add(client);
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Error handling client: {ex.Message}");
                 // Handle any errors or disconnects here
             }
         }*/


        static void BroadcastConnection()
        {

            // Create a copy of the _users list to avoid modification during iteration
            List<Client> usersCopy;
            lock (_users)
            {
                usersCopy = new List<Client>(_users);
            }

            foreach (var user in usersCopy)
            {
                foreach (var usr in usersCopy)
                {
                    var broadCastPacket = new PacketBuilder();
                    broadCastPacket.WriteOpCode(1);
                    broadCastPacket.WriteMessage(usr.Username);
                    broadCastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadCastPacket.GetPacketBytes());
                }
            }



        }
        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());

            }
        }
        public static void DisconnectMessage(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

            }
            BroadcastMessage($"{disconnectedUser.Username}: disconnected");
        }
    }
}



