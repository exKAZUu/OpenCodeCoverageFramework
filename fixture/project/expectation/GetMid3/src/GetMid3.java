public class GetMid3 {
	public int getMid ( int a , int b , int c ) {
		int mid= 0 ;
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 0 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 19 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 13 , 2 , a<= b ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 14 , 2 , a<= c ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 1 , 0 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 20 , 3 , c<= b ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 2 , 0 ) ;
				mid= c ;
			}
			else {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 3 , 0 ) ;
				mid= b ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 4 , 0 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 21 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 15 , 2 , b<= a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 16 , 2 , b<= c ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 5 , 0 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 22 , 3 , c<= a ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 6 , 0 ) ;
				mid= c ;
			}
			else {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 7 , 0 ) ;
				mid= a ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 8 , 0 ) ;
		if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 23 , 1 , jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 17 , 2 , c<= a ) && jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 18 , 2 , c<= b ) ) ) {
			jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 9 , 0 ) ;
			if ( jp . ac . waseda . cs . washi . CoverageWriter . WritePredicate ( 0 , 24 , 3 , b<= a ) ) {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 10 , 0 ) ;
				mid= 6 ;
			}
			else {
				jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 11 , 0 ) ;
				mid= a ;
			}
		}
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 12 , 0 ) ;
		return mid ;
	}
}
