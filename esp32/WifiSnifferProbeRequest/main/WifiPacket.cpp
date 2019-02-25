/*
 * WifiPacket.cpp
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include "WifiPacket.h"
#include <functional>
#include <list>
#include <sys/time.h>


WifiPacket::WifiPacket(void* buff){
	//recupero le metainformazioni sul segnale ricevuto e il suo payload
	wifi_promiscuous_pkt_t *ppkt = (wifi_promiscuous_pkt_t *)buff;
	this->metadata = ppkt->rx_ctrl;
	//recupero il payload e il mac address di source e destination
	wifi_packet *pacchetto = (wifi_packet *)ppkt->payload;
	this->header = pacchetto->header;
	this->payload = pacchetto->payload;
	this->timestamp = std::time(nullptr);
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

std::size_t WifiPacket::getHashCode(){

	//preparazione lista per hash
	//header
	std::list<std::size_t> listaHash;
	std::size_t hash = std::hash<unsigned int>{}(this->header.protocol_v);
	listaHash.push_back(hash);
	hash = std::hash<unsigned short>{}(this->header.type);
	listaHash.push_back(hash);
	hash = std::hash<unsigned short>{}(this->header.flags);
	listaHash.push_back(hash);
	hash = std::hash<unsigned short>{}(this->header.flags);
	listaHash.push_back(hash);
	hash = std::hash<unsigned int>{}(this->header.duration_id);
	listaHash.push_back(hash);
	for (int i=0; i<6;i++) {
		hash = std::hash<unsigned int>{}(this->header.addr1[i]);
		listaHash.push_back(hash);
	}
	for (int i=0; i<6;i++) {
		hash = std::hash<unsigned int>{}(this->header.addr2[i]);
		listaHash.push_back(hash);
	}
	for (int i=0; i<6;i++) {
		hash = std::hash<unsigned int>{}(this->header.addr3[i]);
		listaHash.push_back(hash);
	}
	hash = std::hash<unsigned int>{}(this->header.sequence_ctrl);
	listaHash.push_back(hash);
	//payload
	hash = std::hash<unsigned short>{}(this->payload.element_id);
	listaHash.push_back(hash);
	hash = std::hash<unsigned short>{}(this->payload.SSIDlength);
	listaHash.push_back(hash);
	for (int i=0; i<32;i++) {
		hash = std::hash<unsigned int>{}(this->payload.ssid[i]);
		listaHash.push_back(hash);
	}

	std::list<std::size_t>::iterator itHash;

	hash=0;
	for (itHash= listaHash.begin(); itHash != listaHash.end(); itHash++){
		hash+=*itHash;
	}

	return hash;
}

time_t WifiPacket::getTimestamp(){
	return this->timestamp;
}



