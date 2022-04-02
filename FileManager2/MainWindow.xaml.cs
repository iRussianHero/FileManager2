using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace FileManager2
{
    public partial class MainWindow : Window
    {
        TcpClient connectionToServer;
        NetworkStream stream;
        string ip = "127.0.0.1";
        int port = 8888;
        public MainWindow()
        {
            InitializeComponent();
            connectionToServer = new TcpClient(ip, port);
            stream = connectionToServer.GetStream();

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
        public void Sender() // тоже самое что и Sender2()
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

            stream.Write(buffer, 0, buffer.Length);

            string path = "C:\\Users\\boxbo\\Desktop\\111\\SQL.jpg";

            byte[] buffer = File.ReadAllBytes(path);

            FileLenght.Content = buffer.Length; // вывод размера в байтах в окно приложения

            stream.Write(buffer, 0, buffer.Length);
        }


        private void FORM_CLICK_BUTTON_CONNECTION(object sender, RoutedEventArgs e)
        {
            Sender();
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


