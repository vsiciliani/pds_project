/*
 * SocketC.cpp
 *
 *  Created on: 10 apr 2019
 *      Author: SicilianiVi
 */

#include "SocketC.h"

SocketC::SocketC(char* serverAddress, int serverPort){
	//const struct addrinfo hints;
	int result=0;

	//memset(&hints, 0, sizeof(hints));
	//hints.ai_family = AF_INET;
	//hints.ai_socktype = SOCK_STREAM;

	if (this->socket!=-1) return;

	/*result=getaddrinfo(argv[1], argv[2], &hints, &server);
	if (result<0) {
		std::cout << "getaddrinfo() failed";
		return;
	}*/

	/*char ipServer[serverAddress.size() + 1];
	strcpy(ipServer, serverAddress.c_str());*/

	server.sin_family = AF_INET;
	inet_pton(AF_INET, serverAddress, &server.sin_addr.s_addr);
	server.sin_port = lwip_htons(serverPort);

	result = lwip_socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (result<0) {
			std::cout << "socket() failed" << std::endl;
	} else {
		this->socket = result;
	}
}

int SocketC::connect(){
	int result=0;
	result = lwip_connect(this->socket, (struct sockaddr *)&server, sizeof(struct sockaddr_in));
	if (result == -1) {
		std::cout << "connect() failed" << std::endl;
	} else {
		std::cout << "CONNESSIONE RIUSCITA" << std::endl;
	}
	return result;
}

int SocketC::send(std::string message){

	//add convertion from string to char*
	char buffer[message.size() + 1];
	strcpy(buffer, message.c_str());

	size_t lunghezza= strlen(buffer);
	ssize_t b_send=lwip_send(this->socket,buffer,lunghezza,0);
	if (b_send==lunghezza) {
		std::cout << "TRASMISSIONE COMPLETATA" << std::endl;
		return b_send;
	} else {
		std::cout << "TRASMISSIONE ERRATA" << std::endl;
		return -1;
	}
}

std::string SocketC::receive(){
	char buffer_ric[MAXBUFL]="";
	lwip_recv(this->socket,buffer_ric,MAXBUFL,0);
	return std::string(buffer_ric);
}

void SocketC::close(){
	lwip_close(this->socket);
}
