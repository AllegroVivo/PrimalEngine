#pragma once

#pragma warning(disable: 4530)

// C/C++ Libraries
#include <cstdint>
#include <cassert>
#include <typeinfo>

#if defined(_WIN64)
    #include <DirectXMath.h>
#endif

// Primal Project Files
#include "../Utilities/MathTypes.h"
#include "../Utilities/Utilities.h"
#include "PrimitiveTypes.h"