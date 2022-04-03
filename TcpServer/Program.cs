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
        static TcpListener listener;
        static NetworkStream networkStream;

        static void Main(string[] args)
        {
            ConnectClient();
        }

        static void ClientListener(object _newClient)
        {
            TcpClient newClient = ((TcpClient)_newClient);
            PostFile(newClient);
        }

        static public void PostFile(TcpClient newClient)
        {
            int packetLenght = 0;
            int fileLenght = 0;
            string fileName = string.Empty;

            try
            {
                //////////////////////////////////////////////////////////////////////////
                ///Получаем имя файла
                networkStream = newClient.GetStream();
                byte[] fileNameBuffer = new byte[4096];
                packetLenght = networkStream.Read(fileNameBuffer, 0, 4096);

                if (Encoding.Default.GetString(fileNameBuffer).Contains("<Name="))
                {
                    fileName = Encoding.Default.GetString(fileNameBuffer);
                    fileName = fileName.Remove(fileName.IndexOf('\0'));
                    fileName = fileName.Remove(0, fileName.IndexOf('=') + 1);
                    fileName = fileName.Remove(fileName.IndexOf('>'));
                    Console.WriteLine(fileName);
                }
                //////////////////////////////////////////////////////////////////////////

                if (fileName == string.Empty)
                {
                    Console.WriteLine("Посетитель вышел или не отправили имя файла");
                    return;
                }
                //////////////////////////////////////////////////////////////////////////
                ///Сохранение файла
                using (FileStream fileWriter = File.Create($"C:\\Users\\vyshk\\Desktop\\delete\\{fileName}", 4096, FileOptions.Asynchronous))
                {
                    while (true)
                    {

                        networkStream = newClient.GetStream();
                        byte[] fileDataBuffer = new byte[60000];
                        packetLenght = networkStream.Read(fileDataBuffer, 0, 60000);

                        if (packetLenght == '\0')// если клиент дисконектится то ретерним
                        {
                            Console.WriteLine($"Файл \"{fileName}\" сохранен, размер = {fileLenght}");
                            return;
                        }

                        //Console.WriteLine($"Файл \"{fileName}\" , размер = {fileLenght}");
                        fileWriter.Write(fileDataBuffer, 0, packetLenght);
                        fileLenght += packetLenght;
                    }
                }
                //////////////////////////////////////////////////////////////////////////
            }
            catch
            {
                networkStream.Close();
                Console.WriteLine($"Exception");
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
