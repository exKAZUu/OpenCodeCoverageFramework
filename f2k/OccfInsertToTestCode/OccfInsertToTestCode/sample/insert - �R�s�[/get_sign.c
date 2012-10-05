#include <stdlib.h>
#include <stdio.h>
/*
 * First KLEE tutorial: testing a small function
 */

int get_sign(int x) {
  if (x == 0)
     return 0;
  
  if (x < 0)
     return -1;
  else 
     return 1;
} 

int main() {
  int a;
  klee_make_symbolic(&a, sizeof(a), "a");


char *tmp = getenv("KTEST_FILE");
FILE *file = fopen(".successful_test", "a");
fputs(tmp, file);
fputc('\n', file);
fclose(file);

  return get_sign(a);
} 
