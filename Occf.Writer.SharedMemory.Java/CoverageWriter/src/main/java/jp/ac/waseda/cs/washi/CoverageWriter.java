/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.1
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

package jp.ac.waseda.cs.washi;

public class CoverageWriter {
  static {
    System.loadLibrary("Occf.Writer.SharedMemory.Java");
  }

  public static boolean WriteStatement(int id, int type, int value) {
    return CoverageWriterJNI.WriteStatement(id, type, value);
  }

  public static boolean WritePredicate(int id, int type, boolean value) {
    return CoverageWriterJNI.WritePredicate(id, type, value);
  }

}
