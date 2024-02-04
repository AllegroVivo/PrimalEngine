#pragma once

#include "ComponentsCommon.h"

namespace Primal
{
    
#define INIT_INFO(component) namespace component { struct init_info; }
    
    INIT_INFO(transform);
    
    #undef INIT_INFO
    
    namespace game_entity
    {
        struct entity_info
        {
            transform::init_info* transform { nullptr };
        };

        entity_id CreateGameEntity(const entity_info& info);
        void RemoveGameEntity(entity_id id);
        bool IsAlive(entity_id id);
    }
}
