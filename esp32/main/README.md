# MODULO ESP

Il codice del modulo ESP è suddiso nei seguenti file **.cpp**:
1.  **main_app.cpp:** è il main del codice. L'algoritmo è composto dai seguenti passi:
    1. inizializzazione del modulo ESP;
    2. connessione alla rete WiFi;
    3. apertura del socket verso il server;
    4. abilitazione della modalità promiscua sulla scheda WiFi;
    5. parsing dei messaggi dal server (in un loop infinito). I possibili messaggi sono:
        * messaggio **IDENTIFICA**: stacca un thread in cui gira una funzione che fa lampeggiare il LED built-in sul modulo ESP;
        * messaggio **SYNC_CLOCK**: esegue una funzione di sincronizzazione del clock interno con quello del server (tramite scambio di 4 messaggi che contengono dei timestamp);
        * messaggio **START_SEND**. Esegue in successione i seguenti passi: 
            * configurazione dell'handler per la gestione del messaggio catturato (parsing del messaggio e aggiunta del messaggio in una lista);
            * attesa cattura di nuovi messaggi di tipo **PROBE REQUEST** (e di gestirli tramite l'*handler*) fin quando non trascorre il tempo di attesa definito nel file **config.cpp** (campo *Intervallo di connessione con il server*) o fin quando la memoria occupata non supera il 90% di quella totale;
            * invio sul socket con il server di un messaggio JSON che serializza i dati contenuti nella lista dei messaggi catturati e parsati;
            * pulizia della lista dei messaggi ricevuti.
            
    La concorrenza degli accessi, da parte dei diversi thread, sulla lista dei messaggi catturati e gestiti da parte dei vari handler, in caso di ricezione contemporanea di più messaggi, è gestita da un **mutex** con l'uso di *unique_lock* e *lock_guard*.
    Il main thread tra i vari invii dei dati al server utilizza una chiamata bloccante **wait** sullo *unique_lock* che viene risvegliato da una notifica inviata sul lock da un handler quando finisce la sua funzionalità. Al risveglio del main thread una funzione controlla se la sua elaborazione deve continuare (superamento del tempo massimo tra un invio e il successivo o memoria disponibile inferiore al 10%) oppure deve rimettersi in attesa.
2. **PacketInfo.cpp:** è la classe che definisce l'oggetto che contiene le informazioni da salvare dopo il parsing di un pacchetto e implementa la funzione di serializazzione in *JSON* dell'oggetto stesso.
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
    * *Timeout riconnessione server* espresso in secondi (tempo tra un tentativo di connessione con il server e il successivo in caso di fallimento).

## Richiami teorici

In questo modulo sono stati implementati i seguenti argomenti trattati durante il corso:

1. Programmazione **multi thread** ([Link](main_app.cpp#L84)); 
2. Utilizzo dei **lock** e dei **mutex** per la sincronizzazione tra più thread([Link](main_app.cpp#L94));
3. Utilizzo delle **condition variable** ([Link](main_app.cpp#L107));
4. Utilizzo degli **smart pointer** per gestire i puntatori ([Link](main_app.cpp#L70));
5. Utilizzo del **distruttore** per rilasciare le risorse associate alle istanze di una classe ([Link](Socket.cpp#L84));
6. Implementazione **overload degli operatori** ([Link](Socket.cpp#L89));
7. Gestione degli **opeatori e costruttori di copia e movimento** ([Link](Socket.h#L28)).

In questo modulo è stata volontariamente omessa la gestione delle eccezioni in quanto tutto le casistiche di errore *standard* (errori nella connessione alla rete Wifi, errori nella creazione del socket verso il server o messaggi inattesi nella comunicazione con il server) sono state gestite nel codice. Tutte le altre casistiche inattese portano al *reboot* del dispositivo che, quindi, riparte da una condizione di lavoro pulita.

Inoltre, vista la velocità di reboot il dispositivo si ricollegherà alla rete Wifi con lo stesso IP (anche in caso di rete che utilizza un *DHCP*) e quindi la riconnessione sarà gestita totalmente dal server rendendola completamente *trasparente* per l'utente che non dovrà riconfigurare il dispositivo manualmente.   
 
