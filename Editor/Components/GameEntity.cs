using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using Editor.DllWrapper;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Components;

[DataContract]
[KnownType(typeof(Transform))]
class GameEntity : ViewModelBase
{
    private Int32 _entityID = ID.INVALID_ID;

    public Int32 EntityID
    {
        get => _entityID;
        set
        {
            if (_entityID != value)
            {
                _entityID = value;
                OnPropertyChanged(nameof(EntityID));
            }
        }
    }

    private Boolean _isActive;

    public Boolean IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (_isActive)
                {
                    EntityID = EngineAPI.CreateGameEntity(this);
                    Debug.Assert(ID.IsValid(_entityID));
                }
                else
                {
                    EngineAPI.RemoveGameEntity(this);
                }
                
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }

    
    private Boolean _isEnabled = true;
    [DataMember]
    public Boolean IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    private String _name;
    [DataMember]
    public String Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
    
    [DataMember]
    public Scene ParentScene { get; private set; }

    [DataMember(Name = nameof(Components))]
    private readonly ObservableCollection<Component> _components = new();
    public ReadOnlyObservableCollection<Component> Components { get; private set; }

    public GameEntity(Scene scene)
    {
        Debug.Assert(scene != null);
        ParentScene = scene;
        _components.Add(new Transform(this));

        OnDeserialized(new StreamingContext());
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {
        if (_components != null)
        {
            Components = new ReadOnlyObservableCollection<Component>(_components);
            OnPropertyChanged(nameof(Components));
        }
    }
    
    public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
    public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;
}

abstract class MSEntity : ViewModelBase
{
    private Boolean? _isEnabled;
    public Boolean? IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    private String _name;
    public String Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private Boolean _enableUpdates = true;

    private readonly ObservableCollection<IMSComponent> _components = new();
    public ReadOnlyObservableCollection<IMSComponent> Components { get; }
    
    public List<GameEntity> SelectedEntities { get; }

    public MSEntity(List<GameEntity> entities)
    {
        Debug.Assert(entities?.Any() == true);
        Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
        SelectedEntities = entities;
        PropertyChanged += (_, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); };
    }

    protected virtual Boolean UpdateGameEntities(String propertyName)
    {
        switch (propertyName)
        {
            case nameof(IsEnabled):
                SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled!.Value);
                return true;
            case nameof(Name):
                SelectedEntities.ForEach(x=>x.Name = Name);
                return true;
        }

        return false;
    }

    public void Refresh()
    {
        _enableUpdates = false;
        UpdateMSGameEntity();
        _enableUpdates = true;
    }

    protected virtual bool UpdateMSGameEntity()
    {
        IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, Boolean>(x => x.IsEnabled));
        Name = GetMixedValue(SelectedEntities, new Func<GameEntity, String>(x => x.Name));

        return true;
    }

    public static Single? GetMixedValue(List<GameEntity> entities, Func<GameEntity, Single> getProperty)
    {
        Single value = getProperty(entities.First());
        
        foreach (GameEntity entity in entities.Skip(1))
        {
            if (!value.IsTheSameAs(getProperty(entity)))
                return null;
        }

        return value;
    }

    public static Boolean? GetMixedValue(List<GameEntity> entities, Func<GameEntity, Boolean> getProperty)
    {
        Boolean value = getProperty(entities.First());
        
        foreach (GameEntity entity in entities.Skip(1))
        {
            if (value != getProperty(entity))
                return null;
        }

        return value;
    }

    public static String GetMixedValue(List<GameEntity> entities, Func<GameEntity, String> getProperty)
    {
        String value = getProperty(entities.First());
        
        foreach (GameEntity entity in entities.Skip(1))
        {
            if (value != getProperty(entity))
                return null;
        }

        return value;
    }
}

class MSGameEntity : MSEntity
{
    public MSGameEntity(List<GameEntity> entities) 
        : base(entities)
    {
        Refresh();
    }
}