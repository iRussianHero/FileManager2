using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
namespace FileManager2
{
    public partial class MainWindow : Window
    {
        TcpClient connectionToServer;
        NetworkStream stream;
        string ip = "127.0.0.1";
        int port = 8888;
        OpenFileDialog openFile;
        string path;
        int fileLenght;

        public MainWindow()
        {
            InitializeComponent();
            //Thread thread = new Thread(new ParameterizedThreadStart(Listener));
            //thread.Start(connectionToServer);
        }
        //private void Listener(object client)
        //{
        //    connectionToServer = client as TcpClient;
        //    while (true)
        //    {
        //        stream = connectionToServer.GetStream();
        //        byte[] buffer = new byte[64000];
        //        stream.Read(buffer, 0, buffer.Length);
        //        string message = buffer.ToString();
        //        MessageBox.Show(message);
        //    }
        //}
        private void FORM_CLICK_BUTTON_CONNECTION(object sender, RoutedEventArgs e)
        {
            ip = FORM_IP.Text;
            port = Convert.ToInt32(FORM_PORT.Text);
            connectionToServer = new TcpClient(ip, port);
            stream = connectionToServer.GetStream();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 255, 0);
            FORM_INDICATOR.Fill = mySolidColorBrush;
        }
        private void FORM_CLICK_BUTTON_UPLOAD(object sender, RoutedEventArgs e)
        {
            int i = path.LastIndexOf('\\');
            string name = "<Name>" + path.Remove(0, i + 1);
            byte[] fileName = Encoding.Default.GetBytes(name);
            stream.Write(fileName, 0, fileName.Length);


            byte[] buffer = File.ReadAllBytes(path);
            FORM_LABLE_LENGHT_FILE.Content = buffer.Length; // вывод размера в байтах в окно приложения
            stream.Write(buffer, 0, buffer.Length);
        }
        private void FORM_CLICK_BUTTON_DISCONNECTION(object sender, RoutedEventArgs e)
        {
            FORM_LABLE_LENGHT_FILE.Content = 0;
            stream.Close();
            connectionToServer.Close();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            FORM_INDICATOR.Fill = mySolidColorBrush;
        }

        private void FORM_CLICK_BUTTON_BROWSE(object sender, RoutedEventArgs e)
        {
            openFile = new OpenFileDialog();
            openFile.ShowDialog();
            path = openFile.FileName;
            fileLenght = path.Length;
        }
    }
}
//public void Sender()
//{
//    string path = "C:\\Users\\vyshk\\Desktop\\delete\\slack.exe";
//    byte[] buffer;
//    using (FileStream fileStream = File.OpenRead(path))
//    {
//        buffer = new byte[fileStream.Length];
//        fileStream.Read(buffer, 0, buffer.Length);
//    }
//    FileLenght.Content = buffer.Length; // вывод размера в байтах в окно приложения
//    stream.Write(buffer, 0, buffer.Length);
//    stream.Flush();
//}

//public void Sender()
//{
//    string path = "C:\\Users\\vyshk\\Desktop\\delete\\slack.exe";
//    byte[] buffer;
//    using (FileStream fileStream = File.OpenRead(path))
//    {
//        buffer = new byte[fileStream.Length];
//        fileStream.Read(buffer, 0, buffer.Length);
//    }
//    FileLenght.Content = buffer.Length; // вывод размера в байтах в окно приложения
//    stream.Write(buffer, 0, buffer.Length);
//    stream.Flush();
//}


