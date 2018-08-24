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
#include "WifiPacket.h"
#include "PacketInfo.h"
#include "sdkconfig.h"



static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);
static char tag[]="Sniffer-ProbeRequest";

std::list<std::string> listaRecord;

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
		listaRecord.push_back(record.JSONSerializer());

		printf("\n");
	}

}
