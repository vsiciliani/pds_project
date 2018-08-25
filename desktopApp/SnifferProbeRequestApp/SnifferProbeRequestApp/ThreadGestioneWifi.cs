using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

//Classe Singleton che wrappa il thread per la gestione del Wifi e dell'interfaccia verso le ESP

namespace SnifferProbeRequestApp
{
    class ThreadGestioneWifi
    {
        private volatile bool stopThread;
        private ThreadStart delegateThread;
        private Thread thread;
        private static ThreadGestioneWifi istance=null;

        private ThreadGestioneWifi() {
            stopThread = false;
            start();
        }

        static public ThreadGestioneWifi getIstance() {
            if (istance == null) {
                istance = new ThreadGestioneWifi();
            }
            return istance;
        }

        public void start() {
            delegateThread = new ThreadStart(elaboration);
            thread = new Thread(delegateThread);
            thread.Start();
        }

        public void stop() {
            stopThread = true;
            thread.Join();
        }

        private void elaboration() {
            hotspot("Rete di prova", "pippopluto", true);
            while (!stopThread)
            {
                Thread.Sleep(8000);
            }
        }

        private void hotspot(string ssid, string key, bool status)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                if (status)
                {
                    process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + key);
                    process.StandardInput.WriteLine("netsh wlan start hosted network");
                    process.StandardInput.Close();
                }
                else
                {
                    process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
                    process.StandardInput.Close();
                }
            }
        }

    }
}
