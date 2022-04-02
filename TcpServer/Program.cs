using System;
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
            int packetLenght = 0;
            int fileLenght = 0;
            string fileName = string.Empty;

            //////////////////////////////////////////////////////////////////////////
            ///Получаем имя файла
            networkStream = ((TcpClient)_newClient).GetStream();
            byte[] fileNameBuffer = new byte[4096];
            packetLenght = networkStream.Read(fileNameBuffer, 0, 4096);

            if (Encoding.Default.GetString(fileNameBuffer).Contains("<Name>"))
            {
                fileName = Encoding.Default.GetString(fileNameBuffer);
                fileName = fileName.Remove(fileName.IndexOf('\0'));
                fileName = fileName.Remove(0, fileName.IndexOf('>') + 1);
            }
            //////////////////////////////////////////////////////////////////////////

            using (FileStream writer = File.Create($"C:\\Users\\vyshk\\Desktop\\delete\\{fileName}", 4096, FileOptions.Asynchronous))
            {
                while (true)
                {
                    try
                    {
                        networkStream = ((TcpClient)_newClient).GetStream();
                        byte[] buffer = new byte[4096];
                        packetLenght = networkStream.Read(buffer, 0, 4096);

                        if (Encoding.Default.GetString(buffer).Contains("<Name>"))
                        {
                            fileName = Encoding.Default.GetString(buffer);
                        }

                        if (packetLenght == '\0')
                        {
                            Console.WriteLine($"Файл \"{fileName}\" , размер = {fileLenght}");
                            if (packetLenght == '\0')
                                break;
                        }
                        writer.Write(buffer);
                        fileLenght += packetLenght;
                    }
                    catch
                    {
                        networkStream.Close();
                        Console.WriteLine(fileLenght);
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
