/*
 * SocketC.h
 *
 *  Created on: 10 apr 2019
 *      Author: SicilianiVi
 */

#include <iostream>
#include <string>
#include <lwip/inet.h>
#include <lwip/sockets.h>
#include "lwip/err.h"
#include "esp_log.h"

#ifndef MAIN_SOCKETC_H_
#define MAIN_SOCKETC_H_

#define MAXBUFL 128

class Socket {
private:
	int socket = -1;
	struct sockaddr_in server;

	char buffer_send[MAXBUFL]="";

public:
	char buffer_ric[MAXBUFL]="";
	Socket(const char* serverAddress, int serverPort);
	int connect();
	int send(std::string message);
	std::string receive();
	void receiveRaw();
	~Socket()
};

#endif /* MAIN_SOCKETC_H_ */
