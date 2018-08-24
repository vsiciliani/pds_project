/*
 * PacketInfo.cpp
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include "PacketInfo.h"
#include "stdio.h"

PacketInfo::PacketInfo(std::string sourceAddress, std::string SSID, short int signalStrength) {
	this->sourceAddress = sourceAddress;
	this->SSID = SSID;
	this->signalStrength= signalStrength;
}

std::string PacketInfo::JSONSerializer(){
	char bufferss[10];
	sprintf (bufferss, "%d", this->signalStrength);
	return "{\"sourceAddress\": \""+ this->sourceAddress + "\", \"SSID\": \"" + this->SSID
			+ "\", \"signalStreanght\": " + std::string(bufferss) +" }";
}


