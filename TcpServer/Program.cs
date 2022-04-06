using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServer
{
    internal class Program
    {
        static TcpClient client;
        static TcpListener listener;
        static NetworkStream networkStream;

        static void Main(string[] args)
        {
            client = new TcpClient();
            ConnectClient();
        }

        static void ClientListener(object _newClient)
        {
            int lenght = 0;

            using (FileStream writer = File.Create("C:\\Users\\vyshk\\Desktop\\delete\\slackCopy.exe", 4096, FileOptions.Asynchronous))
            {
                while (true)
                {
                    try
                    {
                        networkStream = ((TcpClient)_newClient).GetStream();
                        byte[] buffer = new byte[4096];
                        networkStream.Read(buffer, 0, 4096);
                        lenght += buffer.Length;
                        writer.Write(buffer);
                    }
                    catch
                    {
                        networkStream.Close();
                        Console.WriteLine(lenght);
                        break;
                    }
                    networkStream.Flush();
                }
            }
        }

        static void ConnectClient()
        {
            TcpClient client;
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();  // работает как транзикация              
                Console.WriteLine("У нас новый посетитель!");
                Thread thread = new Thread(new ParameterizedThreadStart(ClientListener));
                thread.Start(client);
            }
        }
    }
}
