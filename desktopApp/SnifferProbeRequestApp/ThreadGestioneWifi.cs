﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SnifferProbeRequestApp.valueClass;

namespace SnifferProbeRequestApp {
    /// <summary>
    /// Classe Singleton che wrappa il thread per la gestione del Wifi e dell'interfaccia verso le ESP
    /// </summary>
    class ThreadGestioneWifi {

        private NetworkSettings settings;
        private volatile bool stopThreadElaboration;
        private ThreadStart delegateThreadElaboration;
        private Thread threadElaboration;

        private static ThreadGestioneWifi instance = null;
        private TcpListener listener;
        //private static ManualResetEvent allDone = new ManualResetEvent(false);
        private DatabaseManager dbManager = null;

        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        private ThreadGestioneWifi(NetworkSettings settings) {
            this.settings = settings;
            stopThreadElaboration = false;
            //instanzio il db manager
            dbManager = DatabaseManager.getInstance();
            start();
        }

        ///<summary>Ritorna l'istanza della classe ThreadGestioneWifi se già creata, oppure la istanzia</summary>
        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nell'apertura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        ///<returns>L'istanza della classe ThreadGestioneWifi</returns>
        static public ThreadGestioneWifi getInstance(NetworkSettings settings) {
            if (instance == null) {
                instance = new ThreadGestioneWifi(settings);
            }
            return instance;
        }

        ///<summary>Stacca un nuovo thread per la gestione delle connessioni con i rilevatori Wifi</summary>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore nell'apertura di un nuovo thread</exception>
        public void start() {
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

        ///<summary>Chiude le risorse aperte e attende la chiusura del thread di gestione dei rilevatori</summary>
        ///<exception cref = "SnifferAppDBConnectionException">Eccezione lanciata in caso di errore nella chiusura della connessione al DB</exception>
        ///<exception cref = "SnifferAppThreadException">Eccezione lanciata in caso di errore sul kill di un thread</exception>
        public void stop() {
            stopThreadElaboration = true;
            dbManager.closeConnection();
            try {
                listener.Stop();
                threadElaboration.Join();
            } catch (Exception e) {
                string message = "Errore durante l'abort su un thread";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppThreadException(message, e);
            }
        }

        /// <summary>
        /// Metodo che viene elaborato nel thread per la gestione dei rilevatori
        /// </summary>
        private void elaboration() {
            
            // setto il listener sulla porta 5010.
            int port = 5010;
            IPAddress localAddr = IPAddress.Any;
            listener = new TcpListener(localAddr, settings.servicePort);
            
            try {
                // starto il listener in attesa di connessione dei client
                listener.Start();
            } catch (SocketException) {
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore durante l'apertura del socket");
            } 
            
            //lista che contiene i puntamenti ai threads per la gestione dei singoli rilevatori
            List<Thread> listaThreadSocket = new List<Thread>();
            
            //ciclo fin quando non viene invocato il metodo stop in attesa di nuovi rilevatori 
            while (!stopThreadElaboration) {
                //allDone.Reset();
                TcpClient client = null;
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "In attesa di connessioni...");
                try {
                    //aspetto una connessione da parte di un rilevatore
                    //con il controllo sul metodo pending evito la chiamata bloccante AcceptTcpClient() che non mi fa chiudere il thread quando chiudo la GUI
                    while (!listener.Pending()) {
                        Thread.Sleep(2000);
                    }
                    client = listener.AcceptTcpClient();
                } catch (SocketException) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore durante il binding del socket");
                    break;
                } catch (InvalidOperationException) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Il listener TCP è stato chiuso");
                    break;
                }

                try {
                    //thread per gestire il socket connesso con il device
                    Thread threadGestioneDevice = new Thread(() => gestioneDevice(client));
                    threadGestioneDevice.IsBackground = false;
                    //starto il thread
                    threadGestioneDevice.Start();
                    //aggiungo il thread alla lista dei thread
                    listaThreadSocket.Add(threadGestioneDevice);
                } catch (Exception) {
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Error, "Errore durante il binding del socket");
                    client.Close();
                    //devo togliere il device dalla lista dei configurati e aggiungerlo tra quelli non configurati
                    //questo codice è duplicato in frmMain (valutare se fare una funzione)
                    Device device;
                    ConfDevice.lstConfDevices.TryRemove((client.Client.RemoteEndPoint as IPEndPoint).Address.ToString(), out device);
                    ConfDevice.OnLstConfDevicesChanged(this, EventArgs.Empty);

                    NoConfDevice.lstNoConfDevices.TryAdd((client.Client.RemoteEndPoint as IPEndPoint).Address.ToString(), device.evento);
                    NoConfDevice.OnLstNoConfDevicesChanged(this, EventArgs.Empty);
                    break;
                }

