using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace ClientAsync
{
    enum Commands
    {
        String = 0,
        Image = 1
    }
    public class Client
    {
        public delegate void OnConnectEventHandler(Client sender, bool Connected);
        public event OnConnectEventHandler OnConnect;

        public delegate void OnSendEventHandler(Client sender, int sent);
        public event OnSendEventHandler OnSend;

        public delegate void OnDisconnectEventHandler(Client sender);
        public event OnDisconnectEventHandler OnDisconnect;

        Socket socket;
        public bool Connected
        {
            get {
                if (socket != null) {
                    return socket.Connected;
                }
                return false;
            }

        }
        public Client() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }
        public void Connect(string ipAddress, int nPort) {
            if (socket == null) {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            }
            socket.BeginConnect(ipAddress, nPort, ConnectCallBack, null);
        }

        void ConnectCallBack(IAsyncResult ar) {
            try
            {
                socket.EndConnect(ar);
                if (OnConnect != null) {
                    OnConnect(this, Connected);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        public void Send(byte[] data, int index, int length)
        {
            socket.BeginSend(BitConverter.GetBytes(length), 0, 4, SocketFlags.None, sendCallBack, null);
            socket.BeginSend(data, index, length, SocketFlags.None, sendCallBack, null);

        }

        void sendCallBack(IAsyncResult ar) {
            try
            {
                int sent = socket.EndSend(ar);
                if (OnSend != null) {
                    OnSend(this, sent);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error\n", ex.Message);
                
            }
        }

        public void Disconnect() {

            try
            {
                if (socket.Connected) {
                    socket.Close();
                    socket = null;
                    if (OnDisconnect != null) {
                        OnDisconnect(this);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    

    }

}
