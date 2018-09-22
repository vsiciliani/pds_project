/*
 * SocketClient.h
 *
 *  Created on: 26 ago 2018
 *      Author: SicilianiVi
 */

#ifndef MAIN_SOCKETCLIENT_H_
#define MAIN_SOCKETCLIENT_H_

#include<iostream>    //cout
#include<stdio.h> //printf
#include<string.h>    //strlen
#include<string>  //string
#include<lwip/sockets.h>    //socket
#include<lwip/inet.h> //inet_addr
#include<lwip/netdb.h> //hostent

/**
    TCP Client class
*/

class SocketClient
{
private:
    int sock;
    std::string address;
    int port;
    struct sockaddr_in server;

public:
    SocketClient();
    bool conn(std::string, int);
    bool send_data(std::string data);
    std::string receive(int);
};


#endif /* MAIN_SOCKETCLIENT_H_ */
