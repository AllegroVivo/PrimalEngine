﻿#include "Common.h"
#include "CommonHeaders.h"

#ifndef WIN32_MEAN_AND_LEAN
    #define WIN32_MEAN_AND_LEAN
#endif

#include <Windows.h>

using namespace primal;

namespace
{
    HMODULE game_code_dll { nullptr };
}

EDITOR_INTERFACE UInt32 LoadGameCodeDll(const char* dll_path)
{
    if (game_code_dll)
        return FALSE;
    
    game_code_dll = LoadLibraryA(dll_path);
    assert(game_code_dll);

    return game_code_dll ? TRUE : FALSE;
}

EDITOR_INTERFACE UInt32 UnloadGameCodeDll()
{
    if (!game_code_dll)
        return FALSE;

    assert(game_code_dll);
    Int32 result = FreeLibrary(game_code_dll);
    assert(result);
    game_code_dll = nullptr;
    
    return TRUE;
}