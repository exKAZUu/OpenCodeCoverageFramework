package fizzbuzz;
import static org.hamcrest.CoreMatchers.*;
import static org.junit.Assert.*;

import org.junit.Test;

public class FizzBuzzTest {

	@Test
	public void T1_Input1_Output1() {
		assertThat(FizzBuzz.fizzBuzz(1), is("1:"));
	}

	@Test
	public void T2_Input2_Output2() {
		assertThat(FizzBuzz.fizzBuzz(2), is("2:"));
	}

	@Test
	public void T3_Input3_Output3Fizz() {
		assertThat(FizzBuzz.fizzBuzz(3), is("3:Fizz"));
	}

	@Test
	public void T4_Input5_Output5Buzz() {
		assertThat(FizzBuzz.fizzBuzz(5), is("5:Buzz"));
	}

	@Test
	public void T5_Input15_Output15FizzBuzz() {
		assertThat(FizzBuzz.fizzBuzz(15), is("15:FizzBuzz"));
	}

	@Test
	public void T6_Input1and15_IsNoProblem() {
		assertThat(FizzBuzz.fizzBuzz(1), is("1:"));
		assertThat(FizzBuzz.fizzBuzz(15), is("15:FizzBuzz"));
	}
}
