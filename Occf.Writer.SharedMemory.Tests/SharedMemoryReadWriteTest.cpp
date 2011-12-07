#include <WinUnit.h>
extern "C" {
#include "SharedMemory.h"
}

// Fixtureの宣言
FIXTURE(SharedMemoryReadWriterFixture);

// テスト直前に呼ばれる
SETUP(SharedMemoryReadWriterFixture)
{
	Initialize(1024);
}

// テスト直後に呼ばれる
TEARDOWN(SharedMemoryReadWriterFixture)
{
	Release();
}

BEGIN_TESTF(stmt0, SharedMemoryReadWriterFixture)
{
	Write(0, 3);
	WIN_ASSERT_EQUAL(3, Read(0));
	
}
END_TESTF

BEGIN_TESTF(stmt0_1_1022_1023, SharedMemoryReadWriterFixture)
{
	WriteStatement(0, 0, 2);
	WriteStatement(1, 0, 2);
	WriteStatement(1022, 0, 2);
	WriteStatement(1023, 0, 2);
	WIN_ASSERT_EQUAL(3 << 6, Read(0));
	WIN_ASSERT_EQUAL(3 << 6, Read(1));
	WIN_ASSERT_EQUAL(3 << 6, Read(1022));
	WIN_ASSERT_EQUAL(3 << 6, Read(1023));
	
}
END_TESTF

BEGIN_TESTF(Branch_false_0_1_1022_1023, SharedMemoryReadWriterFixture)
{
	WritePredicate(0, 0, false);
	WritePredicate(1, 0, false);
	WritePredicate(1022, 0, false);
	WritePredicate(1023, 0, false);
	WIN_ASSERT_EQUAL(1 << 6, Read(0));
	WIN_ASSERT_EQUAL(1 << 6, Read(1));
	WIN_ASSERT_EQUAL(1 << 6, Read(1022));
	WIN_ASSERT_EQUAL(1 << 6, Read(1023));
	
}
END_TESTF

BEGIN_TESTF(Branch_true_0_1_1022_1023, SharedMemoryReadWriterFixture)
{
	WritePredicate(0, 0, true);
	WritePredicate(1, 0, true);
	WritePredicate(1022, 0, true);
	WritePredicate(1023, 0, true);
	WIN_ASSERT_EQUAL(2 << 6, Read(0));
	WIN_ASSERT_EQUAL(2 << 6, Read(1));
	WIN_ASSERT_EQUAL(2 << 6, Read(1022));
	WIN_ASSERT_EQUAL(2 << 6, Read(1023));
	
}
END_TESTF

BEGIN_TESTF(Branch_false_true_0_1_1022_1023, SharedMemoryReadWriterFixture)
{
	WritePredicate(0, 0, false);
	WritePredicate(1, 0, false);
	WritePredicate(1022, 0, false);
	WritePredicate(1023, 0, false);
	WritePredicate(0, 0, true);
	WritePredicate(1, 0, true);
	WritePredicate(1022, 0, true);
	WritePredicate(1023, 0, true);
	WIN_ASSERT_EQUAL(3 << 6, Read(0));
	WIN_ASSERT_EQUAL(3 << 6, Read(1));
	WIN_ASSERT_EQUAL(3 << 6, Read(1022));
	WIN_ASSERT_EQUAL(3 << 6, Read(1023));
	
}
END_TESTF
