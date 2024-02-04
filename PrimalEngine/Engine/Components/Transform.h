#pragma once

#include "ComponentsCommon.h"

namespace primal::transform
{
    struct init_info
    {
        Single position[3] { };
        Single rotation[4] { };
        Single scale[3] { 1.f, 1.f, 1.f };
    };

    component create_transform(const init_info& info, game_entity::entity entity);
    void remove_transform(component c);
}
