using System;

namespace Editor.Utilities;

public static class MathUtil
{
    public static Single Epsilon => 0.00001f;

    public static Boolean IsTheSameAs(this Single value, Single other) => Math.Abs(value - other) < Epsilon;
    public static Boolean IsTheSameAs(this Single? value, Single? other)
    {
        if (!value.HasValue || !other.HasValue)
            return false;
        
        return Math.Abs(value.Value - other.Value) < Epsilon;
    }
}
