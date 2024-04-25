﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Editor.Components;

interface IMSComponent
{ }

[DataContract]
abstract class Component : ViewModelBase
{
    [DataMember]
    public GameEntity Owner { get; private set; }

    public Component(GameEntity owner)
    {
        Debug.Assert(owner != null);
        Owner = owner;
    }
}

abstract class MSComponent<T> : ViewModelBase where T : Component
{
    
}