using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//Classe Singleton che wrappa il thread per la gestione del Wifi e dell'interfaccia verso le ESP
namespace SnifferProbeRequestApp {
    class ThreadGestioneWifi {
        private volatile bool stopThreadElaboration;
        private ThreadStart delegateThreadElaboration;
        private Thread threadElaboration;

        private static ThreadGestioneWifi instance = null;
        private TcpListener listener;
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private DatabaseManager dbManager = null;

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        private ThreadGestioneWifi() {
            stopThreadElaboration = false;
            start();
        }

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        static public ThreadGestioneWifi getInstance() {
            if (instance == null) {
                instance = new ThreadGestioneWifi();
            }
            return instance;
        }

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        public void start() {
            //instanzio il db manager
            dbManager = DatabaseManager.getInstance();

            try {
                //starto il thread in background che gestisce le connessioni con i devices
                delegateThreadElaboration = new ThreadStart(elaboration);
                threadElaboration = new Thread(delegateThreadElaboration);
                threadElaboration.IsBackground = true;
                threadElaboration.Start();
            } catch (Exception e) {
                string message = "Errore durante l'apertura di un nuovo thread";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppThreadException(message, e);
            }
        }

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nella chiusura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore sulla join di un nuovo thread</exception>
        public void stop() {
            stopThreadElaboration = true;
            dbManager.closeConnection();
            try {
                threadElaboration.Join();
            } catch (Exception e) {
                string message = "Errore durante la join su un thread";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppThreadException(message, e);
            }
        }

        private void elaboration() {
            
            //TODO: decommentare se non lavoro su PC aziendale
            //startHotspot("prova4", "pippopluto");
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket started");
            
            // setto il listener sulla porta 5010.
            int port = 5010;
            IPAddress localAddr = IPAddress.Any;
            listener = new TcpListener(localAddr, port);
            try {
                // starto il listener in attesa di connessione dei client
                listener.Start();
            } catch (SocketException) {
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore durante l'apertura del socket");
            } 
            

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
            } catch (Exception) {
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore durante il binding del socket");
            } finally {
                listaThreadSocket.ForEach(thread => thread.Join());
            }
            //stopHotspot();
        }

        public void gestioneDevice(TcpClient client) {

            IPEndPoint remoteIpEndPoint;
            NetworkStream stream = client.GetStream();
            try {
                remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            } catch (Exception) {
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore nel riconoscere il socket remoto");
                client.Close();
                return;
            }
            
            allDone.Set();

            try {
                Device device;

                //verifico se il dispositivo con quell'IP era già connesso
                if (CommonData.lstConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out device)) {
                    //il dispositivo era gia configurato
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "RECONNECTED with device: " + remoteIpEndPoint.Address.ToString());
                    
                } else {
                    //se non era già configurato aspetto l'evento di Configurazione dall'interfaccia grafica
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "CONNECTED with device: " + remoteIpEndPoint.Address.ToString());

                    //event per gestire la sincronizzazione con il thread dell'interfaccia grafica
                    ManualResetEvent deviceConfEvent = new ManualResetEvent(false);
                    CommonData.lstNoConfDevices.TryAdd(remoteIpEndPoint.Address.ToString(), deviceConfEvent);
                    //delegato per gestire la variazione della lista dei device da configurare
                    CommonData.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                    deviceConfEvent.WaitOne();

                    //controllo se il device è stato eliminato dalla lista dei device non configurati
                    do {
                        if (!CommonData.lstNoConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out deviceConfEvent)) {
                            //il device è stato configurato
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

                string messageReceived;
                int countSyncTimestamp = 0;

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
                    PacketsInfo packetsInfo;

                    try {  
                        //deserializzazione del JSON ricevuto
                        packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(messageReceived);
                    } catch (Exception) { 
                        Utils.logMessage(this.ToString(), Utils.LogCategory.Warning, "Errore nella deserializzazione del messaggio JSON. Il messaggio verrà scartato");
                        break;
                    }
                    //controllo che ci siano messaggi e che il device sia tra quelli configurati
                    if (packetsInfo.listPacketInfo.Count > 0 &&
                            CommonData.lstConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out device)) {
                            //salvo i dati nella tabella raw del DB
                            dbManager.saveReceivedData(packetsInfo, remoteIpEndPoint.Address);
                        }
                    
                    //decremento il contatore per la sincronizzazione dei timestamp
                    countSyncTimestamp--;
                }
            } catch (SnifferAppSocketException e){
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, e.Message);
            } finally {
                client.Close();
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket chiuso");
            }         
        }
    }
}