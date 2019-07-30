using System;
using System.Net;
using System.Text;

namespace CS4390_ServerChat_Client
{
    class Program
    {
        static void Main(string[] args) //If you run this with the argument '192.168.1.2', it'll use that as the server address to connect to.
        {
            Console.WriteLine("Welcome to the Group 2 Chat Client!");
            Console.WriteLine("\n Enter your client ID.");
            string clientID = Console.ReadLine();
            Console.WriteLine("\n Enter your client password.");
            string password = Console.ReadLine();
            string serverIP = "";
            if(args.Length==0) //If the console has not accepted any arguments, ask for the IP.
            {
                Console.WriteLine("\n Please enter the IP address of the server. (Example: 1.1.1.1)");
                serverIP = Console.ReadLine();
            }else
            {
                serverIP = args[0];
            }
            UDPConnection udpConnection = new UDPConnection(clientID, password, serverIP);
            string response = udpConnection.UDPConnect();
            if (response.Equals("FAIL"))
            {
                Console.ReadKey();
                Main(args);
            }
            int[] cookie_port = UDPConnection.udpParse(response);
            TCPConnection tcpConnection = new TCPConnection(cookie_port[0], new IPEndPoint(IPAddress.Parse(serverIP), cookie_port[1]));
            tcpConnection.TCPConnect();
            //tcpConnection.send/receive();

            while (true)
            {

            }


        }

    }
}
