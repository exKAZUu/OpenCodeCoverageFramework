public class Condition1 {
	public static boolean f ( boolean i ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 2 ) ;
		return ! i ;
	}
	public static void main ( String [ ] args ) {
		boolean a= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 61 , 0 , 2 ) ? ( false ) : ( false ) , b= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 62 , 0 , 2 ) ? ( false ) : ( false ) , c= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 63 , 0 , 2 ) ? ( true ) : ( true ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 1 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 32 , 3 , a ^ ( b && c ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 2 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 3 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 33 , 3 , a ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 4 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 5 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 36 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 34 , 2 , a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 35 , 2 , b ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 6 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 7 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 40 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 37 , 2 , a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 38 , 2 , b ) || jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 39 , 2 , c ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 8 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 9 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 44 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 41 , 2 , a ) && ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 42 , 2 , b ) || jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 43 , 2 , c ) ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 10 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 11 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 45 , 3 , 0< ( ~ 1 | 1 & 1 ^ 1 + 1 - 1 / 1 * 1 ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 12 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 13 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 48 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 46 , 2 , a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 47 , 2 , b ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 14 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 15 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 49 , 3 , args [ a ? 0 : 1 ] ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 16 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 17 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 50 , 3 , args [ a ? 0 : 1 ] ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 18 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 19 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 51 , 3 , f ( a ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 21 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 54 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 52 , 2 , a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 53 , 2 , b ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 22 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 23 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 55 , 3 , f ( a && b || c ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 24 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 25 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 56 , 3 , f ( a && ( b || c ) ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 26 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 27 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 57 , 3 , f ( 0< ( ~ 1 | 1 & 1 ^ 1 + 1 - 1 / 1 * 1 ) ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 28 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 29 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 58 , 3 , f ( a && b ) ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 30 , 0 , 2 ) ;
				System . out . println ( "test" ) ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 31 , 0 , 2 ) ;
		return ;
	}
}
