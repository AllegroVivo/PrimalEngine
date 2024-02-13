#pragma once

#include "../Components/ComponentsCommon.h"

namespace primal::script
{
    DEFINE_TYPED_ID(script_id);
    
    class component final
    {
    public:
        constexpr explicit component(script_id id) : _id { id }
        {
        }

        constexpr component() : _id { id::invalid_id }
        {
        }

        constexpr script_id get_id() const { return _id; }
        constexpr Boolean is_valid() const { return id::is_valid(_id); }

        math::Vector3 position() const;
        math::Vector4 rotation() const;
        math::Vector3 scale() const;
        
    private:
        script_id _id;
    };
}
