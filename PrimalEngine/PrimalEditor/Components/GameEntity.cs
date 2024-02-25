using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using PrimalEditor.DllWrappers;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.Components;

[DataContract]
[KnownType(typeof(Transform))]
class GameEntity : ViewModelBase
{
    private Int32 _entityId = ID.INVALID_ID;

    public Int32 EntityId
    {
        get => _entityId;
        set
        {
            if (_entityId != value)
            {
                _entityId = value;
                OnPropertyChanged(nameof(EntityId));
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
                    EntityId = EngineAPI.EntityAPI.CreateGameEntity(this);
                    Debug.Assert(ID.IsValid(_entityId));
                }
                else if (ID.IsValid(_entityId))
                {
                    EngineAPI.EntityAPI.RemoveGameEntity(this);
                    EntityId = ID.INVALID_ID;
                }
                
                OnPropertyChanged(nameof(IsActive));
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


    [DataMember] public Scene ParentScene { get; private set; }

    [DataMember(Name = nameof(Components))]
    private readonly ObservableCollection<Component> _components = new();
    public ReadOnlyObservableCollection<Component> Components { get; private set; }

    public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
    public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

    public GameEntity(Scene scene)
    {
        Debug.Assert(scene != null);
        ParentScene = scene;
        _components.Add(new Transform(this));
        OnDeserialized(new StreamingContext());
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (_components != null)
        {
            Components = new ReadOnlyObservableCollection<Component>(_components);
            OnPropertyChanged(nameof(Components));
        }
        
        
    }
}

abstract class MSEntity : ViewModelBase
{
    private Boolean _enableUpdates = true;
    
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

    private readonly ObservableCollection<IMSComponent> _components = new();
    public ReadOnlyObservableCollection<IMSComponent> Components { get; private set; }

    public T GetMSComponent<T>() where T : IMSComponent
    {
        return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
    }
    
    public List<GameEntity> SelectedEntities { get; }

    public MSEntity(List<GameEntity> entities)
    {
        Debug.Assert(entities?.Any() == true);
        Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
        SelectedEntities = entities;
        PropertyChanged += (_, e) =>
        {  
            if (_enableUpdates)
                UpdateGameEntities(e.PropertyName);
        };
    }

    protected virtual Boolean UpdateGameEntities(String propertyName)
    {
        switch (propertyName)
        {
            case nameof(IsEnabled):
                SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value);
                return true;
            case nameof(Name):
                SelectedEntities.ForEach(x => x.Name = Name);
                return true;
        }

        return false;
    }

    public void Refresh()
    {
        _enableUpdates = false;
        UpdateMSGameEntity();
        MakeComponentList();
        _enableUpdates = true;
    }

    protected virtual Boolean UpdateMSGameEntity()
    {
        IsEnabled = GetMixedValue(SelectedEntities, x => x.IsEnabled);
        Name = GetMixedValue(SelectedEntities, x => x.Name);

        return true;
    }

    private void MakeComponentList()
    {
        _components.Clear();
        GameEntity firstEntity = SelectedEntities.FirstOrDefault();
        if (firstEntity == null)
            return;

        foreach (Component component in firstEntity.Components)
        {
            var type = component.GetType();
            if (!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
            {
                Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                _components.Add(component.GetMultiSelectionComponent(this));
            }
        }
    }


    public static Single? GetMixedValue<T>(List<T> objects, Func<T, Single> getProperty)
    {
        Single value = getProperty(objects.First());
        return objects.Skip(1).Any(x => !getProperty(x).IsTheSameAs(value)) ? null : value;
    }

    public static Boolean? GetMixedValue<T>(List<T> objects, Func<T, Boolean> getProperty)
    {
        Boolean value = getProperty(objects.First());
        return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
    }

    public static String GetMixedValue<T>(List<T> objects, Func<T, String> getProperty)
    {
        String value = getProperty(objects.First());
        return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
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