using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CS4390_ServerChat_Client
{
    public class UDPConnection
    {
        string clientID;
	    public UDPConnection(string clientID)
	    {
            this.clientID = clientID;
        }


        public IPEndPoint UDPConnect(string hostIP)
        {
            IPAddress serverAddress = IPAddress.Parse(hostIP);
            IPEndPoint hostEndPoint = new IPEndPoint(serverAddress, 10020);

            Socket udpConnectionSetup = UDPSend(hostEndPoint);
            int tcpPort = UDPReceive(udpConnectionSetup);
            if(tcpPort<5)
            {
                return null;
            }
            IPEndPoint tcpHostEndPoint = new IPEndPoint(serverAddress, tcpPort);
            return tcpHostEndPoint;
        }

        //Call this function with string "IP:PORT" for example "192.168.1.1:10401"
        //Function returns response from server after sending "HELLO"
        public Socket UDPSend(IPEndPoint serverEndPoint)
        {
            Socket sock = null;

            try
            {
                
                sock = new Socket(SocketType.Dgram, ProtocolType.Udp);
                //SocketType.Dgram (UDP) /Stream (TCP)
                //ProtocolType.UDP/TCP


                //Send "HELLO" to server using UDP.
                string hello = clientID; //Sends "Hello" (Client-ID)
                byte[] buffer = Encoding.ASCII.GetBytes(hello);
                sock.SendTo(buffer, serverEndPoint);

            }
            catch (SocketException e) { }
            catch (ArgumentNullException e) { }
            catch (Exception e) { }
            return sock;
        }

        public int UDPReceive(Socket sock)
        {
            string receiveString = "";

            //Receive response from server using same socket.
            byte[] receiveBytes = null;
            try
            {
                Int32 receive = sock.Receive(receiveBytes);
                receiveString += Encoding.ASCII.GetString(receiveBytes); //(receive, 0, receiveBytes);

                while (receive > 0)
                {
                    receive = sock.Receive(receiveBytes, receiveBytes.Length, 0);
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


            sock.Close();  //Close socket when done.

            return 0; //Return TCP port number

        }
    }
}