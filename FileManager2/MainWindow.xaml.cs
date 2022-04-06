using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;

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

            byte[] packetJson = Encoding.UTF8.GetBytes(jsonObject);

            stream.Write(packetJson, 0, packetJson.Length);
        }
        private void FormClickButtonBrowse(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result != CommonFileDialogResult.Ok) return;
            path = dialog.FileName;
        }

        private void FormClickButtonDownLoad(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result != CommonFileDialogResult.Ok) return;
            path=dialog.FileName;

            string serverPath = string.Empty;

            InfoFile file = new InfoFile();
            byte[] buffer = new byte[60000];
            while(true)
            {
                int length = stream.Read(buffer, 0, buffer.Length);

                if (length < 60000) { break; }
            }

            // TODO Дописать запись в файл
        }
    }
}


