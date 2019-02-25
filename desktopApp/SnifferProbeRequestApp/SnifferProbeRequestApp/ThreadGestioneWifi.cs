using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

//Classe Singleton che wrappa il thread per la gestione del Wifi e dell'interfaccia verso le ESP

namespace SnifferProbeRequestApp
{
    class ThreadGestioneWifi
    {
        private volatile bool stopThreadElaboration;
        private ThreadStart delegateThreadElaboration;
        private Thread threadElaboration;

        private static ThreadGestioneWifi istance = null;
        private Socket listener = null;
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private DatabaseManager dbManager = null;

        private ThreadGestioneWifi()
        {
            stopThreadElaboration = false;
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
            //instanzio il db manager
            dbManager = DatabaseManager.getIstance();

            delegateThreadElaboration = new ThreadStart(elaboration);
            threadElaboration = new Thread(delegateThreadElaboration);
            threadElaboration.IsBackground = true;
            threadElaboration.Start();

             
        }

        public void stop()
        {

            stopThreadElaboration = true;
            dbManager.closeConnection();
            //if (listener != null) listener.Stop();
            threadElaboration.Join();
            
        }

        private void elaboration()
        {
            //TODO:eliminare
            /*
            PacketsInfo packetsInfo = new PacketsInfo();
            PacketInfo packet1 = new PacketInfo();
            packet1.hashCode = "10";
            packet1.signalStrength = 43;
            packet1.sourceAddress = "127.45.32.9";
            packet1.SSID = "pippo";
            packet1.timestamp = 32423;

            PacketInfo packet2 = new PacketInfo();
            packet2.hashCode = "10";
            packet2.signalStrength = 34;
            packet2.sourceAddress = "127.45.32.9";
            packet2.SSID = "pippo";
            packet2.timestamp = 32423;

            List<PacketInfo> list = new List<PacketInfo>();
            list.Add(packet1);
            list.Add(packet2);

            packetsInfo.listPacketInfo = list;

            dbManager.saveReceivedData(packetsInfo, IPAddress.Any);
            */
            //TODO: decommentare se non lavoro su PC aziendale
            //startHotspot("prova4", "pippopluto");
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket started");
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.Any, 5010));
                listener.Listen(10);

                while (true)
                {
                    allDone.Reset();
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(acceptCallback), listener);

                    allDone.WaitOne();
                }

            }
            catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante il binding del socket", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
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

            IPEndPoint remoteIpEndPoint = null;
            try {
                remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore nel riconoscere il socket remoto", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }
            
            allDone.Set();

            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "CONNECTED with device: " + remoteIpEndPoint.Address.ToString());
            
            while (!stopThreadElaboration)
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

                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "MESSAGE FROM CLIENT " + remoteIpEndPoint.Address.ToString()
                    + ": " + receivedMessage);
                PacketsInfo packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(receivedMessage);

                //salvo i dati nella tabella raw del DB
                dbManager.saveReceivedData(packetsInfo, remoteIpEndPoint.Address);
                
                /*
                foreach (var packet in packetsInfo.listPacketInfo)
                {
                    Console.WriteLine("Device: {5} -- SSID: {0}, sourceAddress: {1}, signalStrength: {2}, , hashCode: {3}, timestamp: {4}", packet.SSID, packet.sourceAddress, packet.signalStrength, packet.hashCode, packet.timestamp, remoteIpEndPoint.Address);
                }
                */
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

