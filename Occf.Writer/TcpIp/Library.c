#include <stdio.h>
#include <winsock2.h>

static int gSocket = INVALID_SOCKET;

int Initialize() {
	struct sockaddr_in dstAddr;

	// Windows “ÆŽ©‚ÌÝ’è
	WSADATA data;
	WSAStartup(MAKEWORD(2,0), &data);

	memset(&dstAddr, 0, sizeof(dstAddr));
	dstAddr.sin_port = htons(8998);
	dstAddr.sin_family = AF_INET;
	dstAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
 
	gSocket = socket(AF_INET, SOCK_STREAM, 0);

	connect(gSocket, (struct sockaddr *)&dstAddr, sizeof(dstAddr));
	return gSocket != INVALID_SOCKET;
}

void Release() {
	if (gSocket != INVALID_SOCKET) {
		closesocket(gSocket);
	}
	WSACleanup();
}

void Write(int id, unsigned char value)
{
	unsigned char data[5] = {
		((unsigned int)id >> 24) & 0xFF, ((unsigned int)id >> 16) & 0xFF,
		((unsigned int)id >> 8)  & 0xFF, ((unsigned int)id >> 0)  & 0xFF,
		value
	};
	if (gSocket == INVALID_SOCKET && !Initialize()) {
		return;
	}
	send(gSocket, (char *)data, 5, 0);
}

#include "..\OCCF.Writer\WriterWrapper.h"