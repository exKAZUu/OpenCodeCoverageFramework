#include "covman.h"
 int main ( ) {
	int i = 0 ;
	WriteStatement ( 0 , 0 ) ;
	if ( WritePredicate ( 16 , 3 , i == 0 ) ) {
		WriteStatement ( 1 , 0 ) ;
		;
	}
	WriteStatement ( 2 , 0 ) ;
	switch ( i ) {
		WriteStatement ( 3 , 0 ) ;
		;
	}
	WriteStatement ( 4 , 0 ) ;
	while ( i != 0 ) {
		WriteStatement ( 5 , 0 ) ;
		;
	}
	WriteStatement ( 6 , 0 ) ;
	do {
		WriteStatement ( 7 , 0 ) ;
		;
	}
	while ( i != 0 ) ;
	WriteStatement ( 8 , 0 ) ;
	for ( i = 0 ;
	i < 0 ;
	i ++ ) {
		WriteStatement ( 9 , 0 ) ;
		;
	}
	WriteStatement ( 10 , 0 ) ;
	if ( WritePredicate ( 17 , 3 , i == 0 ) ) {
	}
	WriteStatement ( 11 , 0 ) ;
	switch ( i ) {
	}
	WriteStatement ( 12 , 0 ) ;
	while ( i != 0 ) {
	}
	WriteStatement ( 13 , 0 ) ;
	do {
	}
	while ( i != 0 ) ;
	WriteStatement ( 14 , 0 ) ;
	for ( i = 0 ;
	i < 0 ;
	i ++ ) {
	}
	WriteStatement ( 15 , 0 ) ;
	return 0 ;
}
