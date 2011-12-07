import static org.junit.Assert.assertEquals;
import org.junit.Test;

public class GetMid3Test {
	@Test
	public void TestMid1() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(0,0,0);
		int expected = 0;
		assertEquals(expected, test);
	}

	@Test
	public void TestMid2() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(3,4,3);
		int expected = 3;
		assertEquals(expected, test);
	}

	@Test
	public void TestMid3() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(1,1,2);
		int expected = 1;
		assertEquals(expected, test);
	}

	@Test
	public void TestMid4() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(7,6,5);
		int expected = 6;
		assertEquals(expected, test);
	}

	@Test
	public void TestMid5() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(-2,-2,-2);
		int expected = -2;
		assertEquals(expected, test);
	}

	@Test
	public void TestMid6() {
		GetMid3 ms = new GetMid3();
		int test = ms.getMid(5,3,4);
		int expected = 4;
		assertEquals(expected, test);
	}

}
