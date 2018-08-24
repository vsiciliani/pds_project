using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            while (!stopThread)
            {
                Console.WriteLine("TEST THREAD");
                Thread.Sleep(8000);
            }
        }

    }
}
