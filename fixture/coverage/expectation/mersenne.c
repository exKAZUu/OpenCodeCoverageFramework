#include "covman.h"
 int NextPrime ( int n ) ;
int divisor ( int n ) ;
void main ( ) {
	unsigned Mersenne [ 500 ] , S [ 500 ] , T [ 500 ] , quot [ 500 ] , res [ 500 ] ;
	int a [ 500 ] ;
	int p , j , k , t , d , r ;
	WriteStatement ( 0 , 0 , 2 ) ;
	p = 5 ;
	WriteStatement ( 1 , 0 , 2 ) ;
	while ( WritePredicate ( 67 , 3 , p < 525 ) ) {
		WriteStatement ( 2 , 0 , 2 ) ;
		Initialize ( Mersenne ) ;
		WriteStatement ( 3 , 0 , 2 ) ;
		Mersenne [ 0 ] = 1 ;
		WriteStatement ( 4 , 0 , 2 ) ;
		LongLeftShift ( Mersenne , p ) ;
		WriteStatement ( 5 , 0 , 2 ) ;
		Initialize ( a ) ;
		WriteStatement ( 6 , 0 , 2 ) ;
		a [ 0 ] = 1 ;
		WriteStatement ( 7 , 0 , 2 ) ;
		Sub ( Mersenne , a ) ;
		WriteStatement ( 8 , 0 , 2 ) ;
		printf ( "p = %d  \n" , p ) ;
		WriteStatement ( 9 , 0 , 2 ) ;
		Initialize ( S ) ;
		WriteStatement ( 10 , 0 , 2 ) ;
		S [ 0 ] = 4 ;
		WriteStatement ( 11 , 0 , 2 ) ;
		for ( j = 1 ;
		WritePredicate ( 68 , 3 , j < p - 1 ) ;
		j ++ ) {
			WriteStatement ( 12 , 0 , 2 ) ;
			Copy ( S , a ) ;
			WriteStatement ( 13 , 0 , 2 ) ;
			LongMul ( S , a ) ;
			WriteStatement ( 14 , 0 , 2 ) ;
			Initialize ( a ) ;
			WriteStatement ( 15 , 0 , 2 ) ;
			a [ 0 ] = 2 ;
			WriteStatement ( 16 , 0 , 2 ) ;
			Sub ( S , a ) ;
			WriteStatement ( 17 , 0 , 2 ) ;
			LongDiv ( S , Mersenne , quot , res ) ;
			WriteStatement ( 18 , 0 , 2 ) ;
			if ( WritePredicate ( 57 , 3 , DivCheck ( S , Mersenne , quot , res ) == 0 ) ) {
				WriteStatement ( 19 , 0 , 2 ) ;
				printf ( "DivCheck Failed\n" ) ;
				WriteStatement ( 20 , 0 , 2 ) ;
				return ;
			}
			WriteStatement ( 21 , 0 , 2 ) ;
			Copy ( S , T ) ;
			WriteStatement ( 22 , 0 , 2 ) ;
			LongRightShift ( T , p ) ;
			WriteStatement ( 23 , 0 , 2 ) ;
			t = p / 16 ;
			WriteStatement ( 24 , 0 , 2 ) ;
			r = p - 16 * t ;
			WriteStatement ( 25 , 0 , 2 ) ;
			d = Degree ( S ) ;
			WriteStatement ( 26 , 0 , 2 ) ;
			for ( k = d ;
			WritePredicate ( 69 , 3 , k > t ) ;
			k -- ) {
				WriteStatement ( 27 , 0 , 2 ) ;
				S [ k ] = 0 ;
			}
			WriteStatement ( 28 , 0 , 2 ) ;
			S [ t ] = ( S [ t ] << ( 32 - r ) ) >> ( 32 - r ) ;
			WriteStatement ( 29 , 0 , 2 ) ;
			Add ( S , T ) ;
			WriteStatement ( 30 , 0 , 2 ) ;
			k = Compare ( S , Mersenne ) ;
			WriteStatement ( 31 , 0 , 2 ) ;
			if ( WritePredicate ( 60 , 1 , WritePredicate ( 58 , 2 , k == 1 ) || WritePredicate ( 59 , 2 , k == 0 ) ) ) {
				WriteStatement ( 32 , 0 , 2 ) ;
				Sub ( S , Mersenne ) ;
			}
			WriteStatement ( 33 , 0 , 2 ) ;
			k = Compare ( res , S ) ;
			WriteStatement ( 34 , 0 , 2 ) ;
			if ( WritePredicate ( 61 , 3 , k != 0 ) ) {
				WriteStatement ( 35 , 0 , 2 ) ;
				printf ( "Something is wrong\n" ) ;
				WriteStatement ( 36 , 0 , 2 ) ;
				return ;
			}
		}
		WriteStatement ( 37 , 0 , 2 ) ;
		if ( WritePredicate ( 62 , 3 , ZeroCheck ( S ) == 0 ) ) {
			WriteStatement ( 38 , 0 , 2 ) ;
			printf ( "mersenne,   p = %d\n" , p ) ;
		}
		WriteStatement ( 39 , 0 , 2 ) ;
		p = p + 2 ;
		WriteStatement ( 40 , 0 , 2 ) ;
		p = NextPrime ( p ) ;
	}
}
int NextPrime ( int n ) {
	WriteStatement ( 41 , 0 , 2 ) ;
	if ( WritePredicate ( 63 , 3 , n < 3 ) ) {
		WriteStatement ( 42 , 0 , 2 ) ;
		n = 3 ;
	}
	WriteStatement ( 43 , 0 , 2 ) ;
	if ( WritePredicate ( 64 , 3 , n % 2 == 0 ) ) {
		WriteStatement ( 44 , 0 , 2 ) ;
		n ++ ;
	}
	WriteStatement ( 45 , 0 , 2 ) ;
	while ( WritePredicate ( 70 , 3 , n > divisor ( n ) ) ) {
		WriteStatement ( 46 , 0 , 2 ) ;
		n = n + 2 ;
	}
	WriteStatement ( 47 , 0 , 2 ) ;
	return n ;
}
int divisor ( int n ) {
	int s , i ;
	WriteStatement ( 48 , 0 , 2 ) ;
	if ( WritePredicate ( 65 , 3 , n % 2 == 0 ) ) {
		WriteStatement ( 49 , 0 , 2 ) ;
		return 2 ;
	}
	WriteStatement ( 50 , 0 , 2 ) ;
	s = ( int ) sqrt ( n ) ;
	WriteStatement ( 51 , 0 , 2 ) ;
	i = 3 ;
	WriteStatement ( 52 , 0 , 2 ) ;
	while ( WritePredicate ( 71 , 3 , i <= s ) ) {
		WriteStatement ( 53 , 0 , 2 ) ;
		if ( WritePredicate ( 66 , 3 , n % i == 0 ) ) {
			WriteStatement ( 54 , 0 , 2 ) ;
			return i ;
		}
		WriteStatement ( 55 , 0 , 2 ) ;
		i = i + 2 ;
	}
	WriteStatement ( 56 , 0 , 2 ) ;
	return n ;
}
//#define N 500
 //#define M 601
 // log 16 =  1.204119982656
 // Mersenne = 2 の p 乗 - 1
 // Display(Mersenne);
 //  S -> S^2
 //  S -> S^2 - 2
 // n 以上の最小の素数 (>=3) を返す。
 // n の最小の正の約数 (>1) を返す。
