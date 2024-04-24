using System;
using System.Windows;
using System.Windows.Controls;

namespace Editor.GameProject;

public partial class NewProjectView : UserControl
{
    public NewProjectView()
    {
        InitializeComponent();
    }

    private void OnCreateButton_Click(Object sender, RoutedEventArgs e)
    {
        NewProject vm = DataContext as NewProject;
        String projectPath = vm!.CreateProject(templateListBox.SelectedItem as ProjectTemplate);
        
        Boolean dialogResult = false;
        Window win = Window.GetWindow(this);
        
        if (!String.IsNullOrEmpty(projectPath))
        {
            dialogResult = true;
            Project project = OpenProject.Open(new ProjectData { ProjectName = vm.ProjectName, ProjectPath = projectPath });
            win!.DataContext = project;
        }

        win!.DialogResult = dialogResult;
        win.Close();
    }
}

