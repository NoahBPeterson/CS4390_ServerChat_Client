using System;
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

        public string privateKeyCipher;


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
            serverChallenge = Encoding.UTF8.GetBytes(response);
            int randomChallenge;
            Int32.TryParse(response, out randomChallenge);
            string clientResponse = Encoding.UTF8.GetString(challengeReponse(randomChallenge)); //Get response, convert from byte[] to string
            UDPSend(clientID +" "+ clientResponse); //Send clientID + challenge Response
            privateKeyCipher = clientResponse;

            string tcp = UDPReceive(); //Receive rand_cookie, port_number for TCP connection
            Console.WriteLine("\"" + tcp + "\"");
            tcp = Decrypt(tcp, (randomChallenge.ToString() + privateKey));
            if (tcp.Equals("FAIL"))
            {
                Console.WriteLine("Authentication failed.");
            }
            udpConnectionSocket.Close();  //Close socket when done.
            return tcp;
        }

        public byte[] challengeReponse(int rand)
        {
            SHA256 encryptionObject = SHA256.Create();
            byte[] randKey = Encoding.UTF8.GetBytes(rand.ToString() + privateKey);
            byte[] randHash = encryptionObject.ComputeHash(randKey); //Hashes challenge with out privateKey (password)
            return randHash;
        }

        //Call this function with string you wish to send to the server (Client ID, challenge response, etc.
        
        public void UDPSend(String sendText)
        {
            try
            {
                //Send message to server using UDP.
                byte[] buffer = Encoding.UTF8.GetBytes(sendText);
                EndPoint hostEndPoint = (EndPoint)serverAddress;
                udpConnectionSocket.SendTo(buffer, buffer.Length, SocketFlags.None, hostEndPoint);
                //udpConnectionSocket.SendTo(buffer, hostEndPoint);

            }
            catch (SocketException e) { }
            catch (ArgumentNullException e) { }
            catch (Exception e) { }
        }

        public static int[] udpParse(string response)
        {
            string cookie = "";
            string port = "";
            int space = 0;
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i] == ' ')
                {
                    space = i;
                    break;
                }
                else
                {
                    cookie += response.Substring(i, 1);
                }
            }
            port = response.Substring(space, response.Length - space);
            int[] cookiePort = { Int32.Parse(cookie), Int32.Parse(port) };
            return cookiePort;
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
                receiveString += Encoding.UTF8.GetString(receiveBytes);
                receiveString = receiveString.Substring(0, receive);
                return receiveString;
            }
            catch (Exception e) { }

            return "";

        }
        public string Encrypt(string messageSent, string password)
        {
            //this will use MD5 as well as Triple DES
            using (var CryptoMD5 = new MD5CryptoServiceProvider())
            {
                using (var TripleDES = new TripleDESCryptoServiceProvider())
                {
                    //hashing and encryption begins here based on the password above
                    TripleDES.Key = CryptoMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(password));
                    TripleDES.Mode = CipherMode.ECB;
                    TripleDES.Padding = PaddingMode.PKCS7;

                    //creates an encryption from the library
                    using (var crypt = TripleDES.CreateEncryptor())
                    {
                        //actual cryption goes here and accomodates for the length of the given string
                        byte[] messageBytes = UTF8Encoding.UTF8.GetBytes(messageSent);
                        byte[] totalBytes = crypt.TransformFinalBlock(messageBytes, 0, messageBytes.Length);
                        return Convert.ToBase64String(totalBytes, 0, totalBytes.Length);
                    }
                }
            }
        }

        public string Decrypt(string encryptedMessage, string password)
        {
            //once again this will be based on MD5 and Triple DES
            using (var CryptoMD5 = new MD5CryptoServiceProvider())
            {
                //creating an instance for tripleDES
                using (var TripleDES = new TripleDESCryptoServiceProvider())
                {
                    //here the password is read and cyphered
                    TripleDES.Key = CryptoMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(password));
                    TripleDES.Mode = CipherMode.ECB;
                    TripleDES.Padding = PaddingMode.PKCS7;

                    //here the decrytion occurs as we create an instance to begin decrypting
                    using (var crypt = TripleDES.CreateDecryptor())
                    {
                        //here the magic happens as we decrypt what was sent and allow us to get the original message
                        byte[] cipherBytes = Convert.FromBase64String(encryptedMessage);
                        byte[] totalBytes = crypt.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(totalBytes);
                    }
                }
            }
        }


    }
}