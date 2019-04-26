/*
 * main.cpp
 *
 *  Created on: 23 ago 2018
 *      Author: SicilianiVi
 */

#include <iostream>
#include <esp_wifi.h>
#include <esp_wifi_types.h>
#include <nvs_flash.h>
#include <esp_log.h>
#include <string>
#include <list>
#include <thread>
#include <mutex>
#include <math.h>
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
std::string receiveMessage();

static char tag[]="Sniffer-ProbeRequest";

std::list<std::string> listaRecord;
std::mutex m;
std::condition_variable cvMinuto;
time_t startWaitTime;
WiFi wifi;
Socket *s;
//memoria inizialmente disponibile
float memorySpace;

//definizione costanti
//std::string wifiSSID = "APAndroid2";
//std::string wifiPassword = "pippopluto";

std::string wifiSSID = "dlink-natale";
std::string wifiPassword = "h7onlgqmo8vcbgjr6qc3hg9v";
int intervalloConnessionePc = 15;

extern "C" {
   void app_main();
}


void app_main() {

	nvs_flash_init();

	//setto il led come output
	ESP32CPP::GPIO::setOutput(GPIO_NUM_2); //GPIO_NUM_2BUILTIN LED

	//connetto il dispositivo alla rete Wifi
	//wifi.connectAP("Vodafone-50650385", "pe7dt3793ae9t7b");
	int result = wifi.connectAP(wifiSSID, wifiPassword);

	while (result != 0){
		ESP_LOGE(tag, "Connessione alla rete WiFi fallita. Nuovo tentativo tra 10 secondi... ");
		sleep(10);
		result = wifi.connectAP(wifiSSID, wifiPassword);
	}

	ESP_LOGI(tag, "Connesso a %s con IP: %s Gateway: %s",wifi.getStaSSID().c_str(), wifi.getStaIp().c_str(), wifi.getStaGateway().c_str());

	//creo il socket
	s=new Socket("192.168.1.100",5010);

	//connetto il socket
	connectSocket();

	//abilito la modalità di attività promiscua
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));
	ESP_LOGI(tag, "Modalita schema promiscua abilitata");

	//ciclo per gestire i messaggi in entrata
	do {
		std::string message = receiveMessage();

		if (message.compare("IDENTIFICA")==0){
			//lancio il thread che si occupa di far lampeggiare il led
			std::thread threadBlinkLed (blinkLed);
			//stacco il thread dal flusso principale
			threadBlinkLed.detach();
		} else if (message.compare("SYNC_CLOCK")==0) {
			//effetto la sincronizzazione dei timestamp
			syncClock();
		} else if (message.compare("START_SEND")==0){
			//setto l'handler che gestisce la ricezione del pacchetto
			ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

			//abilito la modalità di attività promiscua
			//ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));
			//ESP_LOGI(tag, "Modalita schema promiscua abilitata");

			//salvo il timestamp per calcolare il tempo di flush verso il server
			time(&startWaitTime);

			//salvo la memoria a disposizione
			memorySpace = xPortGetFreeHeapSize();

			//prendo il lock per leggere la lista di PacketInfo
			std::unique_lock<std::mutex> ul(m);
			//condition variable sul tempo di attesa per il flush
			cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

			ESP_LOGI(tag, "Invio dati dei pacchetti al server");

			//disabilito la modalità di attività promiscua
			//ESP_ERROR_CHECK(esp_wifi_set_promiscuous(false));
			//ESP_LOGI(tag, "ThreadConnessionePc -- Modalita schema promiscua disabilitata");

			//send dei dati verso il server
			sendMessage(createJSONArray(listaRecord));

			//pulisco la lista di PacketInfo
			listaRecord.clear();

		} else {
			ESP_LOGI(tag, "Ricevuto messaggio non valido");
		}

	} while (true);
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

		//lock sulla scrittura della lista che contiene gli oggetti PacketInfo
		std::lock_guard<std::mutex> l(m);
		listaRecord.push_back(record.JSONSerializer());

		//setto la condition variable
		cvMinuto.notify_one();
		ESP_LOGI(tag, "Pacchetto rilevato ed elaborato");
	}
}

//processo che controlla che tra un flush verso il server e il successivo passi il tempo stabilito
bool checkTimeoutThreadConnessionePc() {
	time_t now;
	time(&now);
	float freeMemory = xPortGetFreeHeapSize();
	float percMemoryAvailable =freeMemory/memorySpace;
	ESP_LOGI(tag, "Percentuale memoria libera: %f", percMemoryAvailable);
	//flusho il buffer verso il server è passato il tempo successivo o se la memoria libera e meno del 10 percento
	if ((difftime(now,startWaitTime)>intervalloConnessionePc) || (percMemoryAvailable < 0.1)) {
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
	buf+="]}//n";
	return buf;
}

//funzione che esegue la connessione al socket se non e' già connesso
void connectSocket(){
	int res = s->connect();

	while (res < 0) {
		ESP_LOGE(tag, "Connessione con il server fallita. Nuovo tentativo tra 10 secondi...");
		//attende 10 secondi tra un tentativo di connessione e il successivo
		sleep(10);
		res = s->connect();
	}
	ESP_LOGI(tag, "Socket connesso");
}

//procedura per inviare un messaggio (stringa) al server
bool sendMessage(std::string message){
	int numByteSent;
	do {
		numByteSent = s->send(message);
	} while (numByteSent != message.length());
	ESP_LOGI(tag, "Messaggio %s inviato con successo", message.c_str());
	return true;
}

//procedura per ricevere un messaggio (stringa) dal socket
std::string receiveMessage(){
	//ripulisco il buffer di ricezione
	std::string messaggio = s->receive();
	ESP_LOGI(tag, "Messaggio ricevuto: %s", messaggio.c_str());
	return messaggio;
}

void syncClock(){
	long delay = 0;
	long request_timestamp;
	long reply_timestamp;
	long received_timestamp;

	for (int i=0; i<4; i++){
		time(&request_timestamp);
		sendMessage("SYNC_CLOCK_START//n");
		s->receiveRaw();

		received_timestamp = 0;
		for (int j=0; j<8; j++){
			received_timestamp += (s->buffer_ric[j] * pow(2, j*8));
		}
		time(&reply_timestamp);

		delay = delay + (reply_timestamp - request_timestamp);
	}
	//calcolo il delay medio delle 4 richieste
	delay = delay/4;
	//setto il timer dell'ESP
	struct timeval tv;
	tv.tv_sec = received_timestamp + (delay/2);
	tv.tv_usec = 0;
	settimeofday(&tv, NULL);
	sendMessage("SYNC_CLOCK_STOP//n");
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
