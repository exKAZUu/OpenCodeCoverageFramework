import clr
clr.AddReference("Code2Xml.Languages.Python3")
clr.AddReference("Occf.Languages.Python3")

from Occf.Languages.Python3.CodeToXmls import *
from Occf.Languages.Python3.XmlToCodes import *
from Occf.Languages.Python3.Operators.Inserters import *
from Occf.Languages.Python3.Operators.Selectors import *
from Occf.Languages.Python3.Operators.Taggers import *

def FilePattern():
	return "*.py"

def CodeToXml():
	return Python3CodeToXml.Instance

def XmlToCode():
	return Python3XmlToCode.Instance

def NodeInserter():
	return Python3NodeInserter()

def StatementSelector():
	return Python3SimpleStatementSelector()

def InitializerSelector():
	return None

def BranchSelector():
	return Python3BranchSelector()

def ConditionSelector():
	return Python3ConditionSelector()

def SwitchSelector():
	return None #Python3SwitchSelector()

def CaseLabelTailSelector():
	return None #Python3CaseLabelTailSelector()

def ForeachSelector():
	return Python3ForeachSelector()

def TestCaseLabelTailSelector():
	return None

def Tagger():
	return Python3Tagger()

def LibraryNames():
	return []
