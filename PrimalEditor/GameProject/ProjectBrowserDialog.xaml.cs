using System;
using System.Windows;

namespace PrimalEditor.GameProject;

public partial class ProjectBrowserDialog : Window
{
    public ProjectBrowserDialog()
    {
        InitializeComponent();
    }

    private void OnProjectButton_OnClick(Object sender, RoutedEventArgs e)
    {
        if (sender == openProjectButton)
        {
            if (newProjectButton.IsChecked == true)
            {
                newProjectButton.IsChecked = false;
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

            newProjectButton.IsChecked = true;
        }
    }
}

