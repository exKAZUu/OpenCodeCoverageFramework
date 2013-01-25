/* File : CoverageWriter.i */
%module CoverageWriter
%{
#include "..\Occf.Writer.UM\File.c"
%}

extern bool WriteTestCase(int id, int type, int value);
extern bool WriteStatement(int id, int type, int value);
extern bool WritePredicate(int id, int type, bool value);
