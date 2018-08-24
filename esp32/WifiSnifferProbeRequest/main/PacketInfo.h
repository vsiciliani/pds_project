/*
 * PacketInfo.h
 *
 *  Created on: 24 ago 2018
 *      Author: SicilianiVi
 */

#include <string>

#ifndef MAIN_PACKETINFO_H_
#define MAIN_PACKETINFO_H_

class PacketInfo {
private:
	std::string sourceAddress;
	std::string SSID;
	short int signalStrength;
public:
	PacketInfo(std::string sourceAddress, std::string SSID, short int signalStrength);
	std::string JSONSerializer();
};

#endif /* MAIN_PACKETINFO_H_ */
