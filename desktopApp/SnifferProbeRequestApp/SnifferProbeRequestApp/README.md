# MODULO DESKTOP

Il modulo desktop, realizzato in **C#**, ha il compito di gestire la connessione con i rilevatori Wifi, raccogliere i dati e mostrare i dati all'utente tramite la GUI.
E' composto da più **thread**:
* Thread per la gestione della **GUI** e delle interazioni con l'utente;
* Thread per la **gestione delle connessioni** in ingresso verso il server da parte dei vari rilevatori. Gestisce il socket in ascolto sulla porta *5010* e accetta le connessioni in ingresso.
* Thread per la **gestione della connessione con un rilevatore** (viene staccato un thread per ogni rilevatore). Questo thread mantiene attiva la connessione e si occupa di inviare/ricevere messaggi con il rilevatore associato.

La sincronizzazione tra i vari thread è gestita (dove necessario) con l'utilizzo di *Event*. 
Per la gestione dei dati comuni si è utilizzato la struttura dati dei *ConcurrentDictionary* e le proprietà *ACID* del DB relazionale in cui sono salvati i dati.