# MODULO DESKTOP

Il modulo desktop, realizzato in **C#**, ha il compito di gestire la connessione con i rilevatori WiFi, raccogliere i dati, elaborarli e mostrarli all'utente tramite la GUI.
E' composto da più **thread**:
* Thread per la gestione della **GUI** e delle interazioni con l'utente;
* Thread per la **gestione delle connessioni** in ingresso verso il server da parte dei vari rilevatori. Gestisce il socket in ascolto sulla porta *5010* e accetta le connessioni in ingresso.
* Thread per la **gestione della connessione con un rilevatore** (viene staccato un thread per ogni rilevatore). Questo thread mantiene attiva la connessione e si occupa di inviare/ricevere messaggi con il rilevatore associato.

La sincronizzazione tra i vari thread è gestita (dove necessario) con l'utilizzo di *Event*. 
Per la gestione dei dati comuni si è utilizzato la struttura dati *ConcurrentDictionary* e le proprietà *ACID* del DB relazionale in cui sono salvati i dati.

Il modulo permette di **accettare** la connessione con *N* rilevatori in qualsiasi momento e di **configurarli** indicando la posizione che occupano nello spazio. Permette anche di **modificare** la configurazione dei dispositivi in qualsiasi momento.
Dopo aver configurato almeno due rilevatori WiFi il modulo inizierà a **salvare** in dati ricevuti dai rilevatori nel DB ed a **elaborarli**. Le rilevazioni che vengono mostrate dall'applicativo sono le seguenti:
1. **Numero** di dispositivi connessi continuamente negli ultimi 5 minuti;
2. **Posizione** (media) dei dispositivi rilevati nell'ultimo minuto;
3. **Periodi temporali** in cui i TOP N (per tempo rilevato) sono stati rilevati;
4. **Movimento dei device** rilevati all'interno di un periodo temporale specificato.

Il modello dati si basa su *due tabelle* all'interno del DB:
1. **Packets**: è la tabella che contiene i dati così come sono ricevuti dai rilevatori;
2. **AssembledPacketInfo**: è la tabella che contiene i dati aggregati della tabella **Packets**. E' la tabella su cui si basano tutte le informazioni mostrate nella GUI.

## Richiami teorici

In questo modulo sono stati implementati i seguenti argomenti trattati durante il corso:

1. Programmazione **multi thread** ([Link](ThreadGestioneWifi.cs));
2. Utilizzo **strutture dati thread safe** ([Link1](NoConfDevice.cs#L13), [Link2](ConfDevice.cs#L12));
3. Utilizzo di **delegati** ([Link](ConfDevice.cs#L15)) e **EventHandler** ([Link](ConfDevice.cs#L23));
4. Utilizzo dei **ManualResetEvent** per la gestione della sincronizzazione ([Link](ThreadGestioneWifi.cs#L172)); 
5. Gestione strutturata delle **eccezioni** ([Link](SnifferAppException.cs)).
