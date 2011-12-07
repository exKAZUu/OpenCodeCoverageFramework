import static org . junit . Assert . assertEquals ;
import org . junit . Test ;
public class GetMidTest {
	@ Test public void TestMid1 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 0 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 1 , 2 , 3 ) ;
		int expected= 2 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid2 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 1 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 5 , - 2 , 3 ) ;
		int expected= 3 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid3 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 2 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( - 5 , - 5 , - 8 ) ;
		int expected= - 5 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid4 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 3 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 0 , - 1 , 1 ) ;
		int expected= 0 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid5 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 4 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 6 , 6 , 6 ) ;
		int expected= 6 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid6 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 5 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 0 , 5 , 1 ) ;
		int expected= 1 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid7 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 6 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 4 , 5 , - 3 ) ;
		int expected= 4 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid8 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 7 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( - 5 , 3 , - 5 ) ;
		int expected= - 5 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid9 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 8 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 2 , - 8 , - 1 ) ;
		int expected= - 1 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid10 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 9 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 4 , - 1 , - 3 ) ;
		int expected= - 1 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid11 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 10 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( - 5 , - 6 , - 5 ) ;
		int expected= - 5 ;
		assertEquals ( expected , test ) ;
	}
	@ Test public void TestMid12 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteStatement ( 0 , 11 , 5 ) ;
		GetMid ms= new GetMid ( ) ;
		int test= ms . getMid ( 0 , 3 , 1 ) ;
		int expected= 1 ;
		assertEquals ( expected , test ) ;
	}
}
