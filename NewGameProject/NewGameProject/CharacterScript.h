﻿#pragma once

namespace first_game_project
{
    class character_script : public primal::script::entity_script
    {
    public:
        constexpr explicit character_script(primal::game_entity::entity entity)
            : primal::script::entity_script(entity)
        {
        }

        void update(Single dt) override;
    };
}
