using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Editor.Utilities;

namespace Editor.GameProject;

[DataContract]
public class ProjectData
{
    [DataMember]
    public String ProjectName { get; set; }
    [DataMember]
    public String ProjectPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }

    public String FullPath => $"{ProjectPath}{ProjectName}{Project.Extension}";
    
    public Byte[] Icon { get; set; }
    public Byte[] Screenshot { get; set; }
}

[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<ProjectData> Projects { get; set; }
}

class OpenProject
{
    private static readonly String _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PrimalEditor";
    private static readonly String _projectDataPath;

    private static readonly ObservableCollection<ProjectData> _projects = new();
    public static ReadOnlyObservableCollection<ProjectData> Projects { get; }
    
    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(_applicationDataPath))
                Directory.CreateDirectory(_applicationDataPath);

            _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
            Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

            ReadProjectData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static Project Open(ProjectData data)
    {
        ReadProjectData();
        ProjectData project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
        if (project != null)
        {
            project.Date = DateTime.Now;
        }
        else
        {
            project = data;
            project.Date = DateTime.Now;
            _projects.Add(project);
        }

        WriteProjectData();

        return Project.Load(project.FullPath);
    }
    
    private static void ReadProjectData()
    {
        if (File.Exists(_projectDataPath))
        {
            var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
            _projects.Clear();

            foreach (ProjectData project in projects)
            {
                if (File.Exists(project.FullPath))
                {
                    project.Icon = File.ReadAllBytes($@"{project.ProjectPath}.Primal\Icon.png");
                    project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}.Primal\Screenshot.png");
                    _projects.Add(project);
                }
            }
        }
        
        Debug.WriteLine("Finished Loading");
    }

    private static void WriteProjectData()
    {
        var projects = _projects.OrderBy(x => x.Date).ToList();
        Serializer.ToFile(new ProjectDataList { Projects = projects }, _projectDataPath);
    }
}