#include <iostream>
#include <fstream>
#include <sstream>
#include <string>

struct Config {
	char nomeRete [32];
	char key[64];
	std::string serverIp;
	int port;
};

void loadConfig(Config& config) {
	std::ifstream fin("config.txt");
	std::string line;
	while (getline(fin, line)) {
		
		std::istringstream sin(line.substr(line.find("=") + 1)); //Input stream class to operate on strings.
		if (line.find("SSID") != -1)
			sin >> config.nomeRete;
		else if (line.find("key") != -1)
			sin >> config.key;
		else if (line.find("serverIp") != -1)
			sin >> config.serverIp;
		else if (line.find("port") != -1)
			sin >> config.port;
	}
}
