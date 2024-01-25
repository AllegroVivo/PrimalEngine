using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PrimalEditor.GameProject;

[DataContract(Name = "Game")]
public class Project : ViewModelBase
{
    public static String Extension = ".primal";
    
    [DataMember]
    public String Name { get; private set; }
    [DataMember]
    public String ProjectPath { get; private set; }

    public String FullPath => $"{ProjectPath}{Name}{Extension}";

    [DataMember(Name = "Scenes")]
    private ObservableCollection<Scene> _scenes = new();
    public ReadOnlyObservableCollection<Scene> Scenes { get; }

    public Project(String name, String path)
    {
        Name = name;
        ProjectPath = path;

        _scenes.Add(new Scene(this, "Default Scene"));
    }
}