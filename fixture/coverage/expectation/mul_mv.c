#include "covman.h"
 int main ( void ) {
	int i , j ;
	double x [ 3 ] = {
		- 33.0 , 9.0 , 6.0 }
	;
	double a [ 3 ] [ 3 ] = {
		{
			2.0 , 4.0 , 6.0 }
		, {
			3.0 , 8.0 , 7.0 }
		, {
			5.0 , 7.0 , 12345678901234567890.0 }
	}
	;
	double y [ 3 ] ;
	WriteStatement ( 0 , 0 , 2 ) ;
	printf ( "x = \n" ) ;
	WriteStatement ( 1 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 16 , 3 , i < 3 ) ;
	i ++ ) {
		WriteStatement ( 2 , 0 , 2 ) ;
		printf ( "%6.2f\n" , x [ i ] ) ;
	}
	WriteStatement ( 3 , 0 , 2 ) ;
	printf ( "A = \n" ) ;
	WriteStatement ( 4 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 17 , 3 , i < 3 ) ;
	i ++ ) {
		WriteStatement ( 5 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 18 , 3 , j < 3 ) ;
		j ++ ) {
			WriteStatement ( 6 , 0 , 2 ) ;
			printf ( " %6.5f" , a [ i ] [ j ] ) ;
		}
		WriteStatement ( 7 , 0 , 2 ) ;
		printf ( "\n" ) ;
	}
	WriteStatement ( 8 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 19 , 3 , i < 3 ) ;
	i ++ ) {
		WriteStatement ( 9 , 0 , 2 ) ;
		y [ i ] = 0.0 ;
		WriteStatement ( 10 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 20 , 3 , j < 3 ) ;
		j ++ ) {
			WriteStatement ( 11 , 0 , 2 ) ;
			y [ i ] += a [ i ] [ j ] * x [ j ] ;
		}
	}
	WriteStatement ( 12 , 0 , 2 ) ;
	printf ( "A*x = \n" ) ;
	WriteStatement ( 13 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 21 , 3 , i < 3 ) ;
	i ++ ) {
		WriteStatement ( 14 , 0 , 2 ) ;
		printf ( "%6.2f\n" , y [ i ] ) ;
	}
	WriteStatement ( 15 , 0 , 2 ) ;
	return 0 ;
}
/*  print matrix and vector  */ /*  multiplication  */ /*  print answer  */