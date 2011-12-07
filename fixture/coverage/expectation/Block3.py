import CoverageWriter

CoverageWriter . WriteStatement ( 0 , 0 ) ; i = 0
if CoverageWriter . WritePredicate ( 8 , 3 , i == 0 ) : CoverageWriter . WriteStatement ( 1 , 0 ) ; pass
while i != 0 : CoverageWriter . WriteStatement ( 2 , 0 ) ; pass
for x in [ 0 ] : CoverageWriter . WriteStatement ( 3 , 0 ) ; pass
CoverageWriter . WriteStatement ( 4 , 0 ) ; i = 0
if CoverageWriter . WritePredicate ( 9 , 3 , i == 0 ) :
	CoverageWriter . WriteStatement ( 5 , 0 ) ; pass
while i != 0 :
	CoverageWriter . WriteStatement ( 6 , 0 ) ; pass
for x in [ 0 ] :
	CoverageWriter . WriteStatement ( 7 , 0 ) ; pass

