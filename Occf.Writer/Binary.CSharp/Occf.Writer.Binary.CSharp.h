// Occf.Writer.File.CSharp.h

#pragma once

using namespace System;

#include "..\Binary\File.c"

namespace Occf {
namespace Writer {
namespace CSharp {

	public ref class CoverageWriter
	{
	public:
		bool WriteStatement(int id, int type, int value) {
			return ::WriteStatement(id, type, value);
		}

		bool WritePredicate(int id, int type, bool value) {
			return ::WritePredicate(id, type, value);
		}
	};
}
}
}