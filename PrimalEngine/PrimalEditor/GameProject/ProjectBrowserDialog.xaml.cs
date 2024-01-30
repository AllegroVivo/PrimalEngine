using System;
using System.Linq;
using System.Windows;

namespace PrimalEditor.GameProject;

public partial class ProjectBrowserDialog : Window
{
    public ProjectBrowserDialog()
    {
        InitializeComponent();
        Loaded += OnProjectBrowserDialogLoaded;
    }

    private void OnProjectBrowserDialogLoaded(Object sender, RoutedEventArgs e)
    {
        Loaded -= OnProjectBrowserDialogLoaded;

        if (!OpenProject.Projects.Any())
        {
            openProjectButton.IsEnabled = false;
            openProjectView.Visibility = Visibility.Hidden;
            OnToggleButton_OnClick(createProjectButton, new RoutedEventArgs());
        }
    }

    private void OnToggleButton_OnClick(Object sender, RoutedEventArgs e)
    {
        if (sender == openProjectButton)
        {
            if (createProjectButton.IsChecked == true)
            {
                createProjectButton.IsChecked = false;
                browserContent.Margin = new Thickness(0);
            }

            openProjectButton.IsChecked = true;
        }
        else
        {
            if (openProjectButton.IsChecked == true)
            {
                openProjectButton.IsChecked = false;
                browserContent.Margin = new Thickness(-800, 0, 0, 0);
            }

            createProjectButton.IsChecked = true;
        }
    }
}

