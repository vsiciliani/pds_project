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

#ifndef MAIN_SOCKETC_H_
#define MAIN_SOCKETC_H_

#define MAXBUFL 128

class SocketC {
private:
	int socket = -1;
	struct sockaddr_in server;

	char buffer_send[MAXBUFL]="";

public:
	SocketC(char* serverAddress, int serverPort);
	int connect();
	int send(std::string message);
	std::string receive();
	void close();
};

#endif /* MAIN_SOCKETC_H_ */
