/*Encryption class for CS 4390, Dr. Le's class. This encryption program runs MD5 and Triple DES using a hashing algorithm per
 * requested. This is for use in the Server Project for Team 2*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Encryptey
{
    class Program
    {
        //we have to set a key or password as great as our professor
        static string password { get; set; } = "ProF3sS0RLeIsAw3S0MEInCS4390";

        static void Main(string[] args)
        {
            //here goes the message being sent from server to client or vice versa
            var messageSent = "This is the message being sent";

            //this line can be uncommented to view the cryption in console
           Console.WriteLine(messageSent);

            //here the message will go through the encryption process
            var encryptedMessage = Encrypt(messageSent);

            //this line can be uncommented to view the cryption in console
            Console.WriteLine(encryptedMessage);

            //finally here the messsage is decrypted
            messageSent = Decrypt(encryptedMessage);

            //this line can be uncommented to view the cryption in console
            Console.WriteLine(messageSent);

            Console.ReadKey();
        }

        //here is the encryption class
        public static string Encrypt(string messageSent)
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

        //here lies the decryptor we will use for the encrypted message sent
        public static string Decrypt(string encryptedMessage)
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