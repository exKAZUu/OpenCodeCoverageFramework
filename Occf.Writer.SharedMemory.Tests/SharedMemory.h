#ifndef WRITER_INTERNAL_H_
#define WRITER_INTERNAL_H_

extern int Initialize(int size);
extern void Release();
extern int Read(int id);
extern void Write(int id, unsigned char value);
extern void WriteStatement(int id, int type, int value);
extern int WritePredicate(int id, int type, int value);

#endif // WRITER_INTERNAL_H_