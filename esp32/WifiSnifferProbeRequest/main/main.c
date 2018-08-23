/* Hello World Example

   This example code is in the Public Domain (or CC0 licensed, at your option.)

   Unless required by applicable law or agreed to in writing, this
   software is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
   CONDITIONS OF ANY KIND, either express or implied.
*/
#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_system.h"
#include "esp_spi_flash.h"
#include "esp_wifi.h"
#include "nvs_flash.h"

//definisco il tipo che contiene le informazioni sul frame control
typedef struct {
    unsigned protocol_v:2;
    unsigned type:2;
    unsigned subtype:4;
    unsigned flags:8;
} frame_ctrl;

//definisco il tipo header
typedef struct {
	frame_ctrl frame_ctrl;
	//uint16_t duration_id; //dovrebbe esserci ma se lo metto mi slitta tutto di 2 byte
    uint8_t addr1[6]; /* receiver address */
    uint8_t addr2[6]; /* sender address */
    uint8_t addr3[6]; /* filtering address */
    uint16_t sequence_ctrl;
    //uint8_t addr4[6]; /* optional */
} wifi_packet_header;

//definito il tipo payload
typedef struct {
	unsigned short int element_id:8;
	unsigned short int length:8;
	uint8_t ssid[32];
    uint8_t payload[0]; /* network data ended with 4 bytes csum (CRC32) */
} wifi_payload;

//definito il tipo pacchetto
typedef struct {
    wifi_packet_header header;
    wifi_payload payload; /* network data ended with 4 bytes csum (CRC32) */
} wifi_packet;


static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);

void app_main()
{

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
		//se non è di tipo WIFI_PKT_MGMT non e una probe request
		//quindi scarto il pacchetto
		return;

	//recupero le metainformazioni sul segnale ricevuto e il suo payload
	wifi_promiscuous_pkt_t *ppkt = (wifi_promiscuous_pkt_t *)buff;

	//recupero il payload e il mac address di source e destination
	wifi_packet *pacchetto = (wifi_packet *)ppkt->payload;
	wifi_packet_header header = pacchetto->header;


	//controllo ancora che il type sia 0 e quindi il pacchetto sia di tipo MANAGEMENT
	//e inoltre controllo che il subtype sia 4 e quindi che sia una PROBE REQUEST
	if ((header.frame_ctrl.type == 0) && (header.frame_ctrl.subtype == 4)) {

		//printf("Protocol version: %d \n", header.frame_ctrl.protocol_v);
		//printf("Type: %u\n",header.frame_ctrl.type);
		//printf("Subtype: %u\n",header.frame_ctrl.subtype);
		//printf("Channel: %02d\n",ppkt->rx_ctrl.channel);
		printf("Forza segnale: %02d\n", ppkt->rx_ctrl.rssi);
		//printf("Destination address: %02x:%02x:%02x:%02x:%02x:%02x\n", header.addr1[0],header.addr1[1],header.addr1[2],
				//header.addr1[3],header.addr1[4],header.addr1[5]);
		printf("Source address: %02x:%02x:%02x:%02x:%02x:%02x\n", header.addr2[0],header.addr2[1],header.addr2[2],
				header.addr2[3],header.addr2[4],header.addr2[5]);
		//printf("Payload: \n");

		wifi_payload payload_pacchetto = pacchetto->payload;
		//printf("Element ID: %hu \n", payload_pacchetto.element_id);
		//printf("Length: %hu\n",payload_pacchetto.length);
		if (payload_pacchetto.length == 0){
			printf("SSID non presente\n");
		} else {
			printf("SSID: ");
			for (int i=0; i<payload_pacchetto.length && i<32; i++ ){
				printf("%c", payload_pacchetto.ssid[i]);
			}
			printf("\n");
		}

		//for (int i=0; i<ppkt->rx_ctrl.sig_len;i++) {

			//printf("%x ",pacchetto->payload[i]);
		//}
		printf("\n");
	}

}
