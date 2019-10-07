/*
 * WifiPacket.h
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */
#include <esp_wifi.h>
#include <string>
#include <functional>
#include <ctime>


#ifndef MAIN_WIFIPACKET_H_
#define MAIN_WIFIPACKET_H_

//definisco il tipo header
struct wifiHeader{
	unsigned short int protocol_v:2;
	unsigned short int type:2;
	unsigned short int subtype:4;
	unsigned short int flags:8;
	uint16_t duration_id;
    uint8_t addr1[6]; /* receiver address */
    uint8_t addr2[6]; /* sender address */
    uint8_t addr3[6]; /* filtering address */
    uint16_t sequence_ctrl;
};

//definisco il tipo payload
struct wifiPayload{
	unsigned short int element_id:8;
	unsigned short int SSIDlength:8;
	uint8_t ssid[32];
    uint8_t payload[0]; /* network data ended with 4 bytes csum (CRC32) DROPPATO */
};

struct wifi_packet{
	wifiHeader header;
    wifiPayload payload;
};

class WifiPacket {
private:
	wifi_pkt_rx_ctrl_t metadata;
	wifiHeader header;
    wifiPayload payload;
    time_t timestamp;
public:
	WifiPacket(void* buff);

	short int getSignalStrength();

	unsigned short int getTypeMessage();
	unsigned short int getSubTypeMessage();
	unsigned short int getElementId();
	unsigned short int getSSIDLength();
	std::string getSourceMacAddress();
	std::string getSSID();
	size_t getHashCode();
	time_t getTimestamp();

};

#endif /* MAIN_WIFIPACKET_H_ */
