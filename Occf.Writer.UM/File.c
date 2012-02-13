#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <windows.h>

#define MAX_STR_LENGTH 5000

static FILE *stmtFile; 
static FILE *predFile;
static int initialized;

static int stmtStrLen;
static int predStrLen;

static char stmtStr[256];
static char predStr[256];

static int testVectorId;
static int testPatternId;

int Initialize() {
	testVectorId = 1;
	testPatternId = 1;

	stmtFile = fopen("statement_result.txt", "w"); 
	predFile = fopen("predicate_result.txt", "w");

	if (stmtFile == NULL || predFile == NULL) {
		fprintf(stderr, "Error: failed to open files.");
		return 0;
	}
	fprintf(stmtFile, "$TP;%d;\n", testPatternId);
	fprintf(predFile, "$TP;%d;\n", testPatternId);
	initialized = 1;
	return 1;
}

void Release() {
	if (stmtFile != NULL) {
		fclose(stmtFile);
	}
	if (predFile != NULL) {
		fclose(predFile);
	}
	initialized = 0;
}

int WritePredicate(int id, int type, int value) {
	if (initialized || Initialize()) {
		char *log = value ? "T" : "F";
		predStrLen += sprintf(predStr, "%d:%s;", id, log);
		if (MAX_STR_LENGTH < predStrLen) {
			predStrLen = sprintf(predStr, "\n$BCS;%d;%d:%s;", testVectorId, id, log);
		}
		fprintf(predFile, predStr);
	}
	return value;
}

int WriteStatement(int id, int type, int value) {
	if (initialized || Initialize()) {
		stmtStrLen += sprintf(stmtStr, "%d;", id);
		if (MAX_STR_LENGTH < stmtStrLen) {
			stmtStrLen = sprintf(stmtStr, "\n$SCS;%d;%d;", testVectorId, id);
		}
		fprintf(stmtFile, stmtStr);
	}
	return value;
}

int WriteTestCase(int id, int type, int value) {
	if (initialized || Initialize()) {
		stmtStrLen = sprintf(stmtStr, "\n$SC;%d;", testVectorId);
		fprintf(stmtFile, stmtStr);
	
		predStrLen = sprintf(predStr, "\n$BC;%d;", testVectorId);
		fprintf(predFile, predStr);
	
		testVectorId++;
	}
	return value;
}