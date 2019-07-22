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
            string serverIPPort = "";
            if(args.Length==0) //If the console has not accepted any arguments, ask for the IP.
            {
                Console.WriteLine("\n Please enter the IP address of the server. (Example: 1.1.1.1)");
                serverIPPort = Console.ReadLine();
            }else
            {
                serverIPPort = args[0];
            }
            UDPConnection udpConnection = new UDPConnection(clientID, password, serverIPPort);


            string response = null; //When not null, contains rand_cookie and tcpPortNumber
            while (response==null)
            {
                response = udpConnection.UDPConnect();
            }
            string cookie = "";
            string port = "";
            int space = 0;
            for(int i = 0; i < response.Length; i++)
            {
                if (response[i] == ' ')
                {
                    space = i;
                }
                cookie = response.Substring(0, i);
            }
            byte[] cookieNum = Encoding.ASCII.GetBytes(cookie);
            port = response.Substring(space, response.Length - space);
            byte[] portNum = Encoding.ASCII.GetBytes(port);
            Console.WriteLine("Finished?: "+cookie+" "+port);
            Console.WriteLine(cookieNum + " " + portNum);
            while(true)
            {

            }


        }

    }
}
