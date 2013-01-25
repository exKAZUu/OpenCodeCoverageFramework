import CoverageWriter

CoverageWriter . WriteStatement ( 0 , 0 , 2 ) ; i = 0
CoverageWriter . WriteStatement ( 1 , 0 , 2 ) ;
if CoverageWriter . WritePredicate ( 7 , 3 , i == 0 ) :
	CoverageWriter . WriteStatement ( 2 , 0 , 2 ) ; print ( "test" )
CoverageWriter . WriteStatement ( 3 , 0 , 2 ) ;
while i != 0 :
	CoverageWriter . WriteStatement ( 4 , 0 , 2 ) ; print ( "test" )
CoverageWriter . WriteStatement ( 5 , 0 , 2 ) ;
for x in [ 0 ] :
	CoverageWriter . WriteStatement ( 6 , 0 , 2 ) ; print ( "test" )

