/*
 * PacketInfo.cpp
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include "PacketInfo.h"
#include "stdio.h"

PacketInfo::PacketInfo(std::string sourceAddress, std::string SSID, short int signalStrength, size_t hashCode, time_t timestamp) {
	this->sourceAddress = sourceAddress;
	this->SSID = SSID;
	this->signalStrength= signalStrength;
	this->hashCode= hashCode;
	this->timestamp= timestamp;
}

std::string PacketInfo::JSONSerializer(){
	char signalStrength[10];
	sprintf (signalStrength, "%d", this->signalStrength);
	char hashCode[10];
	sprintf (hashCode, "%d", this->hashCode);
	char timestamp[16];
	sprintf(timestamp, "%lu", this->timestamp);
	return "{\"sourceAddress\": \""+ this->sourceAddress
			+ "\", \"SSID\": \"" + this->SSID
			+ "\", \"signalStrength\": " + std::string(signalStrength)
			+ ", \"hashCode\": \"" + std::string(hashCode)
			+ "\", \"timestamp\": " + std::string(timestamp)
			+" }";
}


