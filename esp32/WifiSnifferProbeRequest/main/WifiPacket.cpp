/*
 * WifiPacket.cpp
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include "WifiPacket.h"

WifiPacket::WifiPacket(void* buff){
	//recupero le metainformazioni sul segnale ricevuto e il suo payload
	wifi_promiscuous_pkt_t *ppkt = (wifi_promiscuous_pkt_t *)buff;
	metadata = ppkt->rx_ctrl;
	//recupero il payload e il mac address di source e destination
	wifi_packet *pacchetto = (wifi_packet *)ppkt->payload;
	header = pacchetto->header;
	payload = pacchetto->payload;
}

short int WifiPacket::getSignalStrength(){
	return metadata.rssi;
}

unsigned short int WifiPacket::getTypeMessage(){
	return header.type;
}

unsigned short int WifiPacket::getSubTypeMessage(){
	return header.subtype;
}

unsigned short int WifiPacket::getElementId(){
	return payload.element_id;
}

unsigned short int WifiPacket::getSSIDLength(){
	return payload.SSIDlength;
}

std::string WifiPacket::getSourceMacAddress(){
	char addr[18];
	sprintf (addr, "%02x:%02x:%02x:%02x:%02x:%02x", header.addr2[0], header.addr2[1],
			header.addr2[2], header.addr2[3], header.addr2[4], header.addr2[5]);
	return std::string(addr);
}

std::string WifiPacket::getSSID(){

	if (getElementId() !=0  || (getElementId() == 0 && getSSIDLength() == 0)){
		return "";
	} else {
		char ssid[32];
		for (int i=0; i<getSSIDLength() && i<32; i++ ){
			sprintf (ssid+i, "%c", payload.ssid[i]);
		}

		return std::string(ssid);
	}
}

