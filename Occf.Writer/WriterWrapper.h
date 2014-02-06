// type:
//  Statement            = 0,
//  Decision             = 1,
//  Condition            = 2,
//  DecisionAndCondition = 3,
//  SwitchCase           = 4,
//  TestCase             = 5,

// value:
//  false                = 0
//  true                 = 1
//  statement            = 2

int WritePredicate(int id, int type, int value)
{
	unsigned char v = (((value ? 1 : 0) + 1) & 0x3) | (type << 2);
	Write(id, v);
	return value;
}

int WriteStatement(int id, int type, int value)
{
	return WritePredicate(id, type, value);
}

int WriteTestCase(int id, int type, int value)
{
	return WritePredicate(id, type, value);
}