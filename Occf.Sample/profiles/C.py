import clr
clr.AddReference("Code2Xml.Languages.C")
clr.AddReference("Occf.Languages.C")

from Code2Xml.Languages.C.CodeToXmls import *
from Code2Xml.Languages.C.XmlToCodes import *
from Occf.Languages.C.Operators.Inserters import *
from Occf.Languages.C.Operators.Selectors import *
from Occf.Languages.C.Operators.Taggers import *

def FilePattern():
	return "*.c"

def CodeToXml():
	return CCodeToXml.Instance

def XmlToCode():
	return CXmlToCode.Instance

def NodeInserter():
	return CNodeInserter()

def StatementSelector():
	return CStatementSelector()

def InitializerSelector():
	return CInitializerSelector()

def BranchSelector():
	return CBranchSelector()

def ConditionSelector():
	return CConditionSelector()

def SwitchSelector():
	return CSwitchSelector()

def CaseLabelTailSelector():
	return CCaseLabelTailSelector()

def ForeachSelector():
	return None

def ForeachHeadSelector():
	return None

def ForeachTailSelector():
	return None

def TestCaseLabelTailSelector():
	return None

def Tagger():
	return CTagger()

def LibraryNames():
	return ["covman.c", "covman.h"]
