using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.GameProject;

public partial class OpenProjectView : UserControl
{
    public OpenProjectView()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            var item = projectsListBox.ItemContainerGenerator
                .ContainerFromIndex(projectsListBox.SelectedIndex) as ListBoxItem;
            item?.Focus();
        };
    }

    private void OnOpenButton_Click(Object sender, RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        Project project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);
        Window win = Window.GetWindow(this);

        Boolean dialogResult = project != null;
        win!.DialogResult = dialogResult;
        win.DataContext = project;
        win.Close();
    }

    private void OnListBoxItem_DoubleClick(Object sender, MouseButtonEventArgs e)
    {
        OpenSelectedProject();
    }
}

