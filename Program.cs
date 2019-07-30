using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 1000;
            string IpAdress = "127.0.0.1";
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAdress), port);
            ClientSocket.Connect(ep);
            Console.Write("Client is connected\n");
            while(true)
            {
                string messageFromClient = null;
                Console.WriteLine("Enter the Message");
                messageFromClient = Console.ReadLine();
                ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(messageFromClient), 0, messageFromClient.Length, SocketFlags.None);
                byte[] msgFromServer = new byte[1024];
                int size = ClientSocket.Receive(msgFromServer);
                Console.WriteLine("Server" + System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size));

            }
        }
    }
}
