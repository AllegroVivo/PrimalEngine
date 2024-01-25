using System;
using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.GameProject;

public partial class NewProjectView : UserControl
{
    public NewProjectView()
    {
        InitializeComponent();
    }

    private void OnCreateButton_Click(Object sender, RoutedEventArgs e)
    {
        NewProject vm = DataContext as NewProject;
        String projectPath = vm!.CreateProject(templatesListBox.SelectedItem as ProjectTemplate);
        Boolean dialogResult = false;
        Window win = Window.GetWindow(this);

        if (!String.IsNullOrEmpty(projectPath))
            dialogResult = true;

        win!.DialogResult = dialogResult;
        win.Close();
    }
}

