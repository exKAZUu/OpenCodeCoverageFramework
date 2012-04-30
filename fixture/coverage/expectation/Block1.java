public class Block1 {
	int method1 ( ) {
		int i= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 35 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 24 , 3 , i<= 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 1 , 0 , 2 ) ;
			System . out . println ( "0" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 2 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 25 , 3 , i == 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 3 , 0 , 2 ) ;
			System . out . println ( "0" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 4 , 0 , 2 ) ;
			System . out . println ( "else" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 5 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 26 , 3 , i == 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 6 , 0 , 2 ) ;
			System . out . println ( "0" ) ;
		}
		else {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 7 , 0 , 2 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 27 , 3 , i == 1 ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 8 , 0 , 2 ) ;
				System . out . println ( "1" ) ;
			}
			else {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 9 , 0 , 2 ) ;
				if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 28 , 3 , i == 2 ) ) {
					jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 10 , 0 , 2 ) ;
					System . out . println ( "2" ) ;
				}
				else {
					jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 11 , 0 , 2 ) ;
					System . out . println ( "else" ) ;
				}
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 12 , 0 , 2 ) ;
		switch ( i ) {
			case 0 : jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 32 , 4 , 2 ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 13 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 14 , 0 , 2 ) ;
			break ;
			default : jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 33 , 4 , 2 ) ;
		}
		test : jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 15 , 0 , 2 ) ;
		while ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 29 , 3 , i != 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 16 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 17 , 0 , 2 ) ;
		do {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 18 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		while ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 30 , 3 , i != 0 ) ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 19 , 0 , 2 ) ;
		for ( i= 0 ;
		jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 31 , 3 , i< 0 ) ;
		i ++ ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 20 , 0 , 2 ) ;
			System . out . println ( "test" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 21 , 0 , 2 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 34 , 3 , 1 ) ;
		for ( int j : new int [ ] {
			1 , 2 }
		) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 34 , 3 , 0 ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 22 , 0 , 2 ) ;
			System . out . println ( j ) ;
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 34 , 3 , 1 ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 34 , 3 , 1 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 23 , 0 , 2 ) ;
		return 0 ;
	}
}
