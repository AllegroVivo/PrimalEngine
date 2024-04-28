#pragma once

#pragma warning(disable: 4530)

#include <cstdint>
#include <cassert>
#include <typeinfo>
#include <unordered_map>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

#include "../Utilities/Utilities.h"
#include "../Utilities/MathTypes.h"
#include "PrimitiveTypes.h"