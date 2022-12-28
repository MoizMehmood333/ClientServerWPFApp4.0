using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ClientAsync;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace ClientAsync
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client;
        public MainWindow()
        {
            InitializeComponent();
            btnDisconnect.Click += BtnDisconnect_Click;
            btnSendImage.Click += BtnSendImage_Click;
            btnConnect.Click += BtnConnect_Click;
            btnSendText.Click += BtnSendText_Click;
            client = new Client();
            client.OnConnect += Client_OnConnect;
            client.OnSend += Client_OnSend;
            client.OnDisconnect += Client_OnDisconnect;
        }

        private void Client_OnDisconnect(Client sender)
        {
            MessageBox.Show("Disconnected");
        }

        private void Client_OnSend(Client sender, int sent)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                txtSentOn.Text = string.Format("Data Sent: {0}", sent);
            });
        }

        private void Client_OnConnect(Client sender, bool Connected)
        {
            if(Connected)
                MessageBox.Show("Connection Accepted");
        }

        private void BtnSendText_Click(object sender, RoutedEventArgs e)
        {
            SendText(txtToSend.Text);
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!client.Connected) {
                client.Connect("127.0.0.1", 4000);
            }
        }

        private void BtnSendImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                SendImage(fileName);
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
        }

        void SendText(string Text) {
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)Commands.String);
            bw.Write(Text);
            bw.Close();

            byte[] data = ((MemoryStream)bw.BaseStream).ToArray();
            bw.BaseStream.Dispose();
            client.Send(data, 0, data.Length);
            data = null;
        }
        void SendImage(string path) {
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write((int)Commands.Image);
            bw.Write(path);
            bw.Close();
            
            byte[] data = ((MemoryStream)bw.BaseStream).ToArray();
            bw.BaseStream.Dispose();
            client.Send(data, 0, data.Length);
            data = null;
        }
    }
}
