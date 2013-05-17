package jp.ac.waseda.cs.washi;

import java.io.IOException;
import java.io.PrintStream;

public class CoverageWriter {
	private static PrintStream writer;
	static {
		try {
			writer = new PrintStream(".occf_record");
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public static boolean WriteTestCase(int id, int type, int value) {
		writer.println(id + " " + type + " " + (value + 1));
		return true;
	}

	public static boolean WriteStatement(int id, int type, int value) {
		writer.println(id + " " + type + " " + (value + 1));
		return true;
	}

	public static boolean WritePredicate(int id, int type, boolean value) {
		writer.println(id + " " + type + " " + (value ? 2 : 1));
		return value;
	}

	public static boolean WriteEqual(int id, int type, int leftValue, int rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b1;
	}

	public static boolean WriteNotEqual(int id, int type, int leftValue, int rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return !b1;
	}

	public static boolean WriteLessThan(int id, int type, int leftValue, int rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b2;
	}

	public static boolean WriteGreaterThan(int id, int type, int leftValue, int rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b3;
	}

	public static boolean WriteEqual(int id, int type, double leftValue, double rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b1;
	}

	public static boolean WriteNotEqual(int id, int type, double leftValue, double rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return !b1;
	}

	public static boolean WriteLessThan(int id, int type, double leftValue, double rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b2;
	}

	public static boolean WriteGreaterThan(int id, int type, double leftValue, double rightValue) {
		boolean b1 = leftValue == rightValue;
		boolean b2 = leftValue < rightValue;
		boolean b3 = leftValue > rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + (b2 ? 2 : 1) + " " + (b3 ? 2 : 1));
		return b3;
	}

	public static boolean WriteEqual(int id, int type, Object leftValue, Object rightValue) {
		boolean b1 = leftValue == rightValue;
		
		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + 3 + " " + 3);
		return b1;
	}

	public static boolean WriteNotEqual(int id, int type, Object leftValue, Object rightValue) {
		boolean b1 = leftValue == rightValue;

		writer.println(id + " " + type + " " + (b1 ? 2 : 1) + " " + 3 + " " + 3);
		return !b1;
	}
}
