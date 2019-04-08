/*
 * main.cpp
 *
 *  Created on: 23 ago 2018
 *      Author: SicilianiVi
 */

#include <esp_wifi.h>
#include <esp_wifi_types.h>
#include <nvs_flash.h>
#include <esp_log.h>
#include <string>
#include <list>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <time.h>
#include "Wifi.h"
#include "WifiPacket.h"
#include "PacketInfo.h"
#include "Socket.h"
#include "sdkconfig.h"
#include "GPIO.h"

static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);
bool checkTimeoutThreadConnessionePc();
void connectSocket();
void blinkLed();
void syncClock();
std::string createJSONArray(std::list<std::string>);
bool sendMessage(std::string message);
int receiveMessage(unsigned char buffer[128], size_t size);

static char tag[]="Sniffer-ProbeRequest";

std::list<std::string> listaRecord;
std::mutex m;
std::condition_variable cvMinuto;
time_t startWaitTime;
WiFi wifi;
Socket *s;
//buffer per salvare i messaggi in ingresso
unsigned char bufferReceive[128];
int numByteReceived;
//memoria inizialmente disponibile
float memorySpace;
//mi salvo quanta memoria ho ancora disponibile
//int freeMemory;

extern "C" {
   void app_main();
}


void app_main() {

	nvs_flash_init();

	//setto il led come output
	ESP32CPP::GPIO::setOutput(GPIO_NUM_2); //GPIO_NUM_2BUILTIN LED

	//connetto il dispositivo alla rete Wifi
	wifi.connectAP("Vodafone-50650385", "pe7dt3793ae9t7b");
	//wifi.connectAP("APAndroid2", "pippopluto");
	//TODO: verificare connessione WIFI
	std::cout << "Connesso a "<<wifi.getStaSSID() << " con IP: "<<wifi.getStaIp()
					  <<" Gateway: "<< wifi.getStaGateway() <<std::endl;

	//creo il socket
	s = new Socket();

	//connetto il socket
	connectSocket();
	s->setTimeout(5);
	//CONFIGURAZIONE INIZIALE

	//ciclo per gestire i messaggi della configurazione iniziale del dispositivo
	do {
		numByteReceived = receiveMessage(bufferReceive,128);
		ESP_LOGI(tag, "messaggio ricevuto: %s", bufferReceive);

		//controllo il contenuto del messaggio
		if (memcmp(bufferReceive, "IDENTIFICA",numByteReceived)==0) {
				//ho ricevuto la richiesta di IDENTIFICAZIONE
				ESP_LOGI(tag, "ho ricevuto IDENTIFICA");
				//lancio il thread che si occupa di far lampeggiare il led
				std::thread threadBlinkLed (blinkLed);
				//stacco il thread dal flusso principale
				threadBlinkLed.detach();
		/*} else if (memcmp(bufferReceive, "CONFOK",numByteReceived)==0){
			//se ricevo CONFOK invio ACK e termino il ciclo della configurazione
			syncClock();
			sendMessage("CONFOK_ACK\n");
			//break;*/
		} else if (memcmp(bufferReceive, "START_SEND",numByteReceived)==0){
			ESP_LOGI(tag, "ThreadConnessionePc -- Ricevuto START_SEND");

			//setto l'handler che gestisce la ricezione del pacchetto
			ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

			//abilito la modalità di attività promiscua
			ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));
			ESP_LOGI(tag, "ThreadConnessionePc -- Modalita schema promiscua abilitata");

			//salvo il timestamp per calcolare il tempo di flush verso il server
			time(&startWaitTime);

			//salvo la memoria a disposizione
			memorySpace = xPortGetFreeHeapSize();
			std::cout<<"Memoria disponibile " << memorySpace << std::endl;

			//prendo il lock per leggere la lista di PacketInfo
			std::unique_lock<std::mutex> ul(m);
			//condition variable sul tempo di attesa per il flush
			cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

			ESP_LOGI(tag, "ThreadConnessionePc -- Invio dati dei pacchetti al server");
			//send dei dati verso il server

			sendMessage(createJSONArray(listaRecord));

			/*do {

				numByteReceived = receiveMessage(bufferReceive,128);
			//aspetto che l'invio dei dati sia completato ("RICEVE_OK")
			} while (memcmp(bufferReceive, "RICEVE_OK",numByteReceived)==0);

			ESP_LOGI(tag, "ThreadConnessionePc -- Ricevuto RICEVE_OK");
			sendMessage("RICEVE_OK_ACK\n");*/

			//pulisco la lista di PacketInfo
			listaRecord.clear();

			//disabilito la modalità di attività promiscua
			ESP_ERROR_CHECK(esp_wifi_set_promiscuous(false));
			ESP_LOGI(tag, "ThreadConnessionePc -- Modalita schema promiscua disabilitata");


		} else { ESP_LOGI(tag, "Ricevuto messaggio non valido"); }
	} while (true);















	/*

	//setto l'handler che gestisce la ricezione del pacchetto
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

	//abilito la modalità di attività promiscua
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));
	ESP_LOGI(tag, "ThreadConnessionePc -- Modalita schema promiscua abilitata");

	//setto un counter per effettuare la sincronizzazione del clock ogni 10 iterazioni
	int numIteration = 0;
	//salvo la memoria a disposizione
	memorySpace = xPortGetFreeHeapSize();
	std::cout<<"Memoria disponibile " << memorySpace << std::endl;

	while (true) {
		//verifico la connessione al socket
		//connectSocket();

		//ricevo l'ok per il SEND ("START_SEND")
		do {
			numByteReceived = receiveMessage(bufferReceive,128);
		} while (memcmp(bufferReceive, "START_SEND",numByteReceived)==0);
		ESP_LOGI(tag, "ThreadConnessionePc -- Ricevuto START_SEND");

		//salvo il timestamp per calcolare il tempo di flush verso il server
		time(&startWaitTime);
		//sleep(15);
		//prendo il lock per leggere la lista di PacketInfo
		std::unique_lock<std::mutex> ul(m);
		//condition variable sul tempo di attesa per il flush
		cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

		ESP_LOGI(tag, "ThreadConnessionePc -- Invio dati dei pacchetti al server");
		numIteration ++;
		//send dei dati verso il server

		sendMessage(createJSONArray(listaRecord));

		//aspetto che l'invio dei dati sia completato ("RICEVE_OK")
		do {
			numByteReceived = receiveMessage(bufferReceive,128);
		} while (memcmp(bufferReceive, "RICEVE_OK",numByteReceived)==0);

		ESP_LOGI(tag, "ThreadConnessionePc -- Ricevuto RICEVE_OK");

		if (numIteration >= 3) {
			//rieseguo la sincronizzazione del clock
			ESP_LOGI(tag, "ThreadConnessionePc -- Richiesta sincronizzazione");
			syncClock();
			sendMessage("SYNC_OK\n");
			ESP_LOGI(tag, "ThreadConnessionePc -- Sincronizzazione effettuata");
			numIteration=0;
		} else {
			//non eseguo la sincronizzazione
			sendMessage("NO_SYNC\n");
			ESP_LOGI(tag, "ThreadConnessionePc -- Sincronizzazione non richiesta");
		}

		//pulisco la lista di PacketInfo
		listaRecord.clear();

	}

	fflush(stdout);
	 */
}

