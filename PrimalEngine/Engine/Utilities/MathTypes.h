#pragma once

#include "CommonHeaders.h"
#include "PrimitiveTypes.h"

namespace primal::math
{
    constexpr Single pi = 3.1415926535897932384626433832795f;
    constexpr Single epsilon = 1e-5f;

#if defined(_WIN64)
    using Vector2 = DirectX::XMFLOAT2;
    using Vector2A = DirectX::XMFLOAT2A;
    using Vector3 = DirectX::XMFLOAT3;
    using Vector3A = DirectX::XMFLOAT3A;
    using Vector4 = DirectX::XMFLOAT4;
    using Vector4A = DirectX::XMFLOAT4A;
    using Vector2UInt = DirectX::XMUINT2;
    using Vector3UInt = DirectX::XMUINT3;
    using Vector4UInt = DirectX::XMUINT4;
    using Vector2Int = DirectX::XMINT2;
    using Vector3Int = DirectX::XMINT3;
    using Vector4Int = DirectX::XMINT4;
    using Matrix3x3 = DirectX::XMFLOAT3X3;
    using Matrix4x4 = DirectX::XMFLOAT4X4;
    using Matrix4x4A = DirectX::XMFLOAT4X4A;
#endif
}
