#pragma once

#include "ComponentsCommon.h"

namespace Primal::transform
{
    DEFINE_TYPED_ID(transform_id);

    struct init_info
    {
        Single position[3] { };
        Single rotation[4] { };
        Single scale[3] { 1.f, 1.f, 1.f };
    };

    transform_id create_transform(const init_info& info, game_entity::entity_id entity_id);
    void remove_transform(transform_id id);
}
