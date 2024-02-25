using System;
using System.Numerics;
using System.Runtime.InteropServices;
using PrimalEditor.Components;
using PrimalEditor.EngineAPIStructs;

namespace PrimalEditor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new();
    }
}

namespace PrimalEditor.DllWrappers
{
    static class EngineAPI
    {
        private const String _engineDll = "C:/Dev/CPlusPlus/PrimalEngine/PrimalEngine/x64/Debug/EngineDLL.dll";

        [DllImport(_engineDll, CharSet = CharSet.Ansi)]
        public static extern Int32 LoadGameCodeDll(String dllPath);
        
        [DllImport(_engineDll)]
        public static extern Int32 UnloadGameCodeDll();

        internal static class EntityAPI
        {
            [DllImport(_engineDll)]
            private static extern Int32 CreateGameEntity(GameEntityDescriptor desc);

            public static Int32 CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor desc = new();

                Transform c = entity.GetComponent<Transform>();
                desc.Transform.Position = c.Position;
                desc.Transform.Rotation = c.Rotation;
                desc.Transform.Scale = c.Scale;

                return CreateGameEntity(desc);
            }

            [DllImport(_engineDll)]
            private static extern void RemoveGameEntity(Int32 id);

            public static void RemoveGameEntity(GameEntity entity)
            {
                RemoveGameEntity(entity.EntityId);
            }
        }
    }
}