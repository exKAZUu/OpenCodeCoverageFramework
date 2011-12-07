int WritePredicate(int id, int type, int value)
{
	unsigned char v = (value + 1) | (type << 2);
	Write(id, v);
	return value;
}

int WriteStatement(int id, int type, int value)
{
	return WritePredicate(id, type, value);
}
