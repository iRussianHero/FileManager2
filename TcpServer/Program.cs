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
        static string path = Environment.CurrentDirectory;
        static string fileName = "Test.txt";

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
                InfoFile file = new InfoFile();
                byte[] buffer = new byte[1000];
                string json = string.Empty;

                while (true)
                {
                    networkStream = client.GetStream(); // подключаемся к клиенту
                    networkStream.Read(buffer, 0, 1000); // ждем пакет от клиента
                    json = Encoding.UTF8.GetString(buffer);
                    file = JsonConvert.DeserializeObject<InfoFile>(json);

                    if (file.Method == "Upload")
                    {
                        Upload(client);
                    }

                    if (file.Method == "Download")
                    {
                        Download(client);
                    }
                }
            }
            catch (Exception ex)
            {
                clients.Remove(client); //  Удаляем клиента если он не используется или ошибка
                Console.WriteLine(ex.Message);
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
            }
            InfoFile file = new InfoFile();
            file = JsonConvert.DeserializeObject<InfoFile>(json);
            if (file == null)
            {
                throw new Exception("Клиент отключился...");
            }
            File.WriteAllBytes(path + "\\" + file.Name, file.Data);
        }

        static void Download(TcpClient client)
        {
            InfoFile file = new InfoFile();

            string pathFile = path + "\\" + fileName;

            ////// Инициализируем file
            int index = pathFile.LastIndexOf('\\');
            file.Name = pathFile.Remove(0, index + 1);
            file.Data = File.ReadAllBytes(pathFile);
            file.Length = file.Data.Length;
            /////////////////////////////////////////////           

            string jsonObject = file.GetJson();
            byte[] packetJson = Encoding.UTF8.GetBytes(jsonObject);

            networkStream = client.GetStream();
            networkStream.Write(packetJson, 0, packetJson.Length);
            networkStream.Flush();           
        }

    }
}

