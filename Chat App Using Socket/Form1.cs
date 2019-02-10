﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Chat_App_Using_Socket
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

            foreach(IPAddress address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverIPtextBox.Text = address.ToString();
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(serverPorttextBox.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(clientIPtextBox.Text.ToString()), int.Parse(clientPorttextBox.Text.ToString()));

            try
            {
                client.Connect(IpEnd);
                if (client.Connected)
                {
                    chatScreentextBox.AppendText("Connected to server " + "\n");

                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            }catch(Exception ex){ }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.chatScreentextBox.Invoke(new MethodInvoker(delegate ()
                    {
                        chatScreentextBox.AppendText("\nYou: " + recieve);
                    }));

                    recieve = "";
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.chatScreentextBox.Invoke(new MethodInvoker(delegate ()
                {
                    chatScreentextBox.AppendText("\nMe: " + TextToSend);
                }));
            }
            else
            {
                MessageBox.Show("Sending failed");
            }

            backgroundWorker2.CancelAsync();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if(messagetextBox.Text != "")
            {
                TextToSend = messagetextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }

            messagetextBox.Text = "";
        }
    }
}
