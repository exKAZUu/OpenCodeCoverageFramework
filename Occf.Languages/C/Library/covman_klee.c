#include <stdio.h>
#include <stdlib.h>
#include <string.h>

static FILE *gFile = NULL;

int Initialize() {
	char name[256];
	strcpy(name, getenv("KTEST_FILE"));
	strcat(name, ".occf_record");
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

// type:
//  Statement            = 0,
//  Decision             = 1,
//  Condition            = 2,
//  DecisionAndCondition = 3,
//  SwitchCase           = 4,
//  TestCase             = 5,

// value:
//  false                = 0
//  true                 = 1
//  statement            = 2

int WritePredicate(int id, int type, int value)
{
	unsigned char v = (value + 1) | (type << 2);
	Write(id, v);
	return value;
}

int WriteStatement(int id, int type, int value)
{
	return WritePredicate(id, type, value);
}

int WriteTestCase(int id, int type, int value)
{
	return WritePredicate(id, type, value);
}