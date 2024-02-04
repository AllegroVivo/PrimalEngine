#pragma once

#include <string>

// -------------------------------------------------- //

typedef unsigned char UInt8;
typedef unsigned short UInt16;
typedef unsigned int UInt32;
typedef unsigned long long UInt64;

typedef signed char Int8;
typedef signed short Int16;
typedef signed int Int32;
typedef signed long long Int64;

typedef float Single;
typedef double Double;

typedef bool Boolean;
typedef std::string String;

constexpr UInt8 u8_invalid_id{ 0xffui8 };
constexpr UInt16 u16_invalid_id{ 0xffffui16 };
constexpr UInt32 u32_invalid_id{ 0xffff'ffffui32 };
constexpr UInt64 u64_invalid_id{ 0xffff'ffff'ffff'ffffui64 };

// -------------------------------------------------- //