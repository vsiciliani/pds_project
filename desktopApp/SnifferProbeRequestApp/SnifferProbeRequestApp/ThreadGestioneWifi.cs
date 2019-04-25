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
        private TcpListener listener = null;
        //private Socket listener = null;
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

            // setto il listener sulla porta 5010.
            Int32 port = 5010;
            IPAddress localAddr = IPAddress.Any;
            listener = new TcpListener(localAddr, port);
            
            // starto il listener in attesa di connessione dei client
            listener.Start();

            List<Thread> listaThreadSocket = new List<Thread>();
            try {
                
                while (!stopThreadElaboration) {
                    allDone.Reset();
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Waiting for a connection...");
                    TcpClient client = listener.AcceptTcpClient();
                    
                    //thread per gestire il socket connesso con il device
                    Thread threadGestioneDevice = new Thread(() => gestioneDevice(client));
                    threadGestioneDevice.IsBackground = false;
                    threadGestioneDevice.Start();
                    listaThreadSocket.Add(threadGestioneDevice);
                    
                    allDone.WaitOne();
                }
            } catch (Exception e) {
                SnifferAppException exception = new SnifferAppException("Errore durante il binding del socket", e);
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, exception.Message);
                listener.Stop();
                throw exception;
            } finally {
                listaThreadSocket.ForEach(thread => thread.Join());
            }
            
            //stopHotspot();
        }

        public void gestioneDevice(TcpClient client) {

            int MAXBUFFER = 4096;

            IPEndPoint remoteIpEndPoint = null;
            NetworkStream stream = client.GetStream();
            try {
                remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
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

                    //Utils.sendMessage(socket, "CONFOK");

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
                            //Utils.sendMessage(socket, "CONFOK");
                            break;
                        } else {
                            //il device non è stato configurato e quindi il thread si è risvegliato per richiedere un "IDENTIFICA"
                            Utils.sendMessage(stream, remoteIpEndPoint, "IDENTIFICA");
                            //pulisco il buffer del messaggio da inviare
                            deviceConfEvent.Reset();
                            deviceConfEvent.WaitOne();
                        }
                    } while (true);
                }

                String messageReceived;
                Int16 countSyncTimestamp = 0;

                while (!stopThreadElaboration) {

                    if (countSyncTimestamp == 0) {
                        Utils.sendMessage(stream, remoteIpEndPoint, "SYNC_CLOCK");
                        messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);

                        //posso ricevere SYNC_CLOCK_START (devo sincronizzare) o SYNC_CLOCK_STOP (sincronizzazione terminata)
                        while (messageReceived == "SYNC_CLOCK_START//n") {
                            Utils.syncClock(client.Client);
                            messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);
                        }
                        //sincronizzo i timestamp ogni 5 interazioni
                        countSyncTimestamp = 5;
                    }

                    //invio messaggio per indicare che può iniziare l'invio
                    Utils.sendMessage(stream, remoteIpEndPoint, "START_SEND");

                    messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);

                    //deserializzazione del JSON ricevuto
                    PacketsInfo packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(messageReceived);

                    if (packetsInfo.listPacketInfo.Count > 0) {
                        //salvo i dati nella tabella raw del DB
                        dbManager.saveReceivedData(packetsInfo, remoteIpEndPoint.Address);
                    }
                    //decremento il contatore per la sincronizzazione dei timestamp
                    countSyncTimestamp--;
                }
            } catch (SnifferAppTimeoutSocketException e){
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, e.Message);
            } finally {
                client.Close();
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket chiuso");
            }         
        }
    }
}

