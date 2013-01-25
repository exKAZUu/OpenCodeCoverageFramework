#include <WinUnit.h>
extern "C" {
#include "SharedMemory.h"
}

// Fixtureの宣言
FIXTURE(SharedMemoryInitializeFixture);

// テスト直前に呼ばれる
SETUP(SharedMemoryInitializeFixture)
{

}

// テスト直後に呼ばれる
TEARDOWN(SharedMemoryInitializeFixture)
{
}

BEGIN_TESTF(initialize_0, SharedMemoryInitializeFixture)
{
	WIN_ASSERT_FALSE(Initialize(0));
}
END_TESTF

BEGIN_TESTF(initialize_1, SharedMemoryInitializeFixture)
{
	WIN_ASSERT_TRUE(Initialize(1));
}
END_TESTF
