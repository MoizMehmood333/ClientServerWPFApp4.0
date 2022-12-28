using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerAsync
{
    public  class Listner
    {
        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;

        Socket listner;

        public int Port;
        public bool Running {
            get;
            private set;
        }
        public Listner() {
            Port = 0;
        }

        public void Start(int port) {
            if (Running)
            {
                return;
            }
            listner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listner.Bind(new IPEndPoint (IPAddress.Parse("127.0.0.1"), port));
            listner.Listen(0);
            listner.BeginAccept(AcceptedCallBack, null);
            Running = true;
        }
        public void Stop()
        {
            if (!Running)
            {
                return;
            }
            listner.Close();
            Running = false;

        }

        void AcceptedCallBack(IAsyncResult ar) {

            try
            {
                Socket s = listner.EndAccept(ar);
                if (SocketAccepted != null) {
                    SocketAccepted(s);
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (Running) {
                try
                {
                    listner.BeginAccept(AcceptedCallBack, null);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

    
    }
}