//processo di gestione del pacchetto Wifi sniffato
void wifi_sniffer_packet_handler(void* buff, wifi_promiscuous_pkt_type_t type){

	//controllo che il pacchetto si di tipo MANAGEMENT
	if (type != WIFI_PKT_MGMT)
		//se non è di tipo WIFI_PKT_MGMT non e una probe request e quindi scarto il pacchetto
		return;

	WifiPacket pacchetto = WifiPacket(buff);

	//controllo ancora che il type sia 0 e quindi il pacchetto sia di tipo MANAGEMENT
	//che il subtype sia 4 e quindi che sia una PROBE REQUEST
	if ((pacchetto.getTypeMessage() == 0) && (pacchetto.getSubTypeMessage() == 4)) {

		//creo l'oggetto PacketInfo per contenere le informazioni del pacchetto
		PacketInfo record = PacketInfo(pacchetto.getSourceMacAddress(), pacchetto.getSSID(), pacchetto.getSignalStrength(), pacchetto.getHashCode(), pacchetto.getTimestamp());
		//ESP_LOGI(tag, "JSON: %s", record.JSONSerializer().c_str());
		//lock sulla scrittura della lista che contiene gli oggetti PacketInfo
		std::lock_guard<std::mutex> l(m);
		listaRecord.push_back(record.JSONSerializer());

		//std::cout<<"Memoria disposinibile: " << freeMemory << std::endl;
		//setto la condition variable
		cvMinuto.notify_one();
		ESP_LOGI(tag, "ThreadGestionePacchetto -- Pacchetto ricevuto ed elaborato");
	}

}

