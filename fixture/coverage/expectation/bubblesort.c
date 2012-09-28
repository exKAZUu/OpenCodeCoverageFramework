#include "covman.h"
 int bubblesort ( int data [ ] ) {
	int i , j , tmp ;
	WriteStatement ( 0 , 0 , 2 ) ;
	for ( i = 1 ;
	WritePredicate ( 22 , 3 , i <= N - 1 ) ;
	i ++ ) {
		WriteStatement ( 1 , 0 , 2 ) ;
		printf ( "---\n%d-th pass\n" , i ) ;
		WriteStatement ( 2 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 23 , 3 , j < N - i ) ;
		j ++ ) {
			WriteStatement ( 3 , 0 , 2 ) ;
			printf ( "[%d %d] => " , data [ j ] , data [ j + 1 ] ) ;
			WriteStatement ( 4 , 0 , 2 ) ;
			if ( WritePredicate ( 21 , 3 , data [ j ] > data [ j + 1 ] ) ) {
				WriteStatement ( 5 , 0 , 2 ) ;
				tmp = data [ j ] ;
				WriteStatement ( 6 , 0 , 2 ) ;
				data [ j ] = data [ j + 1 ] ;
				WriteStatement ( 7 , 0 , 2 ) ;
				data [ j + 1 ] = tmp ;
				WriteStatement ( 8 , 0 , 2 ) ;
				printf ( "[%d %d]\n" , data [ j ] , data [ j + 1 ] ) ;
			}
			else {
				WriteStatement ( 9 , 0 , 2 ) ;
				printf ( "OK\n" ) ;
			}
		}
	}
	WriteStatement ( 10 , 0 , 2 ) ;
	return 0 ;
}
int main ( ) {
	int data [ N ] = {
		5 , 7 , 1 , 4 , 6 , 2 , 3 , 9 , 8 }
	;
	int i ;
	WriteStatement ( 11 , 0 , 2 ) ;
	printf ( "== before ==\n" ) ;
	WriteStatement ( 12 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 24 , 3 , i < N ) ;
	i ++ ) {
		WriteStatement ( 13 , 0 , 2 ) ;
		printf ( "%d " , data [ i ] ) ;
	}
	WriteStatement ( 14 , 0 , 2 ) ;
	printf ( "\n\n" ) ;
	WriteStatement ( 15 , 0 , 2 ) ;
	bubblesort ( data ) ;
	WriteStatement ( 16 , 0 , 2 ) ;
	printf ( "\n== after ==\n" ) ;
	WriteStatement ( 17 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 25 , 3 , i < N ) ;
	i ++ ) {
		WriteStatement ( 18 , 0 , 2 ) ;
		printf ( "%d " , data [ i ] ) ;
	}
	WriteStatement ( 19 , 0 , 2 ) ;
	printf ( "\n" ) ;
	WriteStatement ( 20 , 0 , 2 ) ;
	return 0 ;
}
