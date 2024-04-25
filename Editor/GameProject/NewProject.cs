using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Editor.Utilities;

namespace Editor.GameProject;

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
    private readonly String _templatePath = @"..\..\Editor\ProjectTemplates\";
    
    private String _projectName = "NewProject";
    public String ProjectName
    {
        get => _projectName;
        set
        {
            if (_projectName != value)
            {
                _projectName = value;
                ValidateProjectPath();
                OnPropertyChanged(nameof(ProjectName));
            }
        }
    }

    private String _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProjects\";
    public String ProjectPath
    {
        get => _projectPath;
        set
        {
            if (_projectPath != value)
            {
                _projectPath = value;
                ValidateProjectPath();
                OnPropertyChanged(nameof(ProjectPath));
            }
        }
    }

    private ObservableCollection<ProjectTemplate> _projectTemplates = new();
    public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

    private Boolean _isValid;

    public Boolean IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    private String _errorMessage;

    public String ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }


    public NewProject()
    {
        ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
        try
        {
            String[] templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
            Debug.Assert(templateFiles.Any());
            foreach (String file in templateFiles)
            {
                ProjectTemplate template = Serializer.FromFile<ProjectTemplate>(file);
                template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                template.Icon = File.ReadAllBytes(template.IconFilePath);
                template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                _projectTemplates.Add(template);
            }

            ValidateProjectPath();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Logger.Log(MessageType.Error, "Failed to read project templates");
            throw;
        }
    }

    private Boolean ValidateProjectPath()
    {
        String path = ProjectPath;
        
        if (!Path.EndsInDirectorySeparator(path))
            path += @"\";
        path += $@"{ProjectName}\";

        IsValid = false;

        if (String.IsNullOrWhiteSpace(ProjectName.Trim()))
            ErrorMessage = "Type in a project name.";
        else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            ErrorMessage = "Invalid character(s) used in project name.";
        else if (String.IsNullOrWhiteSpace(ProjectPath.Trim()))
            ErrorMessage = "Type in a project path.";
        else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            ErrorMessage = "Invalid character(s) used in project path.";
        else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            ErrorMessage = "Selected project folder already exists and is not empty.";
        else
        {
            ErrorMessage = String.Empty;
            IsValid = true;
        }

        return IsValid;
    }

    public String CreateProject(ProjectTemplate template)
    {
        ValidateProjectPath();
        if (!IsValid)
            return String.Empty;
           
        if (!Path.EndsInDirectorySeparator(ProjectPath))
            ProjectPath += @"\";
        String path = $@"{ProjectPath}{ProjectName}\";

        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            foreach (String folder in template.Folders)
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));

            DirectoryInfo dirInfo = new(path + @".Primal\");
            dirInfo.Attributes |= FileAttributes.Hidden;
            
            File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
            File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

            String projectXml = File.ReadAllText(template.ProjectFilePath);
            projectXml = String.Format(projectXml, ProjectName, ProjectPath);
            String projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
            File.WriteAllText(projectPath, projectXml);

            return path;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Logger.Log(MessageType.Error, $"Failed to create {template.ProjectType}");
            throw;
        }
    }
}
