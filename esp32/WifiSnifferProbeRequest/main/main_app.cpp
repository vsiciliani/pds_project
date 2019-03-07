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
#include "WiFiEventHandler.h"
#include "WifiPacket.h"
#include "PacketInfo.h"
#include "Socket.h"
#include "SocketClient.h"
#include "sdkconfig.h"
#include "GPIO.h"


static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);
void threadGestioneConnessionePc();
bool checkTimeoutThreadConnessionePc();
void connectSocket();
void blinkLed();


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

	wifi.connectAP("Vodafone-50650385", "pe7dt3793ae9t7b");
	std::cout << "Connesso a "<<wifi.getStaSSID() << " con IP: "<<wifi.getStaIp()
					  <<" Gateway: "<< wifi.getStaGateway() <<std::endl;

	s = new Socket();

	connectSocket();



	 	/*int res = socket->connect("192.168.1.5", 5010);

		if (res < 0) {
			ESP_LOGD(tag, "ThreadConnessionePc -- Connessione con il server fallita");
		} else {
			ESP_LOGD(tag, "ThreadConnessionePc -- Socket connesso");
		}*/

		unsigned char data[10];

		//CONFIGURAZIONE INIZIALE

		do {
			int numByteReceived = s->receive(data,15);
			ESP_LOGD(tag, "messaggio ricevuto: %s", data);
			if (memcmp(data, "IDENTIFICA",numByteReceived)==0) {
					ESP_LOGD(tag, "ho ricevuto IDENTIFICA");
					std::thread threadBlinkLed (blinkLed);
					threadBlinkLed.detach();
					//ripulisco il buffer
					memset(data, 0, 15 * (sizeof data[0]) );
			} else if (memcmp(data, "CONFOK",numByteReceived)==0){
				break;
			} else { ESP_LOGD(tag, "Ricevuto messaggio non valido"); }
		} while (true);


	//abilito la modalità di attività promiscua
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));

	//setto l'handler che gestisce la ricezione del pacchetto
	ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

	//spostare nella zona SETUP quando sarà pronta
	time(&startWaitTime);

	std::thread threadConnessionePc (threadGestioneConnessionePc);

	threadConnessionePc.join();
	fflush(stdout);

}

void wifi_sniffer_packet_handler(void* buff, wifi_promiscuous_pkt_type_t type){

	//controllo che il pacchetto si di tipo MANAGEMENT
	if (type != WIFI_PKT_MGMT)
		//se non è di tipo WIFI_PKT_MGMT non e una probe request e quindi scarto il pacchetto
		return;

	WifiPacket pacchetto = WifiPacket(buff);

	//controllo ancora che il type sia 0 e quindi il pacchetto sia di tipo MANAGEMENT
	//che il subtype sia 4 e quindi che sia una PROBE REQUEST
	if ((pacchetto.getTypeMessage() == 0) && (pacchetto.getSubTypeMessage() == 4)) {

		PacketInfo record = PacketInfo(pacchetto.getSourceMacAddress(), pacchetto.getSSID(), pacchetto.getSignalStrength(), pacchetto.getHashCode(), pacchetto.getTimestamp());
		ESP_LOGD(tag, "JSON: %s", record.JSONSerializer().c_str());
		std::lock_guard<std::mutex> l(m);
		listaRecord.push_back(record.JSONSerializer());
		cvMinuto.notify_one();
		printf("\n");
	}

}

bool checkTimeoutThreadConnessionePc() {
	time_t now;
	time(&now);
	if (difftime(now,startWaitTime)>20) {
		return true;
	}
	else return false;
}

/** preparo il messaggio da inviare al server*/
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

void threadGestioneConnessionePc(){
	ESP_LOGD(tag, "ThreadConnessionePc -- START THREAD");
	/*Socket *socket = new Socket();
	int res = socket->connect("192.168.1.5", 5010);

	ESP_LOGD(tag, "ThreadConnessionePc -- Socket connesso");
	*/
	while (true) {
		connectSocket();

		std::unique_lock<std::mutex> ul(m);
		cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

		ESP_LOGD(tag, "ThreadConnessionePc -- SONO PASSATI ALMENO 20 SECONDI");

		s->send(createJSONArray(listaRecord));

		listaRecord.clear();

		time(&startWaitTime);
	}

	//socket->close();
	ESP_LOGD(tag, "ThreadConnessionePc -- END THREAD");

}

void connectSocket(){
	int res = s->connect("192.168.1.5", 5010);

	while (res < 0) {
		ESP_LOGD(tag, "ThreadConnessionePc -- Connessione con il server fallita. Nuovo tentativo tra 10 secondi...");
		sleep(10);
		res = s->connect("192.168.1.5", 5010);
	}

	ESP_LOGD(tag, "ThreadConnessionePc -- Socket connesso");
	return;

}

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
	} while(difftime(blink_time,blink_time_start)<30);

}
