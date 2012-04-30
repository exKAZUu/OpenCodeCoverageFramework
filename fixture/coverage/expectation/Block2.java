public class Block2 {
	public static int method1 ( ) {
		int i= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 21 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 14 , 3 , i == 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 1 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 2 , 0 , 2 ) ;
		switch ( i ) {
			case 0 : jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 18 , 4 , 2 ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 3 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 4 , 0 , 2 ) ;
			break ;
			default : jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 19 , 4 , 2 ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 5 , 0 , 2 ) ;
		while ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 15 , 3 , i != 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 6 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 7 , 0 , 2 ) ;
		do {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 8 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		while ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 16 , 3 , i != 0 ) ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 9 , 0 , 2 ) ;
		for ( i= 0 ;
		jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 17 , 3 , i< 0 ) ;
		i ++ ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 10 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 11 , 0 , 2 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 3 , 1 ) ;
		for ( int j : new int [ ] {
			1 , 2 }
		) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 3 , 0 ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 12 , 0 , 2 ) ;
			System . out . println ( j ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 3 , 1 ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 3 , 1 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 13 , 0 , 2 ) ;
		return 0 ;
	}
}
