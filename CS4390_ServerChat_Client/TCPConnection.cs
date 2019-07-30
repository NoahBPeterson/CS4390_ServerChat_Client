using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS4390_ServerChat_Client
{
    class TCPConnection
    {
        int cookie;
        IPEndPoint serverEndpoint;
        Socket ClientSocket;
        public TCPConnection(int cookie, IPEndPoint server)
        {
            this.cookie = cookie;
            serverEndpoint = server;
        }

        public void TCPConnect()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(serverEndpoint);
            Console.Write("Client is connected\n");
            while (true)
            {
                string messageFromClient = null;
                Console.WriteLine("Enter the Message");
                messageFromClient = Console.ReadLine();

            }
        }

        public void send(string Message)
        {
            ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(Message), 0, Message.Length, SocketFlags.None);

        }

        public string receive()
        {
            byte[] msgFromServer = new byte[1024];
            int size = ClientSocket.Receive(msgFromServer);
            return System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size));
        }
    }
}
