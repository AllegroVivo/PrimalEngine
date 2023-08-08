using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract]
public class ProjectTemplate
{
    [DataMember]
    public String ProjectType { get; set; }
    [DataMember]
    public String ProjectFile { get; set; }
    [DataMember]
    public List<String> Folders { get; set; }

    public Byte[] Icon { get; set; }
    public Byte[] Screenshot { get; set; }
    public String IconFilePath { get; set; }
    public String ScreenshotFilePath { get; set; }
    public String ProjectFilePath { get; set; }
}

class NewProject : ViewModelBase
{
    // TODO: get the path from the installation location
    private readonly String _templatePath = @"..\..\PrimalEditor\ProjectTemplates";
    private String _projectName = "NewProject";
    public String ProjectName
    {
        get => _projectName;
        set
        {
            if(_projectName != value)
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }
    }

    private String _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProject\";
    public String ProjectPath
    {
        get => _projectPath;
        set
        {
            if (_projectPath != value)
            {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
            }
        }
    }

    private ObservableCollection<ProjectTemplate> _projectTemplates = new();
    public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
    { get; }


    public NewProject()
    {
        ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
        try
        {
            String[] templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
            Debug.Assert(templatesFiles.Any());
            foreach (String file in templatesFiles)
            {
                ProjectTemplate template = Serializer.FromFile<ProjectTemplate>(file);
                
                template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                template.Icon = File.ReadAllBytes(template.IconFilePath);
                
                template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                
                template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                _projectTemplates.Add(template);
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            // TODO: log error
        }
    }
}