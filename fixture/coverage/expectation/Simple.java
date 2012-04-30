public class Simple {
	int method1 ( ) {
		int i= jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 5 , 0 , 2 ) ? ( 0 ) : ( 0 ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 2 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 4 , 3 , i == 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 1 , 0 , 2 ) ;
			System . out . println ( "0" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 2 , 0 , 2 ) ;
		System . out . println ( "0" ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 3 , 0 , 2 ) ;
		return 0 ;
	}
}
