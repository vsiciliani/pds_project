# MODULO ESP

Il codice del modulo Wifi è suddiso nei seguenti file **.cpp**:
1.  **main_app.cpp:** è il main del codice. L'algoritmo è composto dai seguenti passi:
    1. inizializzazione del modulo ESP;
    2. connessione alla rete WiFi;
    3. apertura del socket verso il server;
    4. abilitazione della modalità promiscua sulla scheda WiFi;
    5. parsing dei messaggi dal server (in un loop infinito):
        * messaggio **IDENTIFICA**: stacca un thread in cui gira una funzione che fa lampeggiare il LED built-in sul modulo ESP;
        * messaggio **SYNC_CLOCK**: esegue una funzione di sincronizzazione del clock interno con quello del server (tramite scambio di 4 messaggi che contengono dei timestamp);
        * messaggio **START_SEND**: 
            * setto l'handler per la gestione del messaggio ricevuto (parsing del messaggio e aggiunta del messaggio in una lista);
            * il dispositivo rimane in attesa di ricevere nuovi messaggi (e di gestirli tramite l'*handler*) fin quando non passa il tempo di attesa definito (1 minuto) o fin quando la mememoria occupata non supera il 90% di quella totale;
            * invio sul socket con il server di un messaggio JSON che serializza i dati contenuti nella lista dei messaggi ricevuti e parsati;
            * pulizia della lista di messaggi ricevuti.
    
    La concorrenza sulla lista dei messaggi ricevuti da parte dei vari handler, in caso di ricezione contemporanea di più messaggi, è gestita da un **mutex** con l'uso di *unique_lock* e *lock_guard*.
    Il main thread tra un invio dei dati ad un server ed il successivo utilizza una chiamata bloccante **wait** sullo *unique_lock* che viene risvegliato da una notifica inviata sul lock da un handler quando finisce la sua funzionalità. Al suo risveglio una funzione controlla se l'elaborazione nel main thread deve continuare (passato il tempo massimo tra un invio e il successivo o memoria troppo piena) oppure deve rimettersi in attesa.
2. **PacketInfo.cpp:** è la classe che definisce l'oggetto che contiene le informazioni da salvare dopo il parsing di un pacchetto e implementa la funzione di serializazzione in JSON dell'oggetto stesso.
3. **Socket.cpp:** è la classe che gestisce il Socket per la connessione al server. Implementa le seguenti funzionalità:
    * connect;
    * send;
    * receive (riceve il messaggio in stringa);
    * receiveRaw (riceve il messaggio in byte);
    * close.
4. **WifiPacket.cpp**: è la classe che implementa la logica di parsing del pacchetto ricevuto dalla scheda di rete;
5. **config.cpp**: è il file di configurazione del modulo. Permette di settare:
    * *Wifi SSID* della rete in cui è presente il server;
    * *Wifi Password* della rete in cui è presente il server;
    * *Server Host*
    * *Server Port*
    * *Intervallo di connessione con il server* espresso in secondi (tempo tra una send dei dati verso il server e la successiva);
    * *Timeout riconnessione server* espesso in secondi (tempo tra un tentivato di connessione con il server e il successivo in caso di fallimento).