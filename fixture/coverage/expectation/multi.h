#include "covman.h"
 void Add ( unsigned a [ ] , unsigned b [ ] ) ;
void Sub ( unsigned a [ ] , unsigned b [ ] ) ;
void Mul ( unsigned a [ ] , unsigned d ) ;
void LongMul ( unsigned a [ ] , unsigned b [ ] ) ;
unsigned Div ( unsigned a [ ] , unsigned d ) ;
void LongDiv ( unsigned a [ ] , unsigned b [ ] , unsigned quot [ ] , unsigned res [ ] ) ;
int DivCheck ( unsigned a [ ] , unsigned b [ ] , unsigned quot [ ] , unsigned res [ ] ) ;
int Compare ( unsigned a [ ] , unsigned b [ ] ) ;
void LeftShift ( unsigned a [ ] , int sh ) ;
void LongLeftShift ( unsigned a [ ] , int sh ) ;
void RightShift ( unsigned a [ ] , int sh ) ;
void LongRightShift ( unsigned a [ ] , int sh ) ;
void Initialize ( unsigned c [ ] ) ;
void Copy ( unsigned a [ ] , unsigned b [ ] ) ;
int ZeroCheck ( unsigned a [ ] ) ;
int Degree ( unsigned a [ ] ) ;
void ToDecimal ( unsigned [ ] , unsigned w [ ] ) ;
void ToHex ( unsigned w [ ] , unsigned a [ ] ) ;
unsigned ToHexDiv ( unsigned z [ ] ) ;
int ToHexZeroCheck ( unsigned z [ ] ) ;
void Display ( unsigned a [ ] ) ;
void DisplayHex ( unsigned a [ ] ) ;
void Add ( unsigned a [ ] , unsigned b [ ] ) {
	int i , j , s ;
	unsigned x ;
	WriteStatement ( 0 , 0 , 2 ) ;
	s = Degree ( b ) ;
	WriteStatement ( 1 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 254 , 3 , i <= s ) ;
	i ++ ) {
		WriteStatement ( 2 , 0 , 2 ) ;
		x = a [ i ] + b [ i ] ;
		WriteStatement ( 3 , 0 , 2 ) ;
		if ( WritePredicate ( 228 , 3 , x <= 0xffff ) ) {
			WriteStatement ( 4 , 0 , 2 ) ;
			a [ i ] = x ;
		}
		else {
			WriteStatement ( 5 , 0 , 2 ) ;
			a [ i ] = x & 0xffff ;
			WriteStatement ( 6 , 0 , 2 ) ;
			j = i + 1 ;
			WriteStatement ( 7 , 0 , 2 ) ;
			while ( WritePredicate ( 255 , 3 , a [ j ] == 0xffff ) ) {
				WriteStatement ( 8 , 0 , 2 ) ;
				a [ j ++ ] = 0 ;
			}
			WriteStatement ( 9 , 0 , 2 ) ;
			a [ j ] ++ ;
		}
	}
}
void Sub ( unsigned a [ ] , unsigned b [ ] ) {
	int i , j , s ;
	WriteStatement ( 10 , 0 , 2 ) ;
	s = Degree ( b ) ;
	WriteStatement ( 11 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 256 , 3 , i <= s ) ;
	i ++ ) {
		WriteStatement ( 12 , 0 , 2 ) ;
		if ( WritePredicate ( 229 , 3 , a [ i ] >= b [ i ] ) ) {
			WriteStatement ( 13 , 0 , 2 ) ;
			a [ i ] = a [ i ] - b [ i ] ;
		}
		else {
			WriteStatement ( 14 , 0 , 2 ) ;
			a [ i ] = ( 0x10000 + a [ i ] ) - b [ i ] ;
			WriteStatement ( 15 , 0 , 2 ) ;
			j = i + 1 ;
			WriteStatement ( 16 , 0 , 2 ) ;
			while ( WritePredicate ( 257 , 3 , a [ j ] == 0 ) ) {
				WriteStatement ( 17 , 0 , 2 ) ;
				a [ j ++ ] = 0xffff ;
			}
			WriteStatement ( 18 , 0 , 2 ) ;
			a [ j ] -- ;
		}
	}
}
void Mul ( unsigned a [ ] , unsigned d ) {
	int i , s ;
	unsigned x , q ;
	WriteStatement ( 19 , 0 , 2 ) ;
	s = Degree ( a ) ;
	WriteStatement ( 20 , 0 , 2 ) ;
	q = 0 ;
	WriteStatement ( 21 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 258 , 3 , i <= s + 1 ) ;
	i ++ ) {
		WriteStatement ( 22 , 0 , 2 ) ;
		x = a [ i ] * d + q ;
		WriteStatement ( 23 , 0 , 2 ) ;
		a [ i ] = x & 0xffff ;
		WriteStatement ( 24 , 0 , 2 ) ;
		q = x >> 16 ;
	}
}
void LongMul ( unsigned a [ ] , unsigned b [ ] ) {
	int i , j , k ;
	int s , t ;
	unsigned x , q , c [ 500 ] ;
	WriteStatement ( 25 , 0 , 2 ) ;
	s = Degree ( a ) ;
	WriteStatement ( 26 , 0 , 2 ) ;
	t = Degree ( b ) ;
	WriteStatement ( 27 , 0 , 2 ) ;
	Initialize ( c ) ;
	WriteStatement ( 28 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 259 , 3 , i <= s ) ;
	i ++ ) {
		WriteStatement ( 29 , 0 , 2 ) ;
		for ( j = 0 ;
		WritePredicate ( 260 , 3 , j <= t ) ;
		j ++ ) {
			WriteStatement ( 30 , 0 , 2 ) ;
			x = a [ i ] * b [ j ] ;
			WriteStatement ( 31 , 0 , 2 ) ;
			k = i + j ;
			WriteStatement ( 32 , 0 , 2 ) ;
			while ( WritePredicate ( 261 , 3 , x > 0 ) ) {
				WriteStatement ( 33 , 0 , 2 ) ;
				q = c [ k ] + x ;
				WriteStatement ( 34 , 0 , 2 ) ;
				c [ k ] = q & 0xffff ;
				WriteStatement ( 35 , 0 , 2 ) ;
				x = q >> 16 ;
				WriteStatement ( 36 , 0 , 2 ) ;
				k ++ ;
			}
		}
	}
	WriteStatement ( 37 , 0 , 2 ) ;
	Copy ( c , a ) ;
}
unsigned Div ( unsigned a [ ] , unsigned d ) {
	int i , x , q ;
	unsigned res ;
	WriteStatement ( 38 , 0 , 2 ) ;
	res = 0 ;
	WriteStatement ( 39 , 0 , 2 ) ;
	i = Degree ( a ) ;
	WriteStatement ( 40 , 0 , 2 ) ;
	while ( WritePredicate ( 262 , 3 , i >= 0 ) ) {
		WriteStatement ( 41 , 0 , 2 ) ;
		x = ( res << 16 ) + a [ i ] ;
		WriteStatement ( 42 , 0 , 2 ) ;
		q = x / d ;
		WriteStatement ( 43 , 0 , 2 ) ;
		res = x - q * d ;
		WriteStatement ( 44 , 0 , 2 ) ;
		a [ i ] = q ;
		WriteStatement ( 45 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 46 , 0 , 2 ) ;
	return res ;
}
void LongDiv ( unsigned a [ ] , unsigned b [ ] , unsigned quot [ ] , unsigned res [ ] ) {
	int s , t , k , sh ;
	int i ;
	unsigned x , D , Q ;
	unsigned c [ 500 ] , d [ 500 ] ;
	WriteStatement ( 47 , 0 , 2 ) ;
	Copy ( a , res ) ;
	WriteStatement ( 48 , 0 , 2 ) ;
	Initialize ( quot ) ;
	WriteStatement ( 49 , 0 , 2 ) ;
	if ( WritePredicate ( 230 , 3 , Compare ( res , b ) == - 1 ) ) {
		WriteStatement ( 50 , 0 , 2 ) ;
		return ;
	}
	WriteStatement ( 51 , 0 , 2 ) ;
	t = Degree ( b ) ;
	WriteStatement ( 52 , 0 , 2 ) ;
	sh = 0 ;
	WriteStatement ( 53 , 0 , 2 ) ;
	x = b [ t ] ;
	WriteStatement ( 54 , 0 , 2 ) ;
	while ( WritePredicate ( 263 , 3 , ( x & ( 0x8000 ) ) == 0 ) ) {
		WriteStatement ( 55 , 0 , 2 ) ;
		x = x << 1 ;
		WriteStatement ( 56 , 0 , 2 ) ;
		sh ++ ;
	}
	WriteStatement ( 57 , 0 , 2 ) ;
	LeftShift ( res , sh ) ;
	WriteStatement ( 58 , 0 , 2 ) ;
	LeftShift ( b , sh ) ;
	WriteStatement ( 59 , 0 , 2 ) ;
	while ( WritePredicate ( 264 , 3 , 1 ) ) {
		WriteStatement ( 60 , 0 , 2 ) ;
		s = Degree ( res ) ;
		WriteStatement ( 61 , 0 , 2 ) ;
		if ( WritePredicate ( 231 , 3 , res [ s ] >= b [ t ] ) ) {
			WriteStatement ( 62 , 0 , 2 ) ;
			Initialize ( c ) ;
			WriteStatement ( 63 , 0 , 2 ) ;
			for ( i = 0 ;
			WritePredicate ( 265 , 3 , i <= t ) ;
			i ++ ) {
				WriteStatement ( 64 , 0 , 2 ) ;
				c [ s - t + i ] = b [ i ] ;
			}
			WriteStatement ( 65 , 0 , 2 ) ;
			k = Compare ( res , c ) ;
			WriteStatement ( 66 , 0 , 2 ) ;
			if ( WritePredicate ( 234 , 1 , WritePredicate ( 232 , 2 , k == 1 ) || WritePredicate ( 233 , 2 , k == 0 ) ) ) {
				WriteStatement ( 67 , 0 , 2 ) ;
				quot [ s - t ] = 1 ;
				WriteStatement ( 68 , 0 , 2 ) ;
				Sub ( res , c ) ;
			}
			else {
				WriteStatement ( 69 , 0 , 2 ) ;
				Initialize ( c ) ;
				WriteStatement ( 70 , 0 , 2 ) ;
				Initialize ( d ) ;
				WriteStatement ( 71 , 0 , 2 ) ;
				for ( i = 0 ;
				WritePredicate ( 266 , 3 , i <= t ) ;
				i ++ ) {
					WriteStatement ( 72 , 0 , 2 ) ;
					c [ s - t - 1 + i ] = b [ i ] ;
				}
				WriteStatement ( 73 , 0 , 2 ) ;
				Copy ( c , d ) ;
				WriteStatement ( 74 , 0 , 2 ) ;
				D = 0xffff ;
				WriteStatement ( 75 , 0 , 2 ) ;
				Mul ( c , D ) ;
				WriteStatement ( 76 , 0 , 2 ) ;
				while ( WritePredicate ( 267 , 3 , 1 ) ) {
					WriteStatement ( 77 , 0 , 2 ) ;
					k = Compare ( res , c ) ;
					WriteStatement ( 78 , 0 , 2 ) ;
					if ( WritePredicate ( 237 , 1 , WritePredicate ( 235 , 2 , k == 1 ) || WritePredicate ( 236 , 2 , k == 0 ) ) ) {
						WriteStatement ( 79 , 0 , 2 ) ;
						break ;
					}
					WriteStatement ( 80 , 0 , 2 ) ;
					Sub ( c , d ) ;
					WriteStatement ( 81 , 0 , 2 ) ;
					D -- ;
				}
				WriteStatement ( 82 , 0 , 2 ) ;
				quot [ s - t - 1 ] = D ;
				WriteStatement ( 83 , 0 , 2 ) ;
				Sub ( res , c ) ;
			}
		}
		else {
			WriteStatement ( 84 , 0 , 2 ) ;
			x = ( res [ s ] << 16 ) + res [ s - 1 ] ;
			WriteStatement ( 85 , 0 , 2 ) ;
			Q = x / b [ t ] ;
			WriteStatement ( 86 , 0 , 2 ) ;
			Initialize ( c ) ;
			WriteStatement ( 87 , 0 , 2 ) ;
			Initialize ( d ) ;
			WriteStatement ( 88 , 0 , 2 ) ;
			for ( i = 0 ;
			WritePredicate ( 268 , 3 , i <= t ) ;
			i ++ ) {
				WriteStatement ( 89 , 0 , 2 ) ;
				c [ s - t - 1 + i ] = b [ i ] ;
			}
			WriteStatement ( 90 , 0 , 2 ) ;
			Copy ( c , d ) ;
			WriteStatement ( 91 , 0 , 2 ) ;
			Mul ( c , Q ) ;
			WriteStatement ( 92 , 0 , 2 ) ;
			while ( WritePredicate ( 269 , 3 , 1 ) ) {
				WriteStatement ( 93 , 0 , 2 ) ;
				k = Compare ( res , c ) ;
				WriteStatement ( 94 , 0 , 2 ) ;
				if ( WritePredicate ( 240 , 1 , WritePredicate ( 238 , 2 , k == 1 ) || WritePredicate ( 239 , 2 , k == 0 ) ) ) {
					WriteStatement ( 95 , 0 , 2 ) ;
					break ;
				}
				WriteStatement ( 96 , 0 , 2 ) ;
				Sub ( c , d ) ;
				WriteStatement ( 97 , 0 , 2 ) ;
				Q -- ;
			}
			WriteStatement ( 98 , 0 , 2 ) ;
			quot [ s - t - 1 ] = Q ;
			WriteStatement ( 99 , 0 , 2 ) ;
			Sub ( res , c ) ;
		}
		WriteStatement ( 100 , 0 , 2 ) ;
		k = Compare ( res , b ) ;
		WriteStatement ( 101 , 0 , 2 ) ;
		if ( WritePredicate ( 241 , 3 , k == - 1 ) ) {
			WriteStatement ( 102 , 0 , 2 ) ;
			break ;
		}
	}
	WriteStatement ( 103 , 0 , 2 ) ;
	RightShift ( res , sh ) ;
	WriteStatement ( 104 , 0 , 2 ) ;
	RightShift ( b , sh ) ;
}
int DivCheck ( unsigned a [ ] , unsigned b [ ] , unsigned quot [ ] , unsigned res [ ] ) {
	unsigned c [ 500 ] ;
	WriteStatement ( 105 , 0 , 2 ) ;
	Copy ( b , c ) ;
	WriteStatement ( 106 , 0 , 2 ) ;
	LongMul ( c , quot ) ;
	WriteStatement ( 107 , 0 , 2 ) ;
	Add ( c , res ) ;
	WriteStatement ( 108 , 0 , 2 ) ;
	if ( WritePredicate ( 242 , 3 , Compare ( a , c ) == 0 ) ) {
		WriteStatement ( 109 , 0 , 2 ) ;
		return 1 ;
	}
	WriteStatement ( 110 , 0 , 2 ) ;
	return 0 ;
}
int Compare ( unsigned a [ ] , unsigned b [ ] ) {
	int s , t ;
	WriteStatement ( 111 , 0 , 2 ) ;
	s = Degree ( a ) ;
	WriteStatement ( 112 , 0 , 2 ) ;
	t = Degree ( b ) ;
	WriteStatement ( 113 , 0 , 2 ) ;
	if ( WritePredicate ( 243 , 3 , s < t ) ) {
		WriteStatement ( 114 , 0 , 2 ) ;
		return - 1 ;
	}
	WriteStatement ( 115 , 0 , 2 ) ;
	if ( WritePredicate ( 244 , 3 , s > t ) ) {
		WriteStatement ( 116 , 0 , 2 ) ;
		return 1 ;
	}
	WriteStatement ( 117 , 0 , 2 ) ;
	while ( WritePredicate ( 272 , 1 , WritePredicate ( 270 , 2 , s > 0 ) && WritePredicate ( 271 , 2 , a [ s ] == b [ s ] ) ) ) {
		WriteStatement ( 118 , 0 , 2 ) ;
		s -- ;
	}
	WriteStatement ( 119 , 0 , 2 ) ;
	if ( WritePredicate ( 245 , 3 , a [ s ] < b [ s ] ) ) {
		WriteStatement ( 120 , 0 , 2 ) ;
		return - 1 ;
	}
	WriteStatement ( 121 , 0 , 2 ) ;
	if ( WritePredicate ( 246 , 3 , a [ s ] > b [ s ] ) ) {
		WriteStatement ( 122 , 0 , 2 ) ;
		return 1 ;
	}
	WriteStatement ( 123 , 0 , 2 ) ;
	return 0 ;
}
void LeftShift ( unsigned a [ ] , int sh ) {
	int i , x ;
	WriteStatement ( 124 , 0 , 2 ) ;
	if ( WritePredicate ( 247 , 3 , sh == 0 ) ) {
		WriteStatement ( 125 , 0 , 2 ) ;
		return ;
	}
	WriteStatement ( 126 , 0 , 2 ) ;
	i = Degree ( a ) ;
	WriteStatement ( 127 , 0 , 2 ) ;
	x = a [ i ] << sh ;
	WriteStatement ( 128 , 0 , 2 ) ;
	if ( WritePredicate ( 248 , 3 , x > 0xffff ) ) {
		WriteStatement ( 129 , 0 , 2 ) ;
		a [ i + 1 ] |= ( x >> 16 ) ;
	}
	WriteStatement ( 130 , 0 , 2 ) ;
	a [ i ] = x & 0xffff ;
	WriteStatement ( 131 , 0 , 2 ) ;
	i -- ;
	WriteStatement ( 132 , 0 , 2 ) ;
	while ( WritePredicate ( 273 , 3 , i >= 0 ) ) {
		WriteStatement ( 133 , 0 , 2 ) ;
		x = a [ i ] << sh ;
		WriteStatement ( 134 , 0 , 2 ) ;
		a [ i + 1 ] |= ( x >> 16 ) ;
		WriteStatement ( 135 , 0 , 2 ) ;
		a [ i ] = x & 0xffff ;
		WriteStatement ( 136 , 0 , 2 ) ;
		i -- ;
	}
}
void LongLeftShift ( unsigned a [ ] , int sh ) {
	int i , u ;
	unsigned temp [ 500 ] ;
	WriteStatement ( 137 , 0 , 2 ) ;
	if ( WritePredicate ( 249 , 3 , sh < 16 ) ) {
		WriteStatement ( 138 , 0 , 2 ) ;
		LeftShift ( a , sh ) ;
	}
	else {
		WriteStatement ( 139 , 0 , 2 ) ;
		u = sh / 16 ;
		WriteStatement ( 140 , 0 , 2 ) ;
		sh = sh - u * 16 ;
		WriteStatement ( 141 , 0 , 2 ) ;
		i = Degree ( a ) ;
		WriteStatement ( 142 , 0 , 2 ) ;
		Initialize ( temp ) ;
		WriteStatement ( 143 , 0 , 2 ) ;
		while ( WritePredicate ( 274 , 3 , i >= 0 ) ) {
			WriteStatement ( 144 , 0 , 2 ) ;
			temp [ i + u ] = a [ i ] ;
			WriteStatement ( 145 , 0 , 2 ) ;
			i -- ;
		}
		WriteStatement ( 146 , 0 , 2 ) ;
		Copy ( temp , a ) ;
		WriteStatement ( 147 , 0 , 2 ) ;
		LeftShift ( a , sh ) ;
	}
}
void RightShift ( unsigned a [ ] , int sh ) {
	int i , j , x ;
	WriteStatement ( 148 , 0 , 2 ) ;
	if ( WritePredicate ( 250 , 3 , sh == 0 ) ) {
		WriteStatement ( 149 , 0 , 2 ) ;
		return ;
	}
	WriteStatement ( 150 , 0 , 2 ) ;
	i = Degree ( a ) ;
	WriteStatement ( 151 , 0 , 2 ) ;
	i = 500 - 1 ;
	WriteStatement ( 152 , 0 , 2 ) ;
	while ( WritePredicate ( 277 , 1 , WritePredicate ( 275 , 2 , i > 0 ) && WritePredicate ( 276 , 2 , a [ i ] == 0 ) ) ) {
		WriteStatement ( 153 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 154 , 0 , 2 ) ;
	a [ 0 ] = a [ 0 ] >> sh ;
	WriteStatement ( 155 , 0 , 2 ) ;
	j = 1 ;
	WriteStatement ( 156 , 0 , 2 ) ;
	while ( WritePredicate ( 278 , 3 , j <= i ) ) {
		WriteStatement ( 157 , 0 , 2 ) ;
		x = ( a [ j ] << 16 ) >> sh ;
		WriteStatement ( 158 , 0 , 2 ) ;
		a [ j - 1 ] |= ( x & 0xffff ) ;
		WriteStatement ( 159 , 0 , 2 ) ;
		a [ j ] = x >> 16 ;
		WriteStatement ( 160 , 0 , 2 ) ;
		j ++ ;
	}
}
void LongRightShift ( unsigned a [ ] , int sh ) {
	int i , u ;
	unsigned temp [ 500 ] ;
	WriteStatement ( 161 , 0 , 2 ) ;
	if ( WritePredicate ( 251 , 3 , sh < 16 ) ) {
		WriteStatement ( 162 , 0 , 2 ) ;
		RightShift ( a , sh ) ;
	}
	else {
		WriteStatement ( 163 , 0 , 2 ) ;
		u = sh / 16 ;
		WriteStatement ( 164 , 0 , 2 ) ;
		sh = sh - u * 16 ;
		WriteStatement ( 165 , 0 , 2 ) ;
		i = Degree ( a ) ;
		WriteStatement ( 166 , 0 , 2 ) ;
		Initialize ( temp ) ;
		WriteStatement ( 167 , 0 , 2 ) ;
		while ( WritePredicate ( 279 , 3 , i >= u ) ) {
			WriteStatement ( 168 , 0 , 2 ) ;
			temp [ i - u ] = a [ i ] ;
			WriteStatement ( 169 , 0 , 2 ) ;
			i -- ;
		}
		WriteStatement ( 170 , 0 , 2 ) ;
		Copy ( temp , a ) ;
		WriteStatement ( 171 , 0 , 2 ) ;
		RightShift ( a , sh ) ;
	}
}
void Initialize ( unsigned c [ ] ) {
	int i , t ;
	WriteStatement ( 172 , 0 , 2 ) ;
	t = Degree ( c ) ;
	WriteStatement ( 173 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 280 , 3 , i <= t ) ;
	i ++ ) {
		WriteStatement ( 174 , 0 , 2 ) ;
		c [ i ] = 0 ;
	}
}
void Copy ( unsigned a [ ] , unsigned b [ ] ) {
	int i , s ;
	WriteStatement ( 175 , 0 , 2 ) ;
	s = Degree ( a ) ;
	WriteStatement ( 176 , 0 , 2 ) ;
	Initialize ( b ) ;
	WriteStatement ( 177 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 281 , 3 , i <= s ) ;
	i ++ ) {
		WriteStatement ( 178 , 0 , 2 ) ;
		b [ i ] = a [ i ] ;
	}
}
int ZeroCheck ( unsigned a [ ] ) {
	int i , j = WriteStatement ( 298 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
	WriteStatement ( 179 , 0 , 2 ) ;
	for ( i = 500 - 1 ;
	WritePredicate ( 282 , 3 , i >= 0 ) ;
	i -- ) {
		WriteStatement ( 180 , 0 , 2 ) ;
		if ( WritePredicate ( 252 , 3 , a [ i ] != 0 ) ) {
			WriteStatement ( 181 , 0 , 2 ) ;
			j = - 1 ;
			WriteStatement ( 182 , 0 , 2 ) ;
			break ;
		}
	}
	WriteStatement ( 183 , 0 , 2 ) ;
	return j ;
}
int Degree ( unsigned a [ ] ) {
	int i = WriteStatement ( 299 , 0 , 2 ) ? ( 500 - 1 ) : ( 500 - 1 ) ;
	WriteStatement ( 184 , 0 , 2 ) ;
	while ( WritePredicate ( 285 , 1 , WritePredicate ( 283 , 2 , i > 0 ) && WritePredicate ( 284 , 2 , a [ i ] == 0 ) ) ) {
		WriteStatement ( 185 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 186 , 0 , 2 ) ;
	return i ;
}
void ToDecimal ( unsigned a [ ] , unsigned w [ ] ) {
	int i ;
	unsigned b [ 500 ] ;
	WriteStatement ( 187 , 0 , 2 ) ;
	Copy ( a , b ) ;
	WriteStatement ( 188 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 286 , 3 , i < 601 ) ;
	i ++ ) {
		WriteStatement ( 189 , 0 , 2 ) ;
		w [ i ] = 0 ;
	}
	WriteStatement ( 190 , 0 , 2 ) ;
	i = 0 ;
	WriteStatement ( 191 , 0 , 2 ) ;
	while ( WritePredicate ( 287 , 3 , ZeroCheck ( b ) == - 1 ) ) {
		WriteStatement ( 192 , 0 , 2 ) ;
		w [ i ++ ] = Div ( b , 10000 ) ;
	}
}
void ToHex ( unsigned w [ ] , unsigned a [ ] ) {
	int i ;
	unsigned z [ 601 ] ;
	WriteStatement ( 193 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 288 , 3 , i < 601 ) ;
	i ++ ) {
		WriteStatement ( 194 , 0 , 2 ) ;
		z [ i ] = w [ i ] ;
	}
	WriteStatement ( 195 , 0 , 2 ) ;
	for ( i = 0 ;
	WritePredicate ( 289 , 3 , i < 500 ) ;
	i ++ ) {
		WriteStatement ( 196 , 0 , 2 ) ;
		a [ i ] = 0 ;
	}
	WriteStatement ( 197 , 0 , 2 ) ;
	i = 0 ;
	WriteStatement ( 198 , 0 , 2 ) ;
	while ( WritePredicate ( 290 , 3 , ToHexZeroCheck ( z ) == - 1 ) ) {
		WriteStatement ( 199 , 0 , 2 ) ;
		a [ i ++ ] = ToHexDiv ( z ) ;
	}
}
unsigned ToHexDiv ( unsigned z [ ] ) {
	int i ;
	unsigned x , q , res ;
	WriteStatement ( 200 , 0 , 2 ) ;
	res = 0 ;
	WriteStatement ( 201 , 0 , 2 ) ;
	i = 601 - 1 ;
	WriteStatement ( 202 , 0 , 2 ) ;
	while ( WritePredicate ( 293 , 1 , WritePredicate ( 291 , 2 , i > 0 ) && WritePredicate ( 292 , 2 , z [ i ] == 0 ) ) ) {
		WriteStatement ( 203 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 204 , 0 , 2 ) ;
	while ( WritePredicate ( 294 , 3 , i >= 0 ) ) {
		WriteStatement ( 205 , 0 , 2 ) ;
		x = res * 10000 + z [ i ] ;
		WriteStatement ( 206 , 0 , 2 ) ;
		q = ( x >> 16 ) ;
		WriteStatement ( 207 , 0 , 2 ) ;
		res = x - ( q << 16 ) ;
		WriteStatement ( 208 , 0 , 2 ) ;
		z [ i ] = q ;
		WriteStatement ( 209 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 210 , 0 , 2 ) ;
	return res ;
}
int ToHexZeroCheck ( unsigned z [ ] ) {
	int i , j = WriteStatement ( 300 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
	WriteStatement ( 211 , 0 , 2 ) ;
	for ( i = 601 - 1 ;
	WritePredicate ( 295 , 3 , i >= 0 ) ;
	i -- ) {
		WriteStatement ( 212 , 0 , 2 ) ;
		if ( WritePredicate ( 253 , 3 , z [ i ] != 0 ) ) {
			WriteStatement ( 213 , 0 , 2 ) ;
			j = - 1 ;
			WriteStatement ( 214 , 0 , 2 ) ;
			break ;
		}
	}
	WriteStatement ( 215 , 0 , 2 ) ;
	return j ;
}
void Display ( unsigned a [ ] ) {
	int i ;
	unsigned w [ 601 ] ;
	WriteStatement ( 216 , 0 , 2 ) ;
	ToDecimal ( a , w ) ;
	WriteStatement ( 217 , 0 , 2 ) ;
	i = Degree ( w ) ;
	WriteStatement ( 218 , 0 , 2 ) ;
	printf ( "%4.1u " , w [ i ] ) ;
	WriteStatement ( 219 , 0 , 2 ) ;
	i -- ;
	WriteStatement ( 220 , 0 , 2 ) ;
	while ( WritePredicate ( 296 , 3 , i >= 0 ) ) {
		WriteStatement ( 221 , 0 , 2 ) ;
		printf ( "%4.4u " , w [ i ] ) ;
		WriteStatement ( 222 , 0 , 2 ) ;
		i -- ;
	}
	WriteStatement ( 223 , 0 , 2 ) ;
	printf ( "\n" ) ;
}
void DisplayHex ( unsigned a [ ] ) {
	int i ;
	WriteStatement ( 224 , 0 , 2 ) ;
	i = Degree ( a ) ;
	WriteStatement ( 225 , 0 , 2 ) ;
	while ( WritePredicate ( 297 , 3 , i >= 0 ) ) {
		WriteStatement ( 226 , 0 , 2 ) ;
		printf ( "%4.4x " , a [ i -- ] ) ;
	}
	WriteStatement ( 227 , 0 , 2 ) ;
	printf ( "\n" ) ;
}
// 原則的に大きさ N の配列を扱う。但し 10 進表示に直すときに
 // 大きさ M の配列を扱う。
 // 4*N = 16 進の桁数、4*M = 10 進の桁数、M≒1.2041 N
 // 関数の定義箇所は　//関数名　で検索可能
 //Add 足算  ------------------------------------------------
 // 長倍精度整数 a[] に長倍精度整数 b[] を加え、
 // 結果を a[] に格納する。
 // a + b -> a
 // 桁上がりしないとき
 // 桁上がりするとき
 // a[i] = x の下位 2 バイト
 // 桁上がり処理
 //Sub 引き算 -----------------------------------------------
 // 長倍精度整数 a[] から長倍精度整数 b[] を引き、
 // 結果を a[] に格納する。 a[] >= b[] であること。
 // a - b -> a  (a >= b)
 // そのまま引けるとき
 // 上の桁から 1 を借りる時
 // 上の桁から 1 を引く
 //Mul かけ算 1 ---------------------------------------------
 // 長倍精度整数 a[] と 2 バイト整数 d の積
 // 結果を a[] に格納する
 // a*d -> a (d <=0xffff)
 // 桁上がり用のための変数
 // a[i] に d を掛けて、
 // 桁上がりしてきた数値 q を加える
 // a[i] = x の下位 2 バイト
 //    q = x の上位 2 バイト
 //LongMul かけ算 2 -----------------------------------------
 // 長倍精度整数 a[] と長倍精度整数 b[] の積
 // 結果を a[] に格納する
 // a*b -> a
 //
 // 初期化
 // 以下、桁上がり処理
 // c[k] = q の下位 2 バイト
 //    x = q の上位 2 バイト
 // より上位の桁に加える
 //Div 割り算 1 -------------------------------------------- 
 // 長倍精度整数 a[] を 2 バイト整数 d で割る。
 // 商を a[] に格納し、余り res (2 バイト整数) を返す。
 // d は 0 でないこと。
 // a/d -> a (d <= 0xffff), 余り res を返す。
 // 上位の桁を割ったときのおつり
 // 最上位の桁から順番に d で割る
 //   x = 上位の桁からのおつりに
 //        i 番目の桁を加える
 //   q = x を d で割ったときの商
 // res = x を d で割ったときの余り
 // 1 つ下の桁にうつる
 //LongDiv 割り算 2 -----------------------------------------
 // 長倍精度整数 a[] を長倍精度整数 b[] で割る
 // a/b  の商を quot[], 余りを res[] に格納,   
 // b は 0 でないこと。
 //
 // 初期化
 // 以後 res/b を実行する。
 // if(t==0 && b[0]==0) {
 //    printf("0 で除算。 Ctrl + c で終了\n");
 // }                    
 // b が 0 の場合には以下の while ループが無限ループ
 // 最上位の数の比較
 // res[s] >= b[t] の場合
 // c = d;
 // res[s] < b[t] の場合
 //DivCheck  割り算の検算 ------------------------------------
 //  LongDiv(a,b,quot, res) を実行後に
 //  DivCheck(a, b, quote, res) を実行する。
 //  正しければ 1 を返し、間違っていれば 0 を返す。
 //
 //Compare 大小比較 -----------------------------------------
 // 長倍精度整数 a[] と長倍精度 b[] の比較。
 // a > b ならば 1, a < b ならば -1, a = b ならば 0 を返す
 //シフト (桁ずらし) ------------------------------
 //LeftShift
 // a を左へ sh 桁シフト、(0<=sh <=16)
 // 最上位の桁だけ別に処理
 //
 //LongLeftShift
 // a を左へ sh 桁シフト  (sh は 16 以上でもよい)
 // はみ出しチェックをしていない。
 //
 //RightShift
 // a を右へ sh 桁シフト、(0<=sh <=16)、はみ出た部分は忘れる。
 // 最高位の桁を探す,
 //
 // 最下位の桁だけ別に処理
 //
 //LongRightShift
 // a を右へ sh 桁シフト、(sh は 16 より大きくてもよい), 
 // はみでた部分は忘れる。
 //
 //Initialize
 // 配列の初期化 --------------------------------------------
 // 長倍精度整数 c[] を 0 にする。
 // c -> 0
 //Copy
 // 配列のコピー --------------------------------------------
 // 長倍精度整数 a[] を長倍精度整数 b[] にコピー
 // a -> b
 //ZeroCheck
 // 零チェック ----------------------------------------------
 // 長倍精度整数 a[] に対して
 // a[] = 0 であれば 0 を返し、そうでなければ -1 を返す
 //
 //Degree
 //  長倍精度整数 a[] の最上位の桁 --------------------------
 //  a に対して a[s]!=0, a[s+1] = a[s+2] = ... = a[N-1] = 0 
 // となる s を返す。但し、成分がすべて 0 の場合は 0 を返す。
 //
 //ToDecimal
 // 10 進数への変換 -----------------------------------------
 //
 //
 //ToHex
 // 16 進数への変換 -----------------------------------------
 //
 //
 //ToHexDiv
 //  z は 10 進表示の長倍精度整数 
 // z を (2の16乗) で割って、商を z, 余り res を返す
 // z[] の最高位の桁を探す
 //
 //ToHexZeroCheck
 // z[0],..,z[M-1] がすべて 0 かどうかのチェック
 // すべて 0 であれば 0 を返し、そうでなければ -1 を返す
 //
 //Display
 // 10 進表示 -----------------------------------------------
 // 長倍精度整数 a[] を10 進数に変換して表示
 //
 // 大きさ M の配列を使用
 //DisplayHex
 //  長倍精度整数 x[] を 16 進表示 --------------------------
