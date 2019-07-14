using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CS4390_ServerChat_Client
{
    public class UDPConnection
    {
        string clientID;
        IPEndPoint serverAddress;
        Socket udpConnectionSocket;
	    public UDPConnection(string clientID, string serverIP)
	    {
            this.clientID = clientID;
            serverAddress = new IPEndPoint(IPAddress.Parse(serverIP), 10020);
            udpConnectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpConnectionSocket.Connect(serverAddress);
        }


        public IPEndPoint UDPConnect()
        {
            //while(clientID=="noahb")
            { 
                UDPSend(clientID); // HELLO
                //Console.WriteLine("Sent another Hello!");
            }
            int tcpPort = UDPReceive();
            if(tcpPort<5)
            {
                return null;
            }
            IPEndPoint tcpHostEndPoint = new IPEndPoint(serverAddress.Address, tcpPort);
            return tcpHostEndPoint;
        }

        //Call this function with string you wish to send to the server (Client ID, challenge response, etc.
        
        public void UDPSend(String sendText)
        {
            try
            {


                //Send message to server using UDP.
                byte[] buffer = Encoding.ASCII.GetBytes(sendText);
                //udpConnectionSocket.SendToAsync( //(buffer, SocketFlags.None);
                udpConnectionSocket.SendTo(buffer, buffer.Length, SocketFlags.None, serverAddress);
                udpConnectionSocket.SendTo(buffer, serverAddress);

            }
            catch (SocketException e) { }
            catch (ArgumentNullException e) { }
            catch (Exception e) { }
        }

        public int UDPReceive()
        {
            string receiveString = "";

            //Receive response from server using same socket.
            byte[] receiveBytes = new byte[1024];
            try
            {
                EndPoint EP = (EndPoint)serverAddress;
                Int32 receive = udpConnectionSocket.ReceiveFrom(receiveBytes, ref EP);
                receiveString += Encoding.ASCII.GetString(receiveBytes); //(receive, 0, receiveBytes);
                receiveString = receiveString.Substring(0, receive);

                while (receive > 0)
                {
                    receive = udpConnectionSocket.ReceiveFrom(receiveBytes, receiveBytes.Length, 0, ref EP);
                    receiveString += Encoding.ASCII.GetString(receiveBytes, receiveBytes.Length, 0);
                }

                int tcpPort;

                if (int.TryParse(receiveString, out tcpPort) == true)
                {
                    return tcpPort;
                }
                else
                {
                    Console.WriteLine("DEBUG: Received string from server: " + receiveString);
                }
            }
            catch (Exception e) { }


            udpConnectionSocket.Close();  //Close socket when done.

            return 0; //Return TCP port number

        }
    }
}