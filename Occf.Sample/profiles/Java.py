import clr
clr.AddReference("Code2Xml.Languages.Java")
clr.AddReference("Occf.Languages.Java")

from Code2Xml.Languages.Java.CodeToXmls import *
from Code2Xml.Languages.Java.XmlToCodes import *
from Occf.Languages.Java.Operators.Inserters import *
from Occf.Languages.Java.Operators.Selectors import *
from Occf.Languages.Java.Operators.Taggers import *

def FilePattern():
	return "*.java"

def CodeToXml():
	return JavaCodeToXml.Instance

def XmlToCode():
	return JavaXmlToCode.Instance

def NodeInserter():
	return JavaNodeInserter()

def FunctionSelector():
	return JavaMethodSelector()

def FunctionNameSelector():
	return JavaMethodNameSelector()

def StatementSelector():
	return JavaStatementSelector()

def InitializerSelector():
	return JavaInitializerSelector()

def BranchSelector():
	return JavaBranchSelector()

def ConditionSelector():
	return JavaConditionSelector()

def SwitchSelector():
	return JavaSwitchSelector()

def CaseLabelTailSelector():
	return JavaCaseLabelTailSelector()

def ForeachSelector():
	return JavaForeachSelector()

def ForeachHeadSelector():
	return JavaForeachHeadSelector()

def ForeachTailSelector():
	return JavaForeachTailSelector()

def TestCaseLabelTailSelector():
	return JavaTestCaseLabelTailSelector()

def Tagger():
	return JavaTagger()

def LibraryNames():
	return ["CoverageWriter.File.jar", "Occf.Writer.File.Java.dll", "junit-4.8.2.jar"]
