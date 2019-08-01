using System;
using System.IO;

namespace ChatHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            //the way this works is if the username is integrated as part of the string, and then each string is put into the array
            //see the example below
            string Username = "noahb";
            string otherUser = "Dr. Le";
            string[] messageArray = new string[] { Username + ": " + "Hello there!", otherUser + ": " + "Hello indeed!" };
            //the above assumes that is the end of the chat and all messages were exchanged
            //this code only works assuming all of the messages are already in the array
            //this will save the most recent chat and allow you to look at the history using a file reader below.

            //creates new messages.txt will work on any machine
            using (StreamWriter sw = new StreamWriter("messages.txt"))
            {
                //stores the arrays into the stream
                foreach (string s in messageArray)
                {
                    sw.WriteLine(s);
                }
            }

            // Read and show each line from the file.
            string line = "";
            using (StreamReader sr = new StreamReader("messages.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            Console.ReadKey();
        }
    }
}