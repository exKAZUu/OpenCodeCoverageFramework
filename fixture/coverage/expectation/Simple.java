public class Simple {
	int method1 ( ) {
		int i= 0 ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 4 , 3 , i == 0 ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 1 , 0 ) ;
			System . out . println ( "0" ) ;
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 2 , 0 ) ;
		System . out . println ( "0" ) ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 3 , 0 ) ;
		return 0 ;
	}
}
