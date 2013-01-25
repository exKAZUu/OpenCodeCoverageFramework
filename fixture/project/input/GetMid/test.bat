javac -cp CoverageWriter.UM.jar -d bin src\*.java
javac -cp bin;CoverageWriter.UM.jar;junit-4.8.2.jar -d bin test\*.java
java -cp bin;CoverageWriter.UM.jar;junit-4.8.2.jar org.junit.runner.JUnitCore GetMidTest
java -version
timeout 100
