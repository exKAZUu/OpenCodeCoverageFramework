import static org . junit . Assert . assertEquals ;
import org . junit . Test ;
public class GetMid3Test {
	@ Test public void TestMid1 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( 0 , 0 , 0 ) ;
		int expected= 0 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid2 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 1 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( 3 , 4 , 3 ) ;
		int expected= 3 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid3 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 2 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( 1 , 1 , 2 ) ;
		int expected= 1 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid4 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 3 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( 7 , 6 , 5 ) ;
		int expected= 6 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid5 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 4 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( - 2 , - 2 , - 2 ) ;
		int expected= - 2 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid6 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 5 , 5 ) ;
		GetMid3 ms= new GetMid3 ( ) ;
		int test= ms . getMid ( 5 , 3 , 4 ) ;
		int expected= 4 ;
		assertEquals ( expected , test ) ;
	}
}
