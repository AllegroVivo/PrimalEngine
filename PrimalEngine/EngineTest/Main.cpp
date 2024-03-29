#pragma comment(lib, "engine.lib")

#define TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS
    #include "TestEntityComponents.h"
#else
    #error At least one test must be enabled!
#endif

int main(int argc, char* argv[])
{
#if _DEBUG
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
    engine_test test{};
    
    if (test.initialize())
        test.run();

    test.shutdown();

    return 0;
}
