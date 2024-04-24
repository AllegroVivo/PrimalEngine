using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Editor.GameProject;

[DataContract]
public class Scene : ViewModelBase
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
    }
}