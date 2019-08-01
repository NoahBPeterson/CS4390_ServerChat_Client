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

        public void TCPConnect()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(serverEndpoint);
            Console.Write("Client is connected\n");
            send(cookie.ToString()); //Need to send the rand_cookie to server for TCP so it knows who we are.
            if(receive().Equals("CONNECTED"))
            {
                Console.WriteLine("Handshake successful!");
            }
            bool exit = false;
            Thread receiveFromServer = new Thread(this.receivePrintLoop);
            receiveFromServer.Start();
            while (!exit)
            {
                string messageFromClient = null;
                Console.WriteLine("Enter the Message");
                messageFromClient = Console.ReadLine();
                send(messageFromClient);
                if (messageFromClient.Equals("exit")) exit = true;
            }
            receiveFromServer.Abort();
            ClientSocket.Close();
        }

        public void receivePrintLoop()
        {
            while(true)
            {
                Console.WriteLine(receive());
            }
        }

        public void send(string Message)
        {
            ClientSocket.Send(System.Text.Encoding.UTF8.GetBytes(Message), 0, Message.Length, SocketFlags.None);

        }

        public string receive()
        {
            byte[] msgFromServer = new byte[1024];
            int size = ClientSocket.Receive(msgFromServer);
            return System.Text.Encoding.UTF8.GetString(msgFromServer, 0, size);
        }

        public string Encrypt(string messageSent)
        {
            using (var CryptoMD5 = new MD5CryptoServiceProvider())
            {
                using (var TripleDES = new TripleDESCryptoServiceProvider())
                {
                    TripleDES.Key = CryptoMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Cipher));
                    TripleDES.Mode = CipherMode.ECB;
                    TripleDES.Padding = PaddingMode.PKCS7;

                    using (var crypt = TripleDES.CreateEncryptor())
                    {
                        byte[] messageBytes = UTF8Encoding.UTF8.GetBytes(messageSent);
                        byte[] totalBytes = crypt.TransformFinalBlock(messageBytes, 0, messageBytes.Length);
                        return Convert.ToBase64String(totalBytes, 0, totalBytes.Length);
                    }
                }
            }
        }
        
        public string Decrypt(string encryptedMessage)
        {
            using (var CryptoMD5 = new MD5CryptoServiceProvider())
            {
                using (var TripleDES = new TripleDESCryptoServiceProvider())
                {
                    TripleDES.Key = CryptoMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Cipher));
                    TripleDES.Mode = CipherMode.ECB;
                    TripleDES.Padding = PaddingMode.PKCS7;

                    using (var crypt = TripleDES.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(encryptedMessage);
                        byte[] totalBytes = crypt.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(totalBytes);
                    }
                }
            }
        }
    }
}
