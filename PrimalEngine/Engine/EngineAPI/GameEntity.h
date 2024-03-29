﻿#pragma once

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"

// ReSharper disable CppClangTidyBugproneMacroParentheses

namespace primal
{
    namespace game_entity
    {
        DEFINE_TYPED_ID(entity_id);

        class entity
        {
        public:
            constexpr explicit entity(entity_id id) : _id { id }
            {
            }

            constexpr entity() : _id { id::invalid_id }
            {
            }

            constexpr entity_id get_id() const { return _id; }
            constexpr Boolean is_valid() const { return id::is_valid(_id); }

            transform::component transform() const;
            script::component script() const;

        private:
            entity_id _id;
        };
    }

    namespace script
    {
        class entity_script : public game_entity::entity
        {
        public:
            virtual ~entity_script() = default;

            virtual void begin_play()
            {
            }

            virtual void update(Single)
            {
            }

        protected:
            constexpr explicit entity_script(game_entity::entity entity)
                : game_entity::entity { entity.get_id() }
            {
            }
        };

        namespace detail
        {
            using script_ptr = std::unique_ptr<entity_script>;
            using script_creator = script_ptr(*)(game_entity::entity entity);
            using string_hash = std::hash<std::string>;

            UInt8 register_script(size_t, script_creator);

            template<class script_class>
            script_ptr create_script(game_entity::entity entity)
            {
                assert(entity.is_valid());
                return std::make_unique<script_class>(entity);
            }
        }

#define REGISTER_SCRIPT(TYPE)                                                   \
        class TYPE;                                                             \
        namespace                                                               \
        {                                                                       \
            UInt8 _reg_##TYPE {                                                 \
                primal::script::detail::register_script(                        \
                    primal::script::detail::string_hash()(#TYPE),               \
                    &primal::script::detail::create_script<TYPE>)               \
            };                                                                  \
        }
    }
}
