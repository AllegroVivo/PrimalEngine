using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Editor.Components;
using Editor.EngineAPIStructs;

namespace Editor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new(1f, 1f, 1f);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {

        public TransformComponent Transform = new();
    }
}

namespace Editor.DllWrapper
{
    static class EngineAPI
    {
        private const String _dllName = "EngineDll.dll";

        [DllImport(_dllName)]
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

        [DllImport(_dllName)]
        private static extern Int32 RemoveGameEntity(Int32 id);
        public static void RemoveGameEntity(GameEntity entity) => RemoveGameEntity(entity.EntityID);
    }
}