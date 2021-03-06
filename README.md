# Progetto di programmazione di sistema

![Banner](resources/banner.png)

Rilevatore di persone in un'area utilizzando il wifi delle schede ESP32 ([Traccia del progetto](resources/Traccia.pdf))

## Struttura progetto

Il progetto è composto da due moduli:
1.  **[Modulo ESP](esp32/main):** è il codice da flashare sul modulo ESP. Permette di:
    *  connettere l'ESP alla rete wifi;
    *  aprire un socket con il server;
    *  sincronizzare il modulo ESP con il server;
    *  sniffare pacchetti wifi di PROBE REQUEST;
    *  scambiare messaggi dati con il server.

2. **[Modulo desktop](desktopApp/SnifferProbeRequestApp):** è l'applicazione che implementa il server. Permette di: 
    *  connettersi con i moduli ESP;
    *  configurare i moduli ESP connessi;
    *  riceve messaggi dai moduli ESP;
    *  salvare le informazioni ricevute in un DB;
    *  elaborare le informazioni ricevute per generare l'output desiderato (numero di device connessi nel tempo e posizione dei device);
    *  gestire una interfaccia grafica per la gestione dell'applicativo.

## Protocollo di comunicazione
Il protocollo di comunicazione tra il server e i rilevatori ESP32 è il seguente:
<p align="center">
	<img src="resources/protocol_sequence.png">
</p>

## Modalità di esecuzione moduli

Il **modulo ESP** è un programma C++ che va *flashato* sui dispositivi ESP32. Per la sua esecuzione sono necessari i seguenti passaggi:
1. unzippare l'archivio **Esp32CodeV1.0.zip** associato alla release selezionata;
2. entrare nella cartella creata dall'operazione precedente;
3. Eseguire il seguente comando da console *bash* : ```make flash```.

Il **modulo desktop** è una applicazione Windows e per essere eseguita è sufficiente lanciare il file eseguibile **SnifferProbeRequestApp.exe**. Tramite il file **SnifferProbeRequestApp.exe.config** è possibile settare la porta su cui il server è in ascolto.
