#include "Common.h"
#include "CommonHeaders.h"
#include "Id.h"
#include "../Engine/Components/Entity.h"
#include "../Engine/Components/Transform.h"

using namespace primal;

namespace
{
    struct transform_component
    {
        Single position[3];
        Single rotation[3];
        Single scale[3];

        transform::init_info to_init_info()
        {
            using namespace DirectX;
            transform::init_info info { };
            memcpy(&info.position[0], &position[0], sizeof(Single) * _countof(position));
            memcpy(&info.scale[0], &scale[0], sizeof(Single) * _countof(scale));
            XMFLOAT3A rot { &rotation[0] };
            XMVECTOR quat { XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
            XMFLOAT4A rot_quat { };
            XMStoreFloat4A(&rot_quat, quat);
            memcpy(&info.rotation[0], &rot_quat.x, sizeof(Single) * _countof(info.rotation));
            return info;
        }
    };

    struct game_entity_descriptor
    {
        transform_component transform;
    };

    game_entity::entity entity_from_id(id::id_type id)
    {
        return game_entity::entity { game_entity::entity_id { id } };
    }
}

EDITOR_INTERFACE id::id_type CreateGameEntity(game_entity_descriptor* e)
{
    assert(e);
    game_entity_descriptor& desc { *e };
    transform::init_info transform_info { desc.transform.to_init_info() };
    game_entity::entity_info entity_info { &transform_info, };
    return create(entity_info).get_id();
}

EDITOR_INTERFACE void RemoveGameEntity(id::id_type id)
{
    assert(id::is_valid(id));
    game_entity::remove(game_entity::entity_id { id });
}