                //allDone.WaitOne();
            }
            //attendo la fine di tutti i thread
            listaThreadSocket.ForEach(thread => thread.Join());
        }

        ///<summary>metodo che gestisce la connessione socket con un rilevatore</summary>
        ///<exception cref = "SnifferAppSocketException">Eccezione lanciata in caso di errore su una operazione sul socket</exception>
        public void gestioneDevice(TcpClient client) {

            IPEndPoint remoteIpEndPoint = null;
            Device device;
            NetworkStream stream = client.GetStream();
            try {
                remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            } catch (Exception e) {
                string message = "Errore nel riconoscere il socket remoto";
                Utils.logMessage(this.ToString(), Utils.LogCategory.Error, message);
                throw new SnifferAppSocketException(message, e);
            }

            //allDone.Set();

            //verifico se il dispositivo con quell'IP era già connesso (l'IP del dispositivo era contentuto già nella lstConfDevices)
            if (ConfDevice.lstConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out device)) {
                //il dispositivo era gia configurato
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "RECONNECTED with device: " + remoteIpEndPoint.Address.ToString());
                    
            } else {
                //se non era già configurato aspetto l'evento di Configurazione dall'interfaccia grafica
                Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "CONNECTED with device: " + remoteIpEndPoint.Address.ToString());

                //event per gestire la sincronizzazione con il thread della GUI
                ManualResetEvent deviceConfEvent = new ManualResetEvent(false);
                //aggiungo il dispositivo (con il rispettivo Event) nella lista dei dispositivi non configurati
                NoConfDevice.lstNoConfDevices.TryAdd(remoteIpEndPoint.Address.ToString(), deviceConfEvent);
                //delegato per gestire la variazione della lista dei device da configurare 
                NoConfDevice.OnLstNoConfDevicesChanged(this, EventArgs.Empty);

                //mi risveglio quando dalla GUI è richiesta la configurazione del dispositivo o si vuole inviare al rilevatore il segnale "IDENTIFICA"
                deviceConfEvent.WaitOne();

                do {
                    //controllo se il device è stato eliminato dalla lista dei device non configurati
                    if (!NoConfDevice.lstNoConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out deviceConfEvent)) {
                        //il device è stato configurato
                        break;
                    } else {
                        //il device non è stato configurato e quindi il thread si è risvegliato per richiedere un "IDENTIFICA"
                        Utils.sendMessage(stream, remoteIpEndPoint, "IDENTIFICA");
                        
                        deviceConfEvent.Reset();
                        deviceConfEvent.WaitOne();
                    }
                } while (true);
            }

            string messageReceived;
            //count per triggerare la sincronizzazione dei clock tra il server e il rilevatore
            int countSyncTimestamp = 0;

            //ciclo fin quando non viene invocato il metodo stop in attesa di messaggi da parte dei rilevatori
            while (!stopThreadElaboration) {
                //se il count è a 0 eseguo la sincronizzazione
                if (countSyncTimestamp == 0) {
                    //invio il messaggio per iniziare la sincronizzazione
                    Utils.sendMessage(stream, remoteIpEndPoint, "SYNC_CLOCK");
                    messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);

                    //posso ricevere SYNC_CLOCK_START (devo sincronizzare) o SYNC_CLOCK_STOP (sincronizzazione terminata)
                    while (messageReceived == "SYNC_CLOCK_START//n") {
                        //invio il segnale di sincronizzazione
                        Utils.syncClock(client.Client);
                        messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);
                    }
                    //resetto il count; sincronizzo i timestamp ogni 5 interazioni
                    countSyncTimestamp = 5;
                }

                //invio messaggio per indicare che può iniziare l'invio dei dati
                Utils.sendMessage(stream, remoteIpEndPoint, "START_SEND");

                //attendo il JSON dal rilevatore con i pacchetti catturati dall'ultima interazione
                messageReceived = Utils.receiveMessage(stream, remoteIpEndPoint);
                PacketsInfo packetsInfo = null;

                try {  
                    //deserializzazione del JSON ricevuto
                    packetsInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PacketsInfo>(messageReceived);
                } catch (Exception) { 
                    Utils.logMessage(this.ToString(), Utils.LogCategory.Warning, "Errore nella deserializzazione del messaggio JSON. Il messaggio verrà scartato");           
                }
                //controllo che ci siano messaggi, che il device sia tra quelli configurati e che ci siano almeno 2 device configurati
                if (packetsInfo != null && ConfDevice.lstConfDevices.Count>=2) {
                    if (packetsInfo.listPacketInfo.Count > 0 && ConfDevice.lstConfDevices.TryGetValue(remoteIpEndPoint.Address.ToString(), out device)) {
                        //salvo i dati nella tabella raw del DB
                        dbManager.saveReceivedData(packetsInfo, remoteIpEndPoint.Address);
                    }
                }
                    
                //decremento il contatore per la sincronizzazione dei timestamp
                countSyncTimestamp--;
            }
            
            //chiudo il client TCP
            client.Close();
            Utils.logMessage(this.ToString(), Utils.LogCategory.Info, "Socket chiuso");
               
        }         
    }
}