#line 1
#include <stdio.h>
#line 2

#line 3
int main() {      // LN:1
#line 4
#ifdef WINDOWS    // LN:2
#line 5
  return -1;       // LN:3
#line 6
#else             // LN:4
#line 7
  return 0;
#line 8
#endif            // LN:6
#line 9
}                 // LN:7
