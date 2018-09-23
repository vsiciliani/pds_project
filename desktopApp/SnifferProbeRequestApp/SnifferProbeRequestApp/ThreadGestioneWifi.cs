using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

//Classe Singleton che wrappa il thread per la gestione del Wifi e dell'interfaccia verso le ESP

namespace SnifferProbeRequestApp
{
    class ThreadGestioneWifi
    {
        private volatile bool stopThread;
        private ThreadStart delegateThread;
        private Thread thread;
        private static ThreadGestioneWifi istance = null;
        private Socket listener = null;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        private ThreadGestioneWifi()
        {
            stopThread = false;
            start();
        }

        static public ThreadGestioneWifi getIstance()
        {
            if (istance == null)
            {
                istance = new ThreadGestioneWifi();
            }
            return istance;
        }

        public void start()
        {
            delegateThread = new ThreadStart(elaboration);
            thread = new Thread(delegateThread);
            thread.IsBackground = true;
            thread.Start();
        }

        public void stop()
        {
            stopThread = true;
            //if (listener != null) listener.Stop();
            thread.Join();
        }

        private void elaboration()
        {
            //TODO: decommentare se non lavoro su PC aziendale
            //startHotspot("prova4", "pippopluto");
            Console.WriteLine("SOCKET STARTED");
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.Any, 5010));
                listener.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(acceptCallback), listener);

                    allDone.WaitOne();
                }

                //Socket socket = listener.Accept();
                
                
            }
            catch (Exception)
            {

                throw;
            }
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Close();
            //stopHotspot();
        }

        public void acceptCallback(IAsyncResult ar)
        {
            int MAXBUFFER = 4096;

            Socket listener = (Socket)ar.AsyncState;
            Socket socket = listener.EndAccept(ar);

            allDone.Set();
            
            Console.WriteLine("CONNECTED");

            while (!stopThread)
            {
                string receivedMessage = string.Empty;
                while (true)
                {
                    byte[] receivedBytes = new byte[MAXBUFFER];
                    int numBytes = socket.Receive(receivedBytes);
                    receivedMessage += Encoding.ASCII.GetString(receivedBytes, 0, numBytes);
                    if (receivedMessage.IndexOf("\n") > -1)
                    {
                        break;
                    }
                }
                Console.WriteLine("MESSAGE FROM CLIENT: {0}", receivedMessage);
                PacketsInfo packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(receivedMessage);
                foreach (var packet in packetsInfo.listPacketInfo)
                {
                    Console.WriteLine("SSID: {0}, sourceAddress: {1}, signalStrength: {2}", packet.SSID, packet.sourceAddress, packet.signalStrength);
                }
            }
        }

        private void startHotspot(string ssid, string key)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + key);
                process.StandardInput.WriteLine("netsh wlan start hosted network");
                process.StandardInput.Close();
                Console.WriteLine("Wifi network started");
            }
        }

        private void stopHotspot()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);

            process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
            process.StandardInput.Close();
            Console.WriteLine("Wifi network closed");
        }

    }
}

