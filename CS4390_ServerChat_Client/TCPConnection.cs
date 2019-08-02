using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
        string Cipher;
        public TCPConnection(int cookie, IPEndPoint server, string cipher)
        {
            Cipher = cipher;
            this.cookie = cookie;
            serverEndpoint = server;
        }

        public bool TCPConnect()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(serverEndpoint);
            Console.Write("Client is connected\n");
            sendPlain(cookie.ToString()); //Need to send the rand_cookie to server for TCP so it knows who we are.
            if (receive() == "CONNECTED")
            {
                Console.WriteLine("Handshake successful!");
                return true;
            }

            return false;
        }

        public void send(string Message)
        {
            byte[] cipherMessage = Encryption.Encrypt(Message, Cipher);
            ClientSocket.Send(cipherMessage, 0, cipherMessage.Length, SocketFlags.None);
        }

        public void sendPlain(string message) {
            ClientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        public string receive()
        {
            byte[] msgFromServer = new byte[1024];
            int size = ClientSocket.Receive(msgFromServer);
            byte[] msg = new byte[size];
            Array.Copy(msgFromServer, msg, size);
            return Encryption.Decrypt(msg, Cipher);
        }

        public void Terminate() {
            ClientSocket.Dispose();
        }
    }
}
