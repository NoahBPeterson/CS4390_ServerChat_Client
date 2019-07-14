using System;
using System.Net;

namespace CS4390_ServerChat_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Group 2 Chat Client!");
            Console.WriteLine("\n Enter your client ID.");
            string clientID = Console.ReadLine();
            string serverIPPort = "";
            if(args.Length==0) //If the console has not accepted any arguments, ask for the IP and port.
            {
                Console.WriteLine("\n Please enter the IP address of the server. (Example: 1.1.1.1)");
                serverIPPort = Console.ReadLine();
            }else
            {
                serverIPPort = args[0];
            }
            UDPConnection udpConnection = new UDPConnection(clientID, serverIPPort);


            IPEndPoint response = null;
            while (response==null)
            {
                response = udpConnection.UDPConnect();
            }
            Console.WriteLine("Finished?: "+response.ToString());
                while(true)
            {

            }


        }

    }
}
