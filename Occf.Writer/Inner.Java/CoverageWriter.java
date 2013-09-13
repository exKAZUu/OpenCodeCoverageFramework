package net.exkazuu;

public class CoverageWriter {
  public static CoverageWriterBase writer = new CoverageWriterBase();
  
  public static boolean WriteTestCase(int id, int type, int value) {
    writer.WriteTestCase(id, type, value);
    return true;
  }

  public static boolean WriteStatement(int id, int type, int value) {
    writer.WriteStatement(id, type, value);
    return true;
  }

  public static boolean WritePredicate(int id, int type, boolean value) {
    writer.WritePredicate(id, type, value);
    return value;
  }
}