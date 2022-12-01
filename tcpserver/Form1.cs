using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace tcpserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private ArrayList alSockets;
        private void Form1_Load(object sender, EventArgs e)
        {
            //IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            Text = IPHost.AddressList[0].ToString();
            lblStatus.Text = "My IP address is " + IPHost.AddressList[0].ToString();
            alSockets = new ArrayList();
            Thread thdListener = new Thread(new
            ThreadStart(listenerThread));
            thdListener.Start();
        }

        public void listenerThread()
        {

            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            //TcpListener tcpListener = new TcpListener(8080);
            TcpListener tcpListener = new TcpListener(IPHost.AddressList[0], 8080);
            tcpListener.Start();
            while (true)
            {
                Socket handlerSocket = tcpListener.AcceptSocket();
                if (handlerSocket.Connected)
                {
                    lbConnections.Items.Add(
                    handlerSocket.RemoteEndPoint.ToString() + " connected."
                    );
                    lock (this)
                    {
                        alSockets.Add(handlerSocket);
                    }
                    ThreadStart thdstHandler = new ThreadStart(handlerThread);
                    Thread thdHandler = new Thread(thdstHandler);
                    thdHandler.Start();
                }
            }
        }


        public void handlerThread()
        {
            Socket handlerSocket = (Socket)alSockets[alSockets.Count - 1];
            NetworkStream networkStream = new
            NetworkStream(handlerSocket);
            int thisRead = 0;
            int blockSize = 1024;
            Byte[] dataByte = new Byte[blockSize];
            lock (this)
            {
                // Only one process can access
                // the same file at any given time
                Stream fileStream = File.OpenWrite("c:\\my documents\\upload.txt");
                while (true)
                {
                    thisRead = networkStream.Read(dataByte, 0, blockSize);
                    fileStream.Write(dataByte, 0, thisRead);
                    if (thisRead == 0) break;
                }
                fileStream.Close();
            }
            lbConnections.Items.Add("File Written");
            handlerSocket = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int bytesReceived = 0;
            byte[] recv = new byte[1];
            Socket clientSocket;
            Socket listenerSocket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
            );
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            //TcpListener tcpListener = new TcpListener(8080);
            TcpListener tcpListener = new TcpListener(IPHost.AddressList[0], 8080);
            //IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            IPEndPoint ipepServer = new
            IPEndPoint(IPHost.AddressList[0], 8080);
            listenerSocket.Bind(ipepServer);
            listenerSocket.Listen(-1);
            clientSocket = listenerSocket.Accept();
            if (clientSocket.Connected)
            {
                do
                {
                    bytesReceived = clientSocket.Receive(recv);
                    tbStatus.Text += Encoding.ASCII.GetString(recv);
                }

                while (bytesReceived != 0);
            }
        }

        
        
        



    }
}
