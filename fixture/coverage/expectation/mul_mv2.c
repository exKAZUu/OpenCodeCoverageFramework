#include "covman.h"
 int main ( void ) {
	int i , j , n ;
	double * a , * ai , * x , * y ;
	WriteStatement ( 0 , 0 , 2 ) ;
	printf ( "Dimension : n = " ) ;
	WriteStatement ( 1 , 0 , 2 ) ;
	scanf ( "%d" , & n ) ;
	WriteStatement ( 2 , 0 , 2 ) ;
	a = ( double * ) malloc ( n * n * sizeof ( double ) ) ;
	WriteStatement ( 3 , 0 , 2 ) ;
	if ( WritePredicate ( 48 , 3 , a == NULL ) ) {
		WriteStatement ( 4 , 0 , 2 ) ;
		printf ( "Can't allocate memory.\n" ) ;
		WriteStatement ( 5 , 0 , 2 ) ;
		exit ( 1 ) ;
	}
	WriteStatement ( 6 , 0 , 2 ) ;
	x = ( double * ) malloc ( n * sizeof ( double ) ) ;
	WriteStatement ( 7 , 0 , 2 ) ;
	if ( WritePredicate ( 49 , 3 , x == NULL ) ) {
		WriteStatement ( 8 , 0 , 2 ) ;
		printf ( "Can't allocate memory.\n" ) ;
		WriteStatement ( 9 , 0 , 2 ) ;
		exit ( 1 ) ;
	}
	WriteStatement ( 10 , 0 , 2 ) ;
	y = ( double * ) malloc ( n * sizeof ( double ) ) ;
	WriteStatement ( 11 , 0 , 2 ) ;
	if ( WritePredicate ( 50 , 3 , y == NULL ) ) {
		WriteStatement ( 12 , 0 , 2 ) ;
		printf ( "Can't allocate memory.\n" ) ;
		WriteStatement ( 13 , 0 , 2 ) ;
		exit ( 1 ) ;
	}
	WriteStatement ( 14 , 0 , 2 ) ;
	printf ( "\n" ) ;
	WriteStatement ( 15 , 0 , 2 ) ;
	ai = a ;
	WriteStatement ( 16 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 51 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 17 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 52 , 3 , j < n ) ;
		j ++ ) {
			WriteStatement ( 18 , 0 , 2 ) ;
			printf ( "a[%d][%d] = " , i , j ) ;
			WriteStatement ( 19 , 0 , 2 ) ;
			scanf ( "%lf" , ai + j ) ;
		}
		WriteStatement ( 20 , 0 , 2 ) ;
		ai += n ;
	}
	WriteStatement ( 21 , 0 , 2 ) ;
	printf ( "\n" ) ;
	WriteStatement ( 22 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 53 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 23 , 0 , 2 ) ;
		printf ( "x[%d] = " , i ) ;
		WriteStatement ( 24 , 0 , 2 ) ;
		scanf ( "%lf" , x + i ) ;
	}
	WriteStatement ( 25 , 0 , 2 ) ;
	printf ( "\n  a =\n" ) ;
	WriteStatement ( 26 , 0 , 2 ) ;
	ai = a ;
	WriteStatement ( 27 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 54 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 28 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 55 , 3 , j < n ) ;
		j ++ ) {
			WriteStatement ( 29 , 0 , 2 ) ;
			printf ( "    %.2f" , * ( ai + j ) ) ;
		}
		WriteStatement ( 30 , 0 , 2 ) ;
		ai += n ;
		WriteStatement ( 31 , 0 , 2 ) ;
		printf ( "\n" ) ;
	}
	WriteStatement ( 32 , 0 , 2 ) ;
	printf ( "\n  x =\n" ) ;
	WriteStatement ( 33 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 56 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 34 , 0 , 2 ) ;
		printf ( "    %.2f\n" , * ( x + i ) ) ;
	}
	WriteStatement ( 35 , 0 , 2 ) ;
	ai = a ;
	WriteStatement ( 36 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 57 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 37 , 0 , 2 ) ;
		* ( y + i ) = 0.0 ;
		WriteStatement ( 38 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 58 , 3 , j < n ) ;
		j ++ ) {
			WriteStatement ( 39 , 0 , 2 ) ;
			* ( y + i ) += * ( ai + j ) * * ( x + j ) ;
		}
		WriteStatement ( 40 , 0 , 2 ) ;
		ai += n ;
	}
	WriteStatement ( 41 , 0 , 2 ) ;
	printf ( "\n  a*x = \n" ) ;
	WriteStatement ( 42 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 59 , 3 , i < n ) ;
	i ++ ) {
		WriteStatement ( 43 , 0 , 2 ) ;
		printf ( "    %.2f\n" , * ( y + i ) ) ;
	}
	WriteStatement ( 44 , 0 , 2 ) ;
	free ( a ) ;
	WriteStatement ( 45 , 0 , 2 ) ;
	free ( x ) ;
	WriteStatement ( 46 , 0 , 2 ) ;
	free ( y ) ;
	WriteStatement ( 47 , 0 , 2 ) ;
	return 0 ;
}
/*  input dimension n  */ /*  allocate memory for A, x, and y  */ /*  input data of matrix A  */ /*  input data of vector x  */ /*  print matrix A  */ /*  print vector x  */ /*  calculate y = A*x  */ /*  print answer  */ /*  free memory  */