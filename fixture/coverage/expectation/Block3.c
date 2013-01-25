#include "covman.h"
 int main ( ) {
	int i = WriteStatement ( 24 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
	WriteStatement ( 0 , 0 , 2 ) ;
	if ( WritePredicate ( 16 , 3 , i == 0 ) ) {
		WriteStatement ( 1 , 0 , 2 ) ;
		;
	}
	WriteStatement ( 2 , 0 , 2 ) ;
	switch ( i ) {
		WriteStatement ( 3 , 0 , 2 ) ;
		;
	}
	WriteStatement ( 4 , 0 , 2 ) ;
	while ( WritePredicate ( 18 , 3 , i != 0 ) ) {
		WriteStatement ( 5 , 0 , 2 ) ;
		;
	}
	WriteStatement ( 6 , 0 , 2 ) ;
	do {
		WriteStatement ( 7 , 0 , 2 ) ;
		;
	}
	while ( WritePredicate ( 19 , 3 , i != 0 ) ) ;
	WriteStatement ( 8 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 20 , 3 , i < 0 ) ;
	i ++ ) {
		WriteStatement ( 9 , 0 , 2 ) ;
		;
	}
	WriteStatement ( 10 , 0 , 2 ) ;
	if ( WritePredicate ( 17 , 3 , i == 0 ) ) {
	}
	WriteStatement ( 11 , 0 , 2 ) ;
	switch ( i ) {
	}
	WriteStatement ( 12 , 0 , 2 ) ;
	while ( WritePredicate ( 21 , 3 , i != 0 ) ) {
	}
	WriteStatement ( 13 , 0 , 2 ) ;
	do {
	}
	while ( WritePredicate ( 22 , 3 , i != 0 ) ) ;
	WriteStatement ( 14 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 23 , 3 , i < 0 ) ;
	i ++ ) {
	}
	WriteStatement ( 15 , 0 , 2 ) ;
	return 0 ;
}
