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
std::string createJSONArray(std::list<std::string>);
bool sendMessage(std::string message);

static char tag[]="Sniffer-ProbeRequest";

std::list<std::string> listaRecord;
std::mutex m;
std::condition_variable cvMinuto;
time_t startWaitTime;
WiFi wifi;
Socket *s;

extern "C" {
   void app_main();
}


void app_main() {

	nvs_flash_init();

	//setto il led come output
	ESP32CPP::GPIO::setOutput(GPIO_NUM_2); //GPIO_NUM_2BUILTIN LED

	//connetto il dispositivo alla rete Wifi
	wifi.connectAP("Vodafone-50650385", "pe7dt3793ae9t7b");
	std::cout << "Connesso a "<<wifi.getStaSSID() << " con IP: "<<wifi.getStaIp()
					  <<" Gateway: "<< wifi.getStaGateway() <<std::endl;

	//creo il socket
	s = new Socket();

	//connetto il socket
	connectSocket();

	//CONFIGURAZIONE INIZIALE
	//buffer per salvare i messaggi in ingresso
	unsigned char bufferReceive[128];

	//ciclo per gestire i messaggi della configurazione iniziale del dispositivo
	do {
		//ricevo il messaggio
		int numByteReceived = s->receive(bufferReceive,128);
		ESP_LOGD(tag, "messaggio ricevuto: %s", bufferReceive);

		//controllo il contenuto del messaggio
		if (memcmp(bufferReceive, "IDENTIFICA",numByteReceived)==0) {
				//ho ricevuto la richiesta di IDENTIFICAZIONE
				ESP_LOGD(tag, "ho ricevuto IDENTIFICA");
				//lancio il thread che si occupa di far lampeggiare il led
				std::thread threadBlinkLed (blinkLed);
				//stacco il thread dal flusso principale
				threadBlinkLed.detach();
				//ripulisco il buffer
				memset(bufferReceive, 0, 128 * (sizeof bufferReceive[0]) );
		} else if (memcmp(bufferReceive, "CONFOK",numByteReceived)==0){
			//se ricevo CONFOK invio ACK e termino il ciclo della configurazione
			sendMessage("CONFOK ACK\n");
			//s->send();
			break;
		} else { ESP_LOGD(tag, "Ricevuto messaggio non valido"); }
	} while (true);


	//abilito la modalità di attività promiscua
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));

	//setto l'handler che gestisce la ricezione del pacchetto
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

	while (true) {
		//verifico la connessione al socket
		connectSocket();
		//salvo il timestamp per calcolare il tempo di flush verso il server
		time(&startWaitTime);

		//prendo il lock per leggere la lista di PacketInfo
		std::unique_lock<std::mutex> ul(m);
		//condition variable sul tempo di attesa per il flush
		cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

		ESP_LOGD(tag, "ThreadConnessionePc -- Invio dati dei pacchetti al server");
		//send dei dati verso il server
		sendMessage(createJSONArray(listaRecord));
		//s->send(createJSONArray(listaRecord));

		ESP_LOGD(tag, "ThreadConnessionePc -- Dati dei pacchetti inviati con successo");

		//pulisco la lista di PacketInfo
		listaRecord.clear();

	}

	fflush(stdout);

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
		ESP_LOGD(tag, "JSON: %s", record.JSONSerializer().c_str());
		//lock sulla scrittura della lista che contiene gli oggetti PacketInfo
		std::lock_guard<std::mutex> l(m);
		listaRecord.push_back(record.JSONSerializer());
		//setto la condition variable
		cvMinuto.notify_one();
		printf("\n");
	}

}

//processo che controlla che tra un flush verso il server e il successivo passi il tempo stabilito
bool checkTimeoutThreadConnessionePc() {
	time_t now;
	time(&now);
	if (difftime(now,startWaitTime)>20) {
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
	std::cout << "Messaggio inviato: " << buf << std::endl;
	return buf;
}

//funzione che esegue la connessione al socket se non e' già connesso
void connectSocket(){
	if (!s->isValid()){
		int res = s->connect("192.168.1.100", 5010);

			while (res < 0) {
				ESP_LOGD(tag, "ThreadConnessionePc -- Connessione con il server fallita. Nuovo tentativo tra 10 secondi...");
				//attende 10 secondi tra un tentativo di connessione e il successivo
				sleep(10);
				res = s->connect("192.168.1.100", 5010);
			}
			ESP_LOGD(tag, "ThreadConnessionePc -- Socket connesso");
	}

	return;
}

//procedura per inviare un messaggio (stringa) al server
bool sendMessage(std::string message){
	int numByteSent;
	do {
		numByteSent = s->send(message);
	} while (numByteSent != message.length());
	return true;
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
