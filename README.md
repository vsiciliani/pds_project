# Progetto di programmazione di sistema

Rilevatore di persone in un'area utilizzando il wifi delle schede ESP32

Il progetto è composto da due moduli:
1.  **Modulo ESP:** è il codice da flashare sul modulo ESP che permette di:
    *  connettere l'ESP alla rete wifi;
    *  aprire un socket con il server;
    *  sincronizzare il modulo ESP con il server;
    *  sniffare pacchetti wifi di PROBE REQUEST;
    *  scambiare messaggi dati con il server.

2. **Mudulo desktop:** è l'applicazione che implementa il server. Permette di: 
    *  connettersi con i moduli ESP;
    *  configurare i moduli ESP connessi;
    *  riceve messaggi dai moduli ESP;
    *  salvare le informazioni ricevute in un DB;
    *  elaborare le informazioni ricevute per generare l'output desiderato (numero di device connessi nel tempo e posizione dei device);
    *  gestire una interfaccia grafica per la gestione dell'applicativo.