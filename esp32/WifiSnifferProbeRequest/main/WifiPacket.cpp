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
	this->metadata = ppkt->rx_ctrl;
	//recupero il payload e il mac address di source e destination
	wifi_packet *pacchetto = (wifi_packet *)ppkt->payload;
	this->header = pacchetto->header;
	this->payload = pacchetto->payload;
	this->hashCode = 0;
}

short int WifiPacket::getSignalStrength(){
	return this->metadata.rssi;
}

unsigned short int WifiPacket::getTypeMessage(){
	return this->header.type;
}

unsigned short int WifiPacket::getSubTypeMessage(){
	return this->header.subtype;
}

unsigned short int WifiPacket::getElementId(){
	return this->payload.element_id;
}

unsigned short int WifiPacket::getSSIDLength(){
	return this->payload.SSIDlength;
}

std::string WifiPacket::getSourceMacAddress(){
	char addr[18];
	sprintf (addr, "%02x:%02x:%02x:%02x:%02x:%02x", this->header.addr2[0],this-> header.addr2[1],
			this->header.addr2[2], this->header.addr2[3], this->header.addr2[4], this->header.addr2[5]);
	return std::string(addr);
}

std::string WifiPacket::getSSID(){

	if (this->getElementId() !=0  || (this->getElementId() == 0 && this->getSSIDLength() == 0)){
		return "";
	} else {
		char ssid[32];
		for (int i=0; i<getSSIDLength() && i<32; i++ ){
			sprintf (ssid+i, "%c", this->payload.ssid[i]);
		}

		return std::string(ssid);
	}
}

