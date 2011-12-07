/* File : CoverageWriter.i */
%module CoverageWriter
%{
#include "..\OCCF.Writer.SharedMemory\SharedMemory.c"
%}

extern bool WriteStatement(int id, int type, int value);
extern bool WritePredicate(int id, int type, bool value);
