using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using Editor.Utilities;

namespace Editor.GameProject;

[DataContract(Name = "Game")]
public class Project : ViewModelBase
{
    public static String Extension = ".primal";

    [DataMember] 
    public String Name { get; private set; } = "New Project";
    [DataMember]
    public String Path { get; private set; }
    public String FullPath => $"{Path}{Name}{Extension}";

    public static Project Current => Application.Current.MainWindow!.DataContext as Project;

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

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (_scenes != null)
        {
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));
        }

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
    }
}