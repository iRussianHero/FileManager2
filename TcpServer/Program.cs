using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServer
{
    public class Program
    {
        static TcpListener listener;
        static NetworkStream networkStream;
        static TcpClient client;
        static string fileName;
        static string path = Environment.CurrentDirectory;

        static void Main(string[] args)
        {
            ConnectClient();
        }

        static void ClientListener()
        {          
            try
            {
                while (true)
                {
                    SetFileName(client);
                    PostFile(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\nCоединение прервано");
            }
        }

        static void SetFileName(TcpClient newClient)
        {
            fileName = string.Empty;
            //////////////////////////////////////////////////////////////////////////
            ///Получаем имя файла
            networkStream = newClient.GetStream();
            byte[] fileNameBuffer = new byte[4096];
            networkStream.Read(fileNameBuffer, 0, 4096);

            if (Encoding.Default.GetString(fileNameBuffer).Contains("<Name="))
            {
                //NetworkToHostOrder
                fileName = Encoding.Default.GetString(fileNameBuffer);
                fileName = fileName.Remove(fileName.IndexOf('\0'));
                fileName = fileName.Remove(0, fileName.IndexOf('=') + 1);
                fileName = fileName.Remove(fileName.IndexOf('>'));
                Console.WriteLine(fileName);
            }

            if (fileName == string.Empty)
            {
                throw new Exception("Посетитель вышел или не отправили имя файла");
            }
            //////////////////////////////////////////////////////////////////////////
        }
        static void PostFile(TcpClient newClient)
        {
            if (fileName == string.Empty) return;
            int packetLenght = 0;
            int fileLenght = 0;

            //////////////////////////////////////////////////////////////////////////
            ///Сохранение файла
            using (FileStream fileWriter = File.Create($"{path}{fileName}", 4096, FileOptions.Asynchronous))
            {
                while (true)
                {

                    networkStream = newClient.GetStream();
                    byte[] fileDataBuffer = new byte[60000];
                    packetLenght = networkStream.Read(fileDataBuffer, 0, 60000);

                    //if (packetLenght == '\0') // если клиент дисконектится то ретерним
                    //{
                    //    Console.WriteLine($"Файл \"{fileName}\" сохранен, размер = {fileLenght}");
                    //    return;
                    //}

                    if (packetLenght < fileDataBuffer.Length)
                    {
                        fileWriter.Write(fileDataBuffer, 0, packetLenght);
                        fileLenght += packetLenght;
                        return;
                    }
                    else
                    {
                        fileWriter.Write(fileDataBuffer, 0, packetLenght);
                        fileLenght += packetLenght;
                    }
                }
            }
            //////////////////////////////////////////////////////////////////////////
        }

        static void ConnectClient()
        {            
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();  // работает как транзикация              
                Console.WriteLine("У нас новый посетитель!");
                Thread thread = new Thread(ClientListener);                
                thread.Start();
            }
        }
    }
}

