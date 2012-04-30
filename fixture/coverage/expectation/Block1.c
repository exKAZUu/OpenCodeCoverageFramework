#include "covman.h"
 int main ( ) {
	int i = WriteStatement ( 21 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
	WriteStatement ( 0 , 0 , 2 ) ;
	if ( WritePredicate ( 13 , 3 , i == 0 ) ) {
		WriteStatement ( 1 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 2 , 0 , 2 ) ;
	if ( WritePredicate ( 17 , 1 , WritePredicate ( 14 , 2 , i <= 0 ) && ( WritePredicate ( 15 , 2 , i >= 0 ) || WritePredicate ( 16 , 2 , i != 0 ) ) ) ) {
		WriteStatement ( 3 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 4 , 0 , 2 ) ;
	switch ( i ) {
		case 0 : WriteStatement ( 5 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 6 , 0 , 2 ) ;
	while ( WritePredicate ( 18 , 3 , i != 0 ) ) {
		WriteStatement ( 7 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 8 , 0 , 2 ) ;
	do {
		WriteStatement ( 9 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	while ( WritePredicate ( 19 , 3 , i != 0 ) ) ;
	WriteStatement ( 10 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 20 , 3 , i < 0 ) ;
	i ++ ) {
		WriteStatement ( 11 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 12 , 0 , 2 ) ;
	return 0 ;
}
