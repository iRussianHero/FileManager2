using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace FileManager2
{
    public partial class MainWindow : Window
    {
        TcpClient connectionToServer;
        NetworkStream stream;
        string ip;
        int port;
        OpenFileDialog openFile;
        string path;

        public MainWindow()
        {
            InitializeComponent();
            ip = "127.0.0.1";
            port = 8888;
        }

        //ctrl shift u
        //ctrl u
        private void FormClickButtonConnection(object sender, RoutedEventArgs e)
        {
            ip = FormIp.Text;
            port = Convert.ToInt32(FormPort.Text);
            connectionToServer = new TcpClient(ip, port);
            stream = connectionToServer.GetStream();

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 255, 0);
            FormIndicator.Fill = mySolidColorBrush;
        }
        private void FormClickButtonDisconnection(object sender, RoutedEventArgs e)
        {
            FormLableLenghtFile.Content = 0;
            stream.Close();
            connectionToServer.Close();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            FormIndicator.Fill = mySolidColorBrush;
        }
        private void FormClickButtonUpload(object sender, RoutedEventArgs e)
        {
            if (path == null) return;
            InfoFile file = new InfoFile();

            ////// Инициализируем file
            int index = path.LastIndexOf('\\');
            file.Name = path.Remove(0, index + 1); // удаляет  от 0 до индекса
            file.Data = File.ReadAllBytes(path);
            file.Length = file.Data.Length;
            /////////////////////////////////////////////           

            string jsonObject = file.GetJson();

            byte[] packetJson = Encoding.Default.GetBytes(jsonObject);

            stream.Write(packetJson, 0, file.Length);            
        }
        private void FormClickButtonBrowse(object sender, RoutedEventArgs e)
        {
            openFile = new OpenFileDialog();
            openFile.ShowDialog();
            path = openFile.FileName;
            // fileLenght = path.Length;
        }
    }
}


