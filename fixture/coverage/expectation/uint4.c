#include "covman.h"
 typedef unsigned long int UINT4 ;
int main ( ) {
	int a = - 1 ;
	UINT4 b ;
	WriteStatement(0,0,2); if ( WritePredicate(3,3, a == 1 ) ) {
		WriteStatement(1,0,2); b = ( UINT4 ) a ;
	}
	WriteStatement(2,0,2); return 0 ;
}
