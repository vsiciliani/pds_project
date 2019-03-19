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

        private ThreadGestioneWifi() {
            stopThreadElaboration = false;
            start();
        }

        static public ThreadGestioneWifi getIstance() {
            if (istance == null) {
                istance = new ThreadGestioneWifi();
            }
            return istance;
        }

        public void start() {
            //instanzio il db manager
            dbManager = DatabaseManager.getInstance();

            //starto il thread in background che gestisce le connessioni con i devices
            delegateThreadElaboration = new ThreadStart(elaboration);
            threadElaboration = new Thread(delegateThreadElaboration);
            threadElaboration.IsBackground = true;
            threadElaboration.Start();   
        }

        public void stop() {
            stopThreadElaboration = true;
            dbManager.closeConnection();
            //if (listener != null) listener.Stop();
            threadElaboration.Join();       
        }

        private void elaboration() {
            
            //TODO: decommentare se non lavoro su PC aziendale
            //startHotspot("prova4", "pippopluto");
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket started");
            //socket in ascolto
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            List<Thread> listaThreadSocket = new List<Thread>();
            try {
                //pongo il socket in ascolto sulla porta 5010
                listener.Bind(new IPEndPoint(IPAddress.Any, 5010));
                listener.Listen(10);
                listener.ReceiveTimeout = 10000;

                while (!stopThreadElaboration) {
                    allDone.Reset();
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Waiting for a connection...");
                    
                    Socket socketConnesso = listener.Accept();
                    //thread per gestire il socket connesso con il device
                    Thread threadGestioneDevice = new Thread(() => gestioneDevice(socketConnesso));
                    listaThreadSocket.Add(threadGestioneDevice);
                    threadGestioneDevice.Start();
                    threadGestioneDevice.IsBackground = false;
                    
                    allDone.WaitOne();
                }
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante il binding del socket", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                throw exception;
            } finally {
                listaThreadSocket.ForEach(thread => thread.Join());
            }
            
            //stopHotspot();
        }

        public void gestioneDevice(Socket socket) {
        //public void acceptCallback(IAsyncResult ar) {
            int MAXBUFFER = 4096;

            //setto i timeout del socket
            socket.ReceiveTimeout = 100000; //5 minuti
            socket.SendTimeout = 30000; //30 secondi

            IPEndPoint remoteIpEndPoint = null;
            try {
                remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore nel riconoscere il socket remoto", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                throw exception;
            }
            
            allDone.Set();

            try {
                Device device;

                //verifico se il dispositivo con quell'IP era già connesso
                if (CommonData.lstConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out device)) {
                    //se era gia connesso invio solo il CONFOK
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "RECONNECTED with device: " + remoteIpEndPoint.Address.ToString());

                    Utils.sendMessage(socket, "CONFOK");

                } else {
                    //se non era già configurato aspetto l'evento di Configurazione dall'interfaccia grafica
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "CONNECTED with device: " + remoteIpEndPoint.Address.ToString());

                    device = new Device(remoteIpEndPoint.Address.ToString(), 1, 0, 0);
                    //event per gestire la sincronizzazione con il thread dell'interfaccia grafica
                    ManualResetEvent deviceConfEvent = new ManualResetEvent(false);
                    CommonData.lstNoConfDevices.TryAdd(device.ipAddress, deviceConfEvent);
                    //delegato per gestire la variazione della lista dei device da configurare
                    CommonData.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                    deviceConfEvent.WaitOne();

                    //controllo se il device è stato eliminato dalla lista dei device non configurati
                    do {
                        if (!CommonData.lstNoConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out deviceConfEvent)) {
                            //il device è stato configurato
                            Utils.sendMessage(socket, "CONFOK");
                            break;
                        } else {
                            //il device non è stato configurato e quindi il thread si è risvegliato per richiedere un "IDENTIFICA"
                            Utils.sendMessage(socket, "IDENTIFICA");
                            //pulisco il buffer del messaggio da inviare
                            deviceConfEvent.Reset();
                            deviceConfEvent.WaitOne();
                        }
                    } while (true);
                }

                String syncMessage;

                do {
                    //posso riceve CONFOK O la richiesta di SYNC

                    syncMessage = Utils.receiveMessage(socket);
                    //se ricevo la richiesta di SYNC invio il timestamp attuale
                    if (syncMessage == "SYNC_CLOCK\n") {
                        //invio il timestap del server
                        Utils.syncClock(socket);
                    }
                } while (syncMessage != "CONFOK_ACK\n");

                String messagePacketsInfo;

                while (!stopThreadElaboration) {

                    //invio messaggio per indicare che può iniziare l'invio
                    Utils.sendMessage(socket, "START_SEND");

                    //ricevo messaggio con i dati dei pacchetti
                    messagePacketsInfo = Utils.receiveMessage(socket);

                    //deserializzazione del JSON ricevuto
                    PacketsInfo packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(messagePacketsInfo);

                    if (packetsInfo.listPacketInfo.Count > 0) {
                        //salvo i dati nella tabella raw del DB
                        dbManager.saveReceivedData(packetsInfo, remoteIpEndPoint.Address);
                    }

                    //invio un messaggio per dire che la ricezione è avvenuta con successo
                    Utils.sendMessage(socket, "RICEVE_OK");

                    do {
                        //ricevo messaggio per la sincronizzazione
                        syncMessage = Utils.receiveMessage(socket);

                        //se ricevo la richiesta di SYNC invio il timestamp attuale
                        if (syncMessage == "SYNC_CLOCK\n") {
                            Utils.syncClock(socket);
                        }
                    } while (syncMessage == "SYNC_CLOCK\n");
                }
            } catch (SnifferAppTimeoutSocketException e){
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, e.Message);
            } finally {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket chiuso");
            }         
        }
    }
}

