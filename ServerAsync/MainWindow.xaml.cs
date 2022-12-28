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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace ServerAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Listner listner;
        Client client;
        public MainWindow()
        {
            InitializeComponent();
            btnListen.Click += BtnListen_Click;
            btnClose.Click += BtnClose_Click;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null) {
                client.Close();
            }
            if (listner != null && listner.Running) {
                listner.Stop();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (client != null) {
                client.Close();
                client = null;
            }
            listner.Stop();

            txtCLientConneted.Text = "Connected: Null";
            txtMessage.Document.Blocks.Clear();
            imgSection.Source = null;

        }

        private void BtnListen_Click(object sender, RoutedEventArgs e)
        {
            listner = new Listner();
            listner.SocketAccepted += Listner_SocketAccepted;
            listner.Start(4000);

        }

        private void Listner_SocketAccepted(Socket e)
        {
            if (client != null)
            {
                e.Close();
                return;
            }
            client = new Client(e);
            client.DataReceived += Client_DataReceived;
            client.Disconnected += Client_Disconnected;
            client.ReceiveAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtCLientConneted.Text = $"Client Connected: {client.EndPoint.ToString()}"; 
           
            });
        }

        private void Client_Disconnected(Client sender)
        {
            client.Close();
            client = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtCLientConneted.Text = $"Client Connected: null";
                MessageBoxResult res = MessageBox.Show("Client Disconnected\nClear Data?", " ", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    //txtMessage.Document.Blocks.Add(new Paragraph(new Run(Encoding.ASCII.GetString(data))));
                    txtMessage.Document.Blocks.Clear();
                    imgSection.Source = null;
                }
            
            });
        }

        private void Client_DataReceived(Client sender, ReceiveBuffer e)
        {
            BinaryReader r = new BinaryReader(e.BufStream);
            Commands header = (Commands)r.ReadInt32();
            switch (header)
            {
                case Commands.String:
                    string s = r.ReadString();
                    //check access method if not null 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        txtMessage.Document.Blocks.Add(new Paragraph(new Run(s)));

                    });
                    break;
                case Commands.Image:
                    string imgSrc = r.ReadString();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        imgSection.Source = new BitmapImage(new Uri(imgSrc));
                    });
                    break;
                
            }
        }
    }
}
