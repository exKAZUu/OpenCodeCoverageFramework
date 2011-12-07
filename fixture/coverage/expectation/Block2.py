import CoverageWriter

CoverageWriter . WriteStatement ( 0 , 0 ) ; i = 0
if CoverageWriter . WritePredicate ( 4 , 3 , i == 0 ) :
	CoverageWriter . WriteStatement ( 1 , 0 ) ; print ( "test" )
while i != 0 :
	CoverageWriter . WriteStatement ( 2 , 0 ) ; print ( "test" )
for x in [ 0 ] :
	CoverageWriter . WriteStatement ( 3 , 0 ) ; print ( "test" )

