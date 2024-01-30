using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.Components;

[DataContract]
[KnownType(typeof(Transform))]
public class GameEntity : ViewModelBase
{
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

    
    [DataMember]
    public Scene ParentScene { get; private set; }
    
    public ICommand RenameCommand { get; private set; }
    public ICommand IsEnabledCommand { get; private set; }

    [DataMember(Name = nameof(Components))]
    private readonly ObservableCollection<Component> _components = new();
    public ReadOnlyObservableCollection<Component> Components { get; private set;  }

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

        RenameCommand = new RelayCommand<String>(x =>
        {
            String oldName = _name;
            Name = x;
            Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity '{oldName}` to `{x}`"));
        }, x => x != _name);

        IsEnabledCommand = new RelayCommand<Boolean>(x =>
        {
            Boolean oldValue = _isEnabled;
            IsEnabled = x;
            Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldValue, x, (x ? "Enable " : "Disable ") + $"'{Name}'"));
        });
    }
}