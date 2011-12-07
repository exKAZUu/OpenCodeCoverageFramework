#include "SharedMemoryCommon.h"

int Read(int id)
{
	if (gMemory == NULL && !Initialize(1)) {
		return -1;
	}
	return gMemory[id];
}

void Write(int id, unsigned char value)
{
	if (gMemory == NULL && !Initialize(1)) {
		return;
	}
	gMemory[id] |= value;
}

#include "..\OCCF.Writer\WriterWrapper.h"