using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;
using PrimalEditor.Components;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract]
class Scene : ViewModelBase
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

    [DataMember]
    public Project Project { get; private set; }

    [DataMember(Name = nameof(GameEntities))]
    private ObservableCollection<GameEntity> _gameEntities = new();
    public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set;  }

    public ICommand AddGameEntityCommand { get; private set; }
    public ICommand RemoveGameEntityCommand { get; private set; }

    private Boolean _isActive;

    [DataMember]
    public Boolean IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }

    public Scene(Project project, String name)
    {
        Debug.Assert(project != null);
        
        Project = project;
        Name = name;
        
        OnDeserialized(new StreamingContext());
    }

    private void AddGameEntity(GameEntity entity, Int32 index = -1)
    {
        Debug.Assert(!_gameEntities.Contains(entity));
        entity.IsActive = IsActive;

        if (index == -1)
            _gameEntities.Add(entity);
        else
            _gameEntities.Insert(index, entity);
    }

    private void RemoveGameEntity(GameEntity entity)
    {
        Debug.Assert(_gameEntities.Contains(entity));
        entity.IsActive = false;
        _gameEntities.Remove(entity);
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (_gameEntities != null)
        {
            GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
            OnPropertyChanged(nameof(GameEntities));
        }

        foreach (GameEntity entity in _gameEntities!)
        {
            entity.IsActive = IsActive;
        }
        
        AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
        {
            AddGameEntity(x);
            Int32 entityIndex = _gameEntities.Count - 1;
            
            Project.UndoRedo.Add(new UndoRedoAction(
                () => RemoveGameEntity(x),
                () => AddGameEntity(x, entityIndex),
                $"Add {x.Name} to {Name}"));
        });
        RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
        {
            Int32 entityIndex = _gameEntities.IndexOf(x);
            RemoveGameEntity(x);

            Project.UndoRedo.Add(new UndoRedoAction(
                () => AddGameEntity(x, entityIndex),
                () => RemoveGameEntity(x),
                $"Remove {x.Name} from {Name}"));
        });
    }
}