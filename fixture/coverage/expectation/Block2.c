#include "covman.h"
 int main ( ) {
	int i = WriteStatement ( 15 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
	WriteStatement ( 0 , 0 , 2 ) ;
	if ( WritePredicate ( 11 , 3 , i == 0 ) ) {
		WriteStatement ( 1 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 2 , 0 , 2 ) ;
	switch ( i ) {
		case 0 : WriteStatement ( 3 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 4 , 0 , 2 ) ;
	while ( WritePredicate ( 12 , 3 , i != 0 ) ) {
		WriteStatement ( 5 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 6 , 0 , 2 ) ;
	do {
		WriteStatement ( 7 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	while ( WritePredicate ( 13 , 3 , i != 0 ) ) ;
	WriteStatement ( 8 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 14 , 3 , i < 0 ) ;
	i ++ ) {
		WriteStatement ( 9 , 0 , 2 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 10 , 0 , 2 ) ;
	return 0 ;
}
