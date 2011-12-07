#include "covman.h"
 int main ( ) {
	int i = 0 ;
	WriteStatement ( 0 , 0 ) ;
	if ( WritePredicate ( 11 , 3 , i == 0 ) ) {
		WriteStatement ( 1 , 0 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 2 , 0 ) ;
	switch ( i ) {
		case 0 : WriteStatement ( 3 , 0 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 4 , 0 ) ;
	while ( i != 0 ) {
		WriteStatement ( 5 , 0 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 6 , 0 ) ;
	do {
		WriteStatement ( 7 , 0 ) ;
		printf ( "test" ) ;
	}
	while ( i != 0 ) ;
	WriteStatement ( 8 , 0 ) ;
	for ( i = 0 ;
	i < 0 ;
	i ++ ) {
		WriteStatement ( 9 , 0 ) ;
		printf ( "test" ) ;
	}
	WriteStatement ( 10 , 0 ) ;
	return 0 ;
}
