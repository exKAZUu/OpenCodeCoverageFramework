import CoverageWriter

CoverageWriter . WriteStatement ( 0 , 0 ) ; i = 0
if CoverageWriter . WritePredicate ( 7 , 3 , i == 0 ) : CoverageWriter . WriteStatement ( 1 , 0 ) ; print ( "test" )
while i != 0 : CoverageWriter . WriteStatement ( 2 , 0 ) ; print ( "test" )
for x in [ 0 ] : CoverageWriter . WriteStatement ( 3 , 0 ) ; print ( "test" )
if CoverageWriter . WritePredicate ( 8 , 3 , i == 0 ) :
	CoverageWriter . WriteStatement ( 4 , 0 ) ; print ( "test" )
while i != 0 :
	CoverageWriter . WriteStatement ( 5 , 0 ) ; print ( "test" )
for x in [ 0 ] :
	CoverageWriter . WriteStatement ( 6 , 0 ) ; print ( "test" )

