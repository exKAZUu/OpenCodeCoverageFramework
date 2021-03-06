package fizzbuzz ;
import static org . hamcrest . CoreMatchers . * ;
import static org . junit . Assert . * ;
import org . junit . Test ;
public class FizzBuzzTest {
	@ Test public void T1_Input1_Output1 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 0 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 1 ) , is ( "1:" ) ) ;
	}
	@ Test public void T2_Input2_Output2 ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 1 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 2 ) , is ( "2:" ) ) ;
	}
	@ Test public void T3_Input3_Output3Fizz ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 2 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 3 ) , is ( "3:Fizz" ) ) ;
	}
	@ Test public void T4_Input5_Output5Buzz ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 3 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 5 ) , is ( "5:Buzz" ) ) ;
	}
	@ Test public void T5_Input15_Output15FizzBuzz ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 4 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 15 ) , is ( "15:FizzBuzz" ) ) ;
	}
	@ Test public void T6_Input1and15_IsNoProblem ( ) {
		jp . ac . waseda . cs . washi . CoverageWriter . WriteTestCase ( 5 , 5 , 2 ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 1 ) , is ( "1:" ) ) ;
		assertThat ( FizzBuzz . fizzBuzz ( 15 ) , is ( "15:FizzBuzz" ) ) ;
	}
}
