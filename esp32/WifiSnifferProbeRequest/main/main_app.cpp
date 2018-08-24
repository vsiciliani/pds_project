/*
 * main.cpp
 *
 *  Created on: 23 ago 2018
 *      Author: SicilianiVi
 */

#include <esp_wifi.h>
#include <nvs_flash.h>
#include <esp_log.h>
#include <string>
#include <list>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <time.h>
#include "WifiPacket.h"
#include "PacketInfo.h"
#include "sdkconfig.h"



static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);
void threadGestioneConnessionePc();
bool checkTimeoutThreadConnessionePc();
static char tag[]="Sniffer-ProbeRequest";

std::list<std::string> listaRecord;
std::mutex m;
std::condition_variable cvMinuto;
time_t startWaitTime;

extern "C" {
   void app_main();
}



void app_main() {

	nvs_flash_init();
	wifi_init_config_t config = WIFI_INIT_CONFIG_DEFAULT();
	ESP_ERROR_CHECK(esp_wifi_init(&config));



	//setto l'ESP32 per funzionare in modalita station
	ESP_ERROR_CHECK(esp_wifi_set_mode(WIFI_MODE_STA));

	//avvio il wifi
	ESP_ERROR_CHECK(esp_wifi_start());

	//ESP_ERROR_CHECK(esp_wifi_set_channel(7,WIFI_SECOND_CHAN_NONE));

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

		PacketInfo record = PacketInfo(pacchetto.getSourceMacAddress(), pacchetto.getSSID(), pacchetto.getSignalStrength());
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
	if (difftime(now,startWaitTime)>60) return true;
	else return false;
}

void threadGestioneConnessionePc(){
	ESP_LOGD(tag, "ThreadConnessionePc -- START THREAD");
	while (true) {
		std::unique_lock<std::mutex> ul(m);
		cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);
		ESP_LOGD(tag, "ThreadConnessionePc -- SONO PASSATI ALMENO 20 SECONDI");
		time(&startWaitTime);
	}
	ESP_LOGD(tag, "ThreadConnessionePc -- END THREAD");
}
