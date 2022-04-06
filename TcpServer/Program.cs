using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        static List<TcpClient> clients;
        static string fileName;
        static string path = Environment.CurrentDirectory;

        static void Main(string[] args)
        {
            clients = new List<TcpClient>();
            ConnectClient();
        }
        static void ConnectClient()
        {
            TcpClient client;
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();  // работает как транзикация
                                                      // Пока не подключится клиент далше шаги не выполняются
                clients.Add(client); // Добавляет нового клиента в Лист
                Console.WriteLine("У нас новый посетитель!");
                Thread thread = new Thread(new ParameterizedThreadStart(ClientListener));
                thread.Start(client);
            }
        }
        static void ClientListener(object _client)
        {
            TcpClient client = (TcpClient)_client;
            try
            {
                while (true)
                {
                    Upload(client);

                    //    SetFileName(client);
                    //    PostFile(client);
                }
            }
            catch (Exception ex)
            {
                clients.Remove(client); //  Удаляем клиента если он не используется или ошибка
                Console.WriteLine(ex.ToString() + "\nCоединение прервано");
            }
        }
        static void Upload(TcpClient newClient)
        {
            string json = "";
            int packetLenght = 0;

            while (true)
            {
                // Получаем имя файла
                networkStream = newClient.GetStream();
                byte[] fileBuffer = new byte[60000];
                packetLenght = networkStream.Read(fileBuffer, 0, 60000);

                json += Encoding.UTF8.GetString(fileBuffer);

                if (packetLenght < fileBuffer.Length)
                {
                    break;
                }
                // TODO :: кодировка
                //Encoding cp1251 = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                //if (cp1251 == null) throw new Exception("Кодировка cp1251 = null обновите .Net");
            }
            InfoFile file = new InfoFile();
            file = JsonConvert.DeserializeObject<InfoFile>(json);
            // TODO :: Создать файл
        }

        //static void SetFileName(TcpClient newClient)
        //{
        //    fileName = string.Empty;
        //    //////////////////////////////////////////////////////////////////////////
        //    ///Получаем имя файла
        //    networkStream = newClient.GetStream();
        //    byte[] fileNameBuffer = new byte[4096];
        //    networkStream.Read(fileNameBuffer, 0, 4096);

        //    if (Encoding.Default.GetString(fileNameBuffer).Contains("<Name="))
        //    {
        //        fileName = Encoding.Default.GetString(fileNameBuffer);
        //        fileName = fileName.Remove(fileName.IndexOf('\0'));
        //        fileName = fileName.Remove(0, fileName.IndexOf('=') + 1);
        //        fileName = fileName.Remove(fileName.IndexOf('>'));
        //        Console.WriteLine(fileName);
        //    }

        //    if (fileName == string.Empty)
        //    {
        //        throw new Exception("Посетитель вышел или не отправили имя файла");
        //    }
        //    //////////////////////////////////////////////////////////////////////////
        //}
        //static void PostFile(TcpClient newClient)
        //{
        //    if (fileName == string.Empty) return;
        //    int packetLenght = 0;
        //    int fileLenght = 0;

        //    //////////////////////////////////////////////////////////////////////////
        //    ///Сохранение файла
        //    using (FileStream fileWriter = File.Create($"{path}{fileName}", 4096, FileOptions.Asynchronous))
        //    {
        //        while (true)
        //        {

        //            networkStream = newClient.GetStream();
        //            byte[] fileDataBuffer = new byte[60000];
        //            packetLenght = networkStream.Read(fileDataBuffer, 0, 60000);

        //            if (packetLenght < fileDataBuffer.Length)
        //            {
        //                fileWriter.Write(fileDataBuffer, 0, packetLenght);
        //                fileLenght += packetLenght;
        //                return;
        //            }
        //            else
        //            {
        //                fileWriter.Write(fileDataBuffer, 0, packetLenght);
        //                fileLenght += packetLenght;
        //            }
        //        }
        //    }
        //    //////////////////////////////////////////////////////////////////////////
        //}

    }
}

