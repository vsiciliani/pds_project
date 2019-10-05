/*
 * SocketC.cpp
 *
 *  Created on: 10 apr 2019
 *      Author: SicilianiVi
 */

#include "Socket.h"

Socket::Socket(std::string serverAddress, int serverPort){

	if (this->socket!=-1) return;

	server.sin_family = AF_INET;
	inet_pton(AF_INET, serverAddress.c_str(), &server.sin_addr.s_addr); //converte la stringa del server in binario e la salva nella struttura server
	server.sin_port = lwip_htons(serverPort);

	//setto le impostazioni per il timeout
	tv.tv_sec = 15;
	tv.tv_usec = 0;
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
}

int Socket::send(std::string message){
	fd_set writeset;
	FD_ZERO(&writeset);
	FD_SET(this->socket, &writeset);

	int ret = lwip_select(FD_SETSIZE, NULL, &writeset, NULL, &tv);
	if (ret <= 0) return -1; //ret = 0: select in timeout; ret < 0: select in errore
	if (FD_ISSET(this->socket, &writeset) != 0) {
		int p = lwip_send(this->socket, message.c_str(), message.size(), 0);
		return p;
	}
	else return -1;
}

std::string Socket::receive(int& result, bool timeoutSet){
	char buffer_ric[MAXBUFL]="";
	fd_set readset;
	FD_ZERO(&readset);
	FD_SET(this->socket, &readset);
	
	if (timeoutSet)
		result = lwip_select(FD_SETSIZE, &readset, NULL, NULL, &tv);
	else 
		result = lwip_select(FD_SETSIZE, &readset, NULL, NULL, NULL);

	if (result <= 0) { //result = 0: select in timeout; result < 0: select in errore
		return ""; 
	}
	lwip_recv(this->socket,buffer_ric,MAXBUFL,0);
	return std::string(buffer_ric);
}

void Socket::receiveRaw(int& result){
	fd_set readset;
	FD_ZERO(&readset);
	FD_SET(this->socket, &readset);

	memset(this->buffer_ric, 0, MAXBUFL * (sizeof this->buffer_ric[0]) );
	result = lwip_select(FD_SETSIZE, &readset, NULL, NULL, &tv);
	
	if (result <= 0) { //result = 0: select in timeout; result < 0: select in errore
		return;
	}
	lwip_recv(this->socket,this->buffer_ric,MAXBUFL,0);
}

Socket::~Socket() {
	lwip_close(this->socket);
	ESP_LOGI("Socket.cpp", "Socket con il server chiuso");
}

Socket& Socket::operator=(Socket&& source) {
	if (this != &source) {
		lwip_close(this->socket);
		this->socket = source.socket;
		this->server = source.server;
		source.socket = -1; //resetto il socket nel source per evitare che il dustruttore sull'oggetto source chiuda il socket
	}
	return *this;
}