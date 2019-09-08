/*
 * main.cpp
 *
 *  Created on: 23 ago 2018
 *      Author: SicilianiVi
 */

#include <iostream>
#include <esp_wifi.h>
#include <esp_wifi_types.h>
#include <esp_event_loop.h>
#include <freertos/event_groups.h>
#include <nvs_flash.h>
#include <esp_log.h>
#include <string>
#include <list>
#include <thread>
#include <mutex>
#include <math.h>
#include <condition_variable>
#include <time.h>
#include "WifiPacket.h"
#include "PacketInfo.h"
#include "Socket.h"
#include "sdkconfig.h"
#include <driver/gpio.h>

//definizione delle costanti
#include "config.cpp"

static char tag[] = "Sniffer-ProbeRequest";

static void wifi_sniffer_packet_handler(void *buff, wifi_promiscuous_pkt_type_t type);
static esp_err_t event_handler(void* ctx, system_event_t* event);
bool checkTimeoutThreadConnessionePc();
void connectSocket();
void blinkLed();
void syncClock();
std::string createJSONArray(std::list<std::string>);
bool sendMessage(std::string message);
std::string receiveMessage();

std::list<std::string> listaRecord;
std::mutex m;
std::condition_variable cvMinuto;
time_t startWaitTime;
Socket *s;
float memorySpace; //memoria inizialmente disponibile

static EventGroupHandle_t wifi_event_group;

extern "C" {
   void app_main();
}

void app_main() {

	nvs_flash_init();

	//setto il led come output
	::gpio_set_direction(GPIO_NUM_2, GPIO_MODE_OUTPUT);

	/* GESTIONE CONNESSIONE WIFI */

	wifi_event_group = xEventGroupCreate();

	tcpip_adapter_init();
	ESP_ERROR_CHECK(esp_event_loop_init(event_handler, NULL));

	wifi_init_config_t cfg = WIFI_INIT_CONFIG_DEFAULT();
	ESP_ERROR_CHECK(esp_wifi_init(&cfg));
	wifi_config_t wifi_config = { 
		.sta = {
			{.ssid = WIFI_SSID},
			{.password = WIFI_PASS}
		}, 
	};
	
	ESP_ERROR_CHECK(esp_wifi_set_mode(WIFI_MODE_STA));
	ESP_ERROR_CHECK(esp_wifi_set_config(ESP_IF_WIFI_STA, &wifi_config));
	ESP_ERROR_CHECK(esp_wifi_start());

	ESP_LOGI(tag, "Inizializzazione completata");
	esp_wifi_connect();
	
	ESP_LOGI(tag, "connect to ap SSID: %s",	WIFI_SSID);

	/* FINE GESTIONE CONNESSIONE WIFI */

	bool flag = true;

	//ciclo per gestire i messaggi in entrata
	do {
		ESP_LOGI(tag, "Apertura socket con il server...");
		//creo il socket
		s = new Socket(SERVER_HOST, SERVER_PORT);
		connectSocket();

		//abilito la modalit� di attivit� promiscua
		ESP_ERROR_CHECK(esp_wifi_set_promiscuous(true));
		ESP_LOGI(tag, "Modalita schema promiscua abilitata");

		flag = true;
		do {	
			std::string message = receiveMessage();

			if (message.compare("IDENTIFICA") == 0) {
				//lancio il thread che si occupa di far lampeggiare il led
				std::thread threadBlinkLed(blinkLed);
				//stacco il thread dal flusso principale
				threadBlinkLed.detach();
			}
			else if (message.compare("SYNC_CLOCK") == 0) {
				//effetto la sincronizzazione dei timestamp
				syncClock();
			}
			else if (message.compare("START_SEND") == 0) {
				
				//setto l'handler che gestisce la ricezione del pacchetto
				ESP_ERROR_CHECK(esp_wifi_set_promiscuous_rx_cb(&wifi_sniffer_packet_handler));

				//salvo il timestamp per calcolare il tempo di flush verso il server
				time(&startWaitTime);

				//salvo la memoria a disposizione
				memorySpace = xPortGetFreeHeapSize();

				//prendo il lock per leggere la lista di PacketInfo
				std::unique_lock<std::mutex> ul(m);
				//condition variable sul tempo di attesa per il flush
				cvMinuto.wait(ul, checkTimeoutThreadConnessionePc);

				ESP_LOGI(tag, "Invio dati dei pacchetti al server");

				//send dei dati verso il server
				sendMessage(createJSONArray(listaRecord));

				//pulisco la lista di PacketInfo
				listaRecord.clear();

			}
			else {
				ESP_LOGI(tag, "Ricevuto messaggio non valido");
				flag = false;
			}
		} while (flag);
		//abilito la modalit� di attivit� promiscua
		ESP_ERROR_CHECK(esp_wifi_set_promiscuous(false));
		ESP_LOGI(tag, "Modalita schema promiscua disattivata");
		delete s;
		ESP_LOGI(tag, "Socket con il server chiuso");
	} while (true);
}

//processo di gestione del pacchetto Wifi sniffato
void wifi_sniffer_packet_handler(void* buff, wifi_promiscuous_pkt_type_t type){

	//controllo che il pacchetto si di tipo MANAGEMENT
	if (type != WIFI_PKT_MGMT)
		//se non � di tipo WIFI_PKT_MGMT non e una probe request e quindi scarto il pacchetto
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
	//flusho il buffer verso il server � passato il tempo successivo o se la memoria libera e meno del 10 percento
	if ((difftime(now,startWaitTime)> INTERVALLO_CONNESSIONE_SERVER) || (percMemoryAvailable < 0.1))
		return true;
	else
		return false;
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

//funzione che esegue la connessione al socket se non e' gi� connesso
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
	ESP_LOGI(tag, "Messaggio inviato al server con successo");
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
		//ESP32CPP::GPIO::high(GPIO_NUM_2);
		//write(GPIO_NUM_2, false);
		::gpio_set_level(GPIO_NUM_2, true);
		sleep(1);
		//ESP32CPP::GPIO::low(GPIO_NUM_2);
		::gpio_set_level(GPIO_NUM_2, false);
		sleep(1);
		time(&blink_time);
	} while(difftime(blink_time,blink_time_start)<30); //lampeggia per 30 secondi
}

static esp_err_t event_handler(void* ctx, system_event_t* event) {
	/* For accessing reason codes in case of disconnection */
	//system_event_info_t* info = &event->event_info;

	switch (event->event_id) {
	case SYSTEM_EVENT_STA_START:
		esp_wifi_connect();
		break;
	case SYSTEM_EVENT_STA_GOT_IP:
		ESP_LOGI(tag, "got ip:%s",
			ip4addr_ntoa(&event->event_info.got_ip.ip_info.ip));
		xEventGroupSetBits(wifi_event_group, BIT0);
		break;
	default:
		break;
	}
	return ESP_OK;
}