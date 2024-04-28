using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using Editor.GameDev;
using Editor.Utilities;

namespace Editor.GameProject;

[DataContract(Name = "Game")]
class Project : ViewModelBase
{
    public static String Extension = ".primal";

    [DataMember] 
    public String Name { get; private set; } = "New Project";
    [DataMember]
    public String Path { get; private set; }
    public String FullPath => $@"{Path}{Name}{Extension}";
    public String Solution => $@"{Path}{Name}.sln";

    public static Project Current => Application.Current.MainWindow!.DataContext as Project;
    public static UndoRedo UndoRedo { get; } = new();

    private Scene _activeScene;
    public Scene ActiveScene
    {
        get => _activeScene;
        set
        {
            if (_activeScene != value)
            {
                _activeScene = value;
                OnPropertyChanged(nameof(ActiveScene));
            }
        }
    }
    
    public ICommand AddSceneCommand { get; private set; }
    public ICommand RemoveSceneCommand { get; private set; }
    
    public ICommand UndoCommand { get; private set; }
    public ICommand RedoCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    
    [DataMember(Name = "Scenes")]
    private ObservableCollection<Scene> _scenes = new();
    public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

    public Project(String name, String path)
    {
        Name = name;
        Path = path;

        OnDeserialized(new StreamingContext());
    }

    public void Unload()
    {
        VisualStudio.CloseVisualStudio();
        UndoRedo.Reset();
    }

    public static Project Load(String file)
    {
        Debug.Assert(File.Exists(file));
        return Serializer.FromFile<Project>(file);
    }

    public static void Save(Project project)
    {
        Serializer.ToFile(project, project.FullPath);
        Logger.Log(MessageType.Info, $"Saved project to {project.FullPath}");
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (_scenes != null)
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));
        }

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

        AddSceneCommand = new RelayCommand<Object>(_ =>
        {
            AddScene($"New Scene {_scenes.Count}");
            Scene newScene = _scenes.Last();
            Int32 sceneIndex = _scenes.Count - 1;
            UndoRedo.Add(new UndoRedoAction(
                () => RemoveScene(newScene),
                () => _scenes.Insert(sceneIndex, newScene),
                $"Add {newScene.Name}"
            ));
        });

        RemoveSceneCommand = new RelayCommand<Scene>(x =>
        {
            Int32 sceneIndex = _scenes.IndexOf(x);
            RemoveScene(x);

            UndoRedo.Add(new UndoRedoAction(
                () => _scenes.Insert(sceneIndex, x),
                () => RemoveScene(x),
                $"Remove {x.Name}"
            ));
        }, x => !x.IsActive);

        UndoCommand = new RelayCommand<Object>(_ => UndoRedo.Undo());
        RedoCommand = new RelayCommand<Object>(_ => UndoRedo.Redo());
        SaveCommand = new RelayCommand<Object>(_ => Save(this));
    }

    private void AddScene(String sceneName)
    {
        Debug.Assert(!String.IsNullOrEmpty(sceneName.Trim()));
        _scenes.Add(new Scene(this, sceneName));
    }

    private void RemoveScene(Scene scene)
    {
        Debug.Assert(_scenes.Contains(scene));
        _scenes.Remove(scene);
    }
}