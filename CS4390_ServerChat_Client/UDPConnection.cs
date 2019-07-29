﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace CS4390_ServerChat_Client
{
    public class UDPConnection
    {
        string clientID, privateKey;
        IPEndPoint serverAddress;
        IPEndPoint clientAddress;
        Socket udpConnectionSocket;
	    public UDPConnection(string clientID, string clientPass, string serverIP)
	    {
            this.clientID = clientID;
            privateKey = clientPass;
            serverAddress = new IPEndPoint(IPAddress.Parse(serverIP), 10020);
            clientAddress = new IPEndPoint(IPAddress.Any, 0); //Our address
            udpConnectionSocket = new Socket(clientAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            udpConnectionSocket.Bind(clientAddress);
        }


        public string UDPConnect()
        {
            UDPSend(clientID); // HELLO
            string response = UDPReceive(); //Receive challenge (random integer, challenge)
            byte[] serverChallenge;
            serverChallenge = Encoding.ASCII.GetBytes(response);
            string clientResponse = Encoding.ASCII.GetString(challengeReponse(serverChallenge)); //Get response, convert from byte[] to string
            UDPSend(clientID +" "+ clientResponse); //Send clientID + challenge Response

            string tcp = UDPReceive(); //Receive rand_cookie, port_number for TCP connection
            if (tcp.Equals("FAIL"))
            {
                Console.WriteLine("Authentication failed.");
            }
            udpConnectionSocket.Close();  //Close socket when done.
            return tcp;
        }

        public byte[] challengeReponse(byte[] rand)
        {
            SHA256 encryptionObject = SHA256.Create();
            byte[] randKey = Encoding.ASCII.GetBytes(rand.ToString() + privateKey);
            byte[] randHash = encryptionObject.ComputeHash(randKey); //Hashes challenge with out privateKey (password)
            
            return randHash;
        }

        //Call this function with string you wish to send to the server (Client ID, challenge response, etc.
        
        public void UDPSend(String sendText)
        {
            try
            {
                //Send message to server using UDP.
                byte[] buffer = Encoding.ASCII.GetBytes(sendText);
                EndPoint hostEndPoint = (EndPoint)serverAddress;
                udpConnectionSocket.SendTo(buffer, buffer.Length, SocketFlags.None, hostEndPoint);
                //udpConnectionSocket.SendTo(buffer, hostEndPoint);

            }
            catch (SocketException e) { }
            catch (ArgumentNullException e) { }
            catch (Exception e) { }
        }

        public string UDPReceive()
        {
            string receiveString = "";

            byte[] receiveBytes = new byte[1024];
            try
            {
                EndPoint EP = (EndPoint)serverAddress;
                udpConnectionSocket.ReceiveTimeout = 1200000;
                Int32 receive = udpConnectionSocket.ReceiveFrom(receiveBytes, ref EP);
                receiveString += Encoding.ASCII.GetString(receiveBytes);
                receiveString = receiveString.Substring(0, receive);
                return receiveString;
            }
            catch (Exception e) { }

            return "";

        }
    }
}