#pragma once

#include "..\Occf.Writer.SharedMemory\SharedMemoryCommon.h"

using namespace System;
using namespace Occf::Core::CoverageInformation::Elements;

namespace Occf {
namespace Reader {
namespace SharedMemory {

	public ref class SharedMemoryReporter
	{
	public:
		static void Initialize(int size) {
			::Initialize(size);
		}

		static CoverageState Read(int id) {
			return (CoverageState)(gMemory[id] & 3);
		}
	};

}
}
}