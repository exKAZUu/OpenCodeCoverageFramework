#include <stdio.h>

static FILE *gFile = NULL;

int Initialize() {
	//char *name = getenv("KTEST_FILE");
	char *name = ".occf_coverage_data";
	gFile = fopen(name, "ab");
	return gFile != NULL;
}

void Release() {
	if (gFile != NULL) {
		fclose(gFile);
	}
}

void Write(int id, unsigned char value)
{
	unsigned char data[5] = {
		((unsigned int)id >> 24) & 0xFF, ((unsigned int)id >> 16) & 0xFF,
		((unsigned int)id >> 8)  & 0xFF, ((unsigned int)id >> 0)  & 0xFF,
		value
	};
	if (gFile == NULL && !Initialize()) {
		return;
	}
	fwrite(data, 1, 5, gFile);
}

#include "..\OCCF.Writer\WriterWrapper.h"