#include <WinUnit.h>
extern "C" {
#include "SharedMemory.h"
}

// Fixture�̐錾
FIXTURE(SharedMemoryInitializeFixture);

// �e�X�g���O�ɌĂ΂��
SETUP(SharedMemoryInitializeFixture)
{

}

// �e�X�g����ɌĂ΂��
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
