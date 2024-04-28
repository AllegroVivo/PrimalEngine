using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public abstract IMSComponent GetMultiselectionComponent(MSEntity entity);
}

abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
{
    private Boolean _enableUpdates = true; 
    
    protected abstract Boolean UpdateComponents(String propertyName);
    protected abstract Boolean UpdateMSComponent();
    
    public List<T> SelectedComponents { get; }

    public MSComponent(MSEntity msEntity)
    {
        Debug.Assert(msEntity?.SelectedEntities?.Any() == true);
        SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();
        PropertyChanged += (_, e) =>
        {
            if (_enableUpdates)
                UpdateComponents(e.PropertyName);
        };
    }

    public void Refresh()
    {
        _enableUpdates = false;
        UpdateMSComponent();
        _enableUpdates = true;
    }
}