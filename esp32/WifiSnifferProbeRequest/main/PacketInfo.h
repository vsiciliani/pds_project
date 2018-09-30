/*
 * PacketInfo.h
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include <string>
#include <time.h>

#ifndef MAIN_PACKETINFO_H_
#define MAIN_PACKETINFO_H_

class PacketInfo {
private:
	std::string sourceAddress;
	std::string SSID;
	short int signalStrength;
	size_t hashCode;
	time_t timestamp;
public:
	PacketInfo(std::string sourceAddress, std::string SSID, short int signalStrength, size_t hashCode, time_t timestamp);
	std::string JSONSerializer();
};

#endif /* MAIN_PACKETINFO_H_ */
