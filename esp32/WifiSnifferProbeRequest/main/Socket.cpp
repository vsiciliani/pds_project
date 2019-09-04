/*
 * SocketC.cpp
 *
 *  Created on: 10 apr 2019
 *      Author: SicilianiVi
 */

#include "Socket.h"

Socket::Socket(const char* serverAddress, int serverPort){

	if (this->socket!=-1) return;

	server.sin_family = AF_INET;
	inet_pton(AF_INET, serverAddress, &server.sin_addr.s_addr);
	server.sin_port = lwip_htons(serverPort);
}

int Socket::connect(){

	int result = 0;
	result = lwip_socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (result < 0) {
		ESP_LOGE("Socket.cpp", "Apertura socket fallita");
		return -1;
	}
	else {
		this->socket = result;
		return lwip_connect(this->socket, (struct sockaddr*) & server, sizeof(struct sockaddr_in));
	}

	//this->socket = lwip_socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	//return lwip_connect(this->socket, (struct sockaddr *)&server, sizeof(struct sockaddr_in));
}

int Socket::send(std::string message){
	return lwip_write(this->socket,message.c_str(),message.size());
}

std::string Socket::receive(){
	char buffer_ric[MAXBUFL]="";
	lwip_recv(this->socket,buffer_ric,MAXBUFL,0);
	return std::string(buffer_ric);
}

void Socket::receiveRaw(){
	memset(this->buffer_ric, 0, MAXBUFL * (sizeof this->buffer_ric[0]) );
	lwip_recv(this->socket,this->buffer_ric,MAXBUFL,0);
}

Socket::~Socket() {
	lwip_close(this->socket); //TODO: implementare regola del 3 (slide 5 pagina 53)
}

Socket& Socket::operator=(const Socket& source) {
	if (this != &source) {
		lwip_close(this->socket);
		this->socket = source.socket;
		this->server = source.server;
	}
	return *this;
}