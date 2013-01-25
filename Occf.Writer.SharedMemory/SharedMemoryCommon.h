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
		(HANDLE)-1,			// ファイルのハンドル(0xFFFFFFFFだとファイルを生成しない)
		NULL,				// セキュリティ
		PAGE_READWRITE,		// 保護
		0,					// サイズを表す上位 DWORD
		size,				// サイズを表す下位 DWORD
		name);				// オブジェクト名（識別子）
	printf("CreateFileMapping, %d\n", gHMemory);
	if (GetLastError() == ERROR_ALREADY_EXISTS) {
		gHMemory = OpenFileMapping(
			FILE_MAP_ALL_ACCESS,	// アクセスモード
			FALSE,					// 継承フラグ（子プロセスに継承するか否か）
			name);					// オブジェクト名（識別子）
	}
	if (gHMemory == NULL)
		return 0;

	gMemory = (unsigned char *)MapViewOfFile(
		gHMemory,				// ファイルマッピングオブジェクトのハンドル
		FILE_MAP_ALL_ACCESS,	// アクセスモード
		0,						// オフセットの上位 DWORD
		0,						// オフセットの下位 DWORD
		0);						// マップ対象のバイト数
	printf("MapViewOfFile, %d\n", gMemory);
	if (gMemory == NULL) {
		Release();
		return 0;
	}
	return 1;
}
