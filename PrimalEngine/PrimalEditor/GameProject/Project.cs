using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrimalEditor.DllWrappers;
using PrimalEditor.GameDev;
using PrimalEditor.Utilities;

// ReSharper disable AsyncVoidLambda

namespace PrimalEditor.GameProject;

enum BuildConfiguration
{
    Debug,
    DebugEditor,
    Release,
    ReleaseEditor
}

[DataContract(Name = "Game")]
class Project : ViewModelBase
{
    public static String Extension = ".primal";

    [DataMember] public String Name { get; private set; } = "New Project";
    [DataMember] public String ProjectPath { get; private set; }

    public String Solution => $"{ProjectPath}{Name}.sln";

    public String FullPath => $"{ProjectPath}{Name}{Extension}";

    private static readonly String[] _buildConfigurationNames =
    {
        "Debug",
        "DebugEditor",
        "Release",
        "ReleaseEditor"
    };

    private static String GetConfigurationName(BuildConfiguration config) => _buildConfigurationNames[(Int32)config];

    private Int32 _buildConfig;

    [DataMember]
    public Int32 BuildConfig
    {
        get => _buildConfig;
        set
        {
            if (_buildConfig != value)
            {
                _buildConfig = value;
                OnPropertyChanged(nameof(BuildConfig));
            }
        }
    }

    public BuildConfiguration StandAloneBuildconfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
    public BuildConfiguration DllBuildConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

    public static Project Current => Application.Current.MainWindow?.DataContext as Project;

    public static UndoRedo UndoRedo { get; } = new();

    [DataMember(Name = "Scenes")] private ObservableCollection<Scene> _scenes = new();
    public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

    public ICommand AddSceneCommand { get; private set; }
    public ICommand RemoveSceneCommand { get; private set; }
    public ICommand UndoCommand { get; private set; }
    public ICommand RedoCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand BuildCommand { get; private set; }

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

    public Project(String name, String path)
    {
        Name = name;
        ProjectPath = path;

        OnDeserialized(new StreamingContext());
    }

    [OnDeserialized]
    private async void OnDeserialized(StreamingContext ctx)
    {
        if (_scenes != null)
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));
        }

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

        await BuildGameCodeDll(false);
        SetCommands();
    }

    private void SetCommands()
    {
        AddSceneCommand = new RelayCommand<Object>(x =>
        {
            AddScene($"New Scene {_scenes.Count}");
            Scene newScene = _scenes.Last();
            Int32 sceneIndex = _scenes.Count - 1;
            UndoRedo.Add(new UndoRedoAction(
                () => RemoveScene(newScene),
                () => _scenes.Insert(sceneIndex, newScene),
                $"Add {newScene.Name}"));
        });

        RemoveSceneCommand = new RelayCommand<Scene>(x =>
        {
            Int32 sceneIndex = _scenes.IndexOf(x);
            RemoveScene(x);
            UndoRedo.Add(new UndoRedoAction(
                () => _scenes.Insert(sceneIndex, x),
                () => RemoveScene(x),
                $"Remove {x.Name}"));
        }, x => !x.IsActive);

        UndoCommand = new RelayCommand<Object>(_ => UndoRedo.Undo(), _ => UndoRedo.UndoList.Any());
        RedoCommand = new RelayCommand<Object>(_ => UndoRedo.Redo(), _ => UndoRedo.RedoList.Any());
        SaveCommand = new RelayCommand<Object>(_ => Save(this));
        BuildCommand = new RelayCommand<Boolean>(async x => await BuildGameCodeDll(x),
            _ => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);

        OnPropertyChanged(nameof(AddSceneCommand));
        OnPropertyChanged(nameof(RemoveSceneCommand));
        OnPropertyChanged(nameof(UndoCommand));
        OnPropertyChanged(nameof(RedoCommand));
        OnPropertyChanged(nameof(SaveCommand));
        OnPropertyChanged(nameof(BuildCommand));
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
        Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
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

    private async Task BuildGameCodeDll(Boolean showWindow = true)
    {
        try
        {
            UnloadGameCodeDll();
            await Task.Run(() => VisualStudio.BuildSolution(this, GetConfigurationName(DllBuildConfig), showWindow));
            if (VisualStudio.BuildSucceeded)
                LoadGameCodeDll();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }
    }

    private void LoadGameCodeDll()
    {
        String configName = GetConfigurationName(DllBuildConfig);
        String dll = $@"{ProjectPath}x64\{configName}\{Name}.dll";

        if (File.Exists(dll) && EngineAPI.LoadGameCodeDll(dll) != 0)
            Logger.Log(MessageType.Info, "Game code DLL loaded successfully");
        else
            Logger.Log(MessageType.Warning, "Failed to load game code DLL file. Try building the project first");
    }

    private void UnloadGameCodeDll()
    {
        if (EngineAPI.UnloadGameCodeDll() != 0)
            Logger.Log(MessageType.Info, "Game code DLL unloaded");
    }
}
