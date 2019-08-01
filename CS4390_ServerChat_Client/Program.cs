using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace CS4390_ServerChat_Client
{
    public enum State {
        None,
        Chatting
    }

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

            string chatPartner = null;
            State state = State.None;
            bool running = true;
            Queue<string> queue = new Queue<string>();

            ChatInterface chatInterface = new ChatInterface(Console.WindowWidth, Console.WindowHeight);
            Console.Clear();
            chatInterface.PushMessage(string.Format("Connected to server as: {0}", clientID));

            Thread listenThread = new Thread(() => {
                while (running) {
                    string message = tcpConnection.receive();
                    lock (queue) {
                        queue.Enqueue(message);
                    }
                }
            });

            listenThread.Start();
            
            while (true) {
                Thread.Sleep(20);
                lock (queue) {
                    while (queue.Count > 0) {
                        string serverMessage = queue.Dequeue();
                        string[] tokens = serverMessage.Split(' ');

                        if (tokens[0] == "UNREACHABLE") {
                            Console.WriteLine("{0} is unreachable", tokens[1]);
                        } else if (tokens[0] == "CHAT_STARTED") {
                            chatPartner = tokens[1];
                            state = State.Chatting;
                            chatInterface.PushMessage(string.Format("Connected to {0}", tokens[1]));
                        } else if (tokens[0] == "END_NOTIF") {
                            state = State.None;
                            chatInterface.PushMessage(string.Format("Chat ended with {0}", tokens[1]));
                        } else {
                            chatInterface.PushMessage(string.Format("[SERVER]: {0}", serverMessage));
                        }
                    }
                }

                string input = chatInterface.Update();
                if (input == null) continue;

                if (state == State.None) {
                    string[] tokens = input.Split(' ');

                    if (tokens[0] == "chat") {
                        tcpConnection.send(input);
                    } else if (tokens[0] == "logoff") {
                        running = false;
                        break;
                    } else if (tokens[0] == "history") {
                        tcpConnection.send(input);
                    } else {
                        chatInterface.PushMessage("Unrecognized command");
                    }
                } else if (state == State.Chatting) {
                    if (input == "endchat") {
                        state = State.None;
                        tcpConnection.send("END_REQUEST");
                    } else {
                        tcpConnection.send(string.Format("CHAT {0}", input));
                    }
                }

                if (!running) break;
            }

            listenThread.Join();
        }

    }
}
