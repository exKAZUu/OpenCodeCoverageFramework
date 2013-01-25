#include "covman.h"
 int quicksort ( int data [ ] , int left , int right ) {
	int pl , pr , pivot , tmp ;
	WriteStatement ( 0 , 0 , 2 ) ;
	pl = left ;
	WriteStatement ( 1 , 0 , 2 ) ;
	pr = right ;
	WriteStatement ( 2 , 0 , 2 ) ;
	pivot = data [ ( pl + pr ) / 2 ] ;
	WriteStatement ( 3 , 0 , 2 ) ;
	printf ( "---\nquicksort( data, %d, %d )\n" , left , right ) ;
	WriteStatement ( 4 , 0 , 2 ) ;
	while ( WritePredicate ( 36 , 3 , pl <= pr ) ) {
		WriteStatement ( 5 , 0 , 2 ) ;
		while ( WritePredicate ( 37 , 3 , data [ pl ] < pivot ) ) {
			WriteStatement ( 6 , 0 , 2 ) ;
			pl ++ ;
		}
		WriteStatement ( 7 , 0 , 2 ) ;
		printf ( " ** found data[%d]=%d >= %d\n" , pl , data [ pl ] , pivot ) ;
		WriteStatement ( 8 , 0 , 2 ) ;
		while ( WritePredicate ( 38 , 3 , pivot < data [ pr ] ) ) {
			WriteStatement ( 9 , 0 , 2 ) ;
			pr -- ;
		}
		WriteStatement ( 10 , 0 , 2 ) ;
		printf ( " ** found data[%d]=%d <= %d\n" , pr , data [ pr ] , pivot ) ;
		WriteStatement ( 11 , 0 , 2 ) ;
		if ( WritePredicate ( 33 , 3 , pl <= pr ) ) {
			WriteStatement ( 12 , 0 , 2 ) ;
			printf ( " swap: data[%d]=%d <=> data[%d]=%d\n" , pl , data [ pl ] , pr , data [ pr ] ) ;
			WriteStatement ( 13 , 0 , 2 ) ;
			tmp = data [ pl ] ;
			WriteStatement ( 14 , 0 , 2 ) ;
			data [ pl ] = data [ pr ] ;
			WriteStatement ( 15 , 0 , 2 ) ;
			data [ pr ] = tmp ;
			WriteStatement ( 16 , 0 , 2 ) ;
			pl ++ ;
			WriteStatement ( 17 , 0 , 2 ) ;
			pr -- ;
		}
	}
	WriteStatement ( 18 , 0 , 2 ) ;
	if ( WritePredicate ( 34 , 3 , left < pr ) ) {
		WriteStatement ( 19 , 0 , 2 ) ;
		quicksort ( data , left , pr ) ;
	}
	WriteStatement ( 20 , 0 , 2 ) ;
	if ( WritePredicate ( 35 , 3 , pl < right ) ) {
		WriteStatement ( 21 , 0 , 2 ) ;
		quicksort ( data , pl , right ) ;
	}
	WriteStatement ( 22 , 0 , 2 ) ;
	return 0 ;
}
int main ( ) {
	int data [ N ] = {
		5 , 7 , 1 , 4 , 6 , 2 , 3 , 9 , 8 }
	;
	int i ;
	WriteStatement ( 23 , 0 , 2 ) ;
	printf ( "== before ==\n" ) ;
	WriteStatement ( 24 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 39 , 3 , i < N ) ;
	i ++ ) {
		WriteStatement ( 25 , 0 , 2 ) ;
		printf ( "%d " , data [ i ] ) ;
	}
	WriteStatement ( 26 , 0 , 2 ) ;
	printf ( "\n\n" ) ;
	WriteStatement ( 27 , 0 , 2 ) ;
	quicksort ( data , 0 , 8 ) ;
	WriteStatement ( 28 , 0 , 2 ) ;
	printf ( "\n== after ==\n" ) ;
	WriteStatement ( 29 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 40 , 3 , i < N ) ;
	i ++ ) {
		WriteStatement ( 30 , 0 , 2 ) ;
		printf ( "%d " , data [ i ] ) ;
	}
	WriteStatement ( 31 , 0 , 2 ) ;
	printf ( "\n" ) ;
	WriteStatement ( 32 , 0 , 2 ) ;
	return 0 ;
}
