using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

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

            if (args.Length==0) //If the console has not accepted any arguments, ask for the IP.
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
            TCPConnection tcpConnection = new TCPConnection(cookie_port[0], new IPEndPoint(IPAddress.Parse(serverIP), cookie_port[1]), udpConnection.privateKeyCipher);
            bool connected = tcpConnection.TCPConnect();
            if (!connected) {

            }
            bool running = true;

            while (running) {
                while (true) {
                    string input = Console.ReadLine();
                    string[] tokens = input.Split(' ');

                    if (tokens[0] == "chat") {
                        tcpConnection.send(input);
                        string requestResponse = tcpConnection.receive();

                        string[] responseTokens = requestResponse.Split(' ');

                        if (responseTokens[0] == "UNREACHABLE") {
                            Console.WriteLine("{0} is unreachable", responseTokens[1]);
                        } else if (responseTokens[0] == "CHAT_STARTED") {
                            break;
                        }
                    } else if (tokens[0] == "exit") {
                        running = false;
                        break;
                    } else {
                        Console.WriteLine("Unrecognized command");
                    }
                }

                if (!running) break;

                ChatInterface chatInterface = new ChatInterface(Console.WindowWidth, Console.WindowHeight);
                Console.Clear();
                bool exit = false;

                Thread listenThread = new Thread(() => {
                    while (!exit) {
                        string message = tcpConnection.receive();
                        chatInterface.PushMessage(string.Format("[SERVER]: {0}", message));
                    }
                });

                listenThread.Start();

                while (!exit) {
                    Thread.Sleep(20);
                    chatInterface.Update();
                    string messageFromClient = null;
                    messageFromClient = Console.ReadLine();
                    tcpConnection.send(messageFromClient);
                    chatInterface.PushMessage(string.Format("{0}: {1}", clientID, messageFromClient));
                    if (messageFromClient.Equals("exit")) exit = true;
                }

                listenThread.Join();
            }
        }

    }
}
