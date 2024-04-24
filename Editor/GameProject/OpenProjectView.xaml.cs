using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor.GameProject;

public partial class OpenProjectView : UserControl
{
    public OpenProjectView()
    {
        InitializeComponent();
    }

    private void OnOpenButton_Click(Object sender, RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OnProjectsListBox_DoubleClick(Object sender, MouseButtonEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        Project project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);
        
        Boolean dialogResult = false;
        Window win = Window.GetWindow(this);

        if (project != null)
        {
            dialogResult = true;
            win!.DataContext = project;
        }

        win!.DialogResult = dialogResult;
        win.Close();
    }
}