//processo che controlla che tra un flush verso il server e il successivo passi il tempo stabilito
bool checkTimeoutThreadConnessionePc() {
	time_t now;
	time(&now);
	float freeMemory = xPortGetFreeHeapSize();
	std::cout<<"Memoria disposinibile: " << freeMemory << std::endl;
	float percMemoryAvailable =freeMemory/memorySpace;
	std::cout<<"Percentuale " << percMemoryAvailable << std::endl;
	if ((difftime(now,startWaitTime)>15) || (percMemoryAvailable < 0.1)) {
		return true;
	}
	else return false;
}

//preparo il messaggio JSON da inviare al server
std::string createJSONArray(std::list<std::string>){
	std::list<std::string>::iterator it;
	std::string buf;
	buf="{ \"listPacketInfo\" : [ ";
	for (it= listaRecord.begin(); it != listaRecord.end(); it++){
		if (it!= listaRecord.begin()){
			buf+=", ";
		}
		buf+=it->c_str();
	}
	buf+="]}\n";
	//std::cout << "Messaggio inviato: " << buf << std::endl;
	return buf;
}

//funzione che esegue la connessione al socket se non e' già connesso
void connectSocket(){
	if (!s->isValid()){
		int res = s->connect("192.168.1.100", 5010);
		//int res = s->connect("192.168.43.213", 5010);

			while (res < 0) {
				ESP_LOGI(tag, "ThreadConnessionePc -- Connessione con il server fallita. Nuovo tentativo tra 10 secondi...");
				//attende 10 secondi tra un tentativo di connessione e il successivo
				sleep(10);
				res = s->connect("192.168.1.100", 5010);
				//res = s->connect("192.168.43.213", 5010);
			}
			ESP_LOGI(tag, "ThreadConnessionePc -- Socket connesso");
	}
	return;
}

//procedura per inviare un messaggio (stringa) al server
bool sendMessage(std::string message){
	int numByteSent;
	do {
		numByteSent = s->send(message);
	} while (numByteSent != message.length());
	ESP_LOGI(tag, "ThreadConnessionePc -- Messaggio inviato con successo");
	return true;
}

//procedura per ricevere un messaggio (stringa) dal socket
int receiveMessage(unsigned char buffer[128], size_t size){
	//ripulisco il buffer di ricezione
	memset(buffer, 0, size * (sizeof buffer[0]) );
	int numByteReceived = s->receive(buffer,size);
	return numByteReceived;
}

void syncClock(){
	long delay = 0;
	long request_timestamp;
	long reply_timestamp;
	long received_timestamp;
	//time_t reply_timestamp;
	for (int i=0; i<4; i++){
		time(&request_timestamp);
		sendMessage("SYNC_CLOCK\n");
		numByteReceived = receiveMessage(bufferReceive,8);
		received_timestamp = bufferReceive[0] | (bufferReceive[1] << 8) | (bufferReceive[2] << 16) | (bufferReceive[3] << 24) | (bufferReceive[4] << 32) | (bufferReceive[5] << 40) | (bufferReceive[6] << 48) | (bufferReceive[7] << 56) ;
		time(&reply_timestamp);

		//reply_timestamp=lwip_ntohl(reply_timestamp);
		delay = delay + (reply_timestamp - request_timestamp);
	}
	//calcolo il delay medio delle 4 richieste
	delay = delay/4;
	//setto il timer dell'ESP
	struct timeval tv;
	tv.tv_sec = received_timestamp + (delay/2);
	tv.tv_usec = 0;
	settimeofday(&tv, NULL);
}

//procedura che gestire il lampeggio del led quando viene richiesta dal server l'IDENTIFICAZIONE
void blinkLed(){
	time_t blink_time_start;
	time_t blink_time;
	time(&blink_time_start);

	do {
		ESP32CPP::GPIO::high(GPIO_NUM_2);
		sleep(1);
		ESP32CPP::GPIO::low(GPIO_NUM_2);
		sleep(1);
		time(&blink_time);
	} while(difftime(blink_time,blink_time_start)<30); //lampeggia per 30 secondi
}
