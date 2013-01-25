#include <windows.h>
#include <tchar.h>
#include <stdio.h>

static HANDLE gHMemory = NULL;
static unsigned char *gMemory = NULL;

void Release()
{
	if (gMemory != NULL) {
		UnmapViewOfFile(gMemory);
		gMemory = NULL;
	}

	if (gHMemory != NULL) {
		CloseHandle(gHMemory);
		gHMemory = NULL;
	}
}

int Initialize(int size)
{
	TCHAR name[256] = _T(".occf_coverage_data");
	//_stprintf_s(name, 256, _T(".occf_coverage_data-%d"), id);
	_tprintf(_T("%s\n"), name);

	Release();

	gHMemory = CreateFileMapping(
		(HANDLE)-1,			// �t�@�C���̃n���h��(0xFFFFFFFF���ƃt�@�C���𐶐����Ȃ�)
		NULL,				// �Z�L�����e�B
		PAGE_READWRITE,		// �ی�
		0,					// �T�C�Y��\����� DWORD
		size,				// �T�C�Y��\������ DWORD
		name);				// �I�u�W�F�N�g���i���ʎq�j
	printf("CreateFileMapping, %d\n", gHMemory);
	if (GetLastError() == ERROR_ALREADY_EXISTS) {
		gHMemory = OpenFileMapping(
			FILE_MAP_ALL_ACCESS,	// �A�N�Z�X���[�h
			FALSE,					// �p���t���O�i�q�v���Z�X�Ɍp�����邩�ۂ��j
			name);					// �I�u�W�F�N�g���i���ʎq�j
	}
	if (gHMemory == NULL)
		return 0;

	gMemory = (unsigned char *)MapViewOfFile(
		gHMemory,				// �t�@�C���}�b�s���O�I�u�W�F�N�g�̃n���h��
		FILE_MAP_ALL_ACCESS,	// �A�N�Z�X���[�h
		0,						// �I�t�Z�b�g�̏�� DWORD
		0,						// �I�t�Z�b�g�̉��� DWORD
		0);						// �}�b�v�Ώۂ̃o�C�g��
	printf("MapViewOfFile, %d\n", gMemory);
	if (gMemory == NULL) {
		Release();
		return 0;
	}
	return 1;
}
