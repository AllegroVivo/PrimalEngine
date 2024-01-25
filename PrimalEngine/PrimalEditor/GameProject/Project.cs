using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract(Name = "Game")]
public class Project : ViewModelBase
{
    public static String Extension = ".primal";

    [DataMember] public String Name { get; private set; } = "New Project";
    [DataMember] public String ProjectPath { get; private set; }

    public String FullPath => $"{ProjectPath}{Name}{Extension}";

    public static Project Current => Application.Current.MainWindow?.DataContext as Project;

    public static UndoRedo UndoRedo { get; } = new();

    [DataMember(Name = "Scenes")] private ObservableCollection<Scene> _scenes = new();
    public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
    
    public ICommand AddScene { get; private set; }
    public ICommand RemoveScene { get; private set; }
    public ICommand UndoAction { get; private set; }
    public ICommand RedoAction { get; private set; }
    
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

    [OnDeserialized]
    private void OnDeserialized(StreamingContext ctx)
    {
        if (_scenes != null)
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));
        }

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

        AddScene = new RelayCommand<Object>(x =>
        {
            AddSceneInternal($"New Scene {_scenes.Count}");
            Scene newScene = _scenes.Last();
            Int32 sceneIndex = _scenes.Count - 1;
            
            UndoRedo.Add(new UndoRedoAction(
                () => RemoveSceneInternal(newScene),
                () => _scenes.Insert(sceneIndex, newScene),
                $"Add {newScene.Name}"));
        });
        RemoveScene = new RelayCommand<Scene>(x =>
        {
            Int32 sceneIndex = _scenes.IndexOf(x);
            RemoveSceneInternal(x);

            UndoRedo.Add(new UndoRedoAction(
                () => _scenes.Insert(sceneIndex, x),
                () => RemoveSceneInternal(x),
                $"Remove {x.Name}"));
        }, x => !x.IsActive);

        UndoAction = new RelayCommand<Object>(_=>UndoRedo.Undo());
        RedoAction = new RelayCommand<Object>(_=>UndoRedo.Redo());
    }

    public Project(String name, String path)
    {
        Name = name;
        ProjectPath = path;

        OnDeserialized(new StreamingContext());
    }

    public void Unload()
    {
    }

    public static Project Load(String file)
    {
        Debug.Assert(File.Exists(file));
        return Serializer.FromFile<Project>(file);
    }

    public static void Save(Project project)
    {
        Serializer.ToFile(project, project.FullPath);
    }

    private void AddSceneInternal(String sceneName)
    {
        Debug.Assert(!String.IsNullOrEmpty(sceneName.Trim()));
        _scenes.Add(new Scene(this, sceneName));
    }

    private void RemoveSceneInternal(Scene scene)
    {
        Debug.Assert(_scenes.Contains(scene));
        _scenes.Remove(scene);
    }
}