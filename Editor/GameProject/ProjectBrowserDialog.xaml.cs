using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Editor.GameProject;

public partial class ProjectBrowserDialog : Window
{
    private readonly CubicEase _easing = new() { EasingMode = EasingMode.EaseInOut };
    
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
            ToggleProjectButton_OnClick(createProjectButton, new RoutedEventArgs());
        }
    }

    private void ToggleProjectButton_OnClick(Object sender, RoutedEventArgs e)
    {
        if (sender == openProjectButton)
        {
            if (createProjectButton.IsChecked == true)
            {
                createProjectButton.IsChecked = false;
                AnimateToOpenProject();
                newProjectView.IsEnabled = false;
                openProjectView.IsEnabled = true;
                browserContent.Margin = new Thickness(0);
            }

            openProjectButton.IsChecked = true;
        }
        else
        {
            if (openProjectButton.IsChecked == true)
            {
                openProjectButton.IsChecked = false;
                AnimateToCreateProject();
                newProjectView.IsEnabled = true;
                openProjectView.IsEnabled = false;
                browserContent.Margin = new Thickness(-800, 0, 0, 0);
            }

            createProjectButton.IsChecked = true;
        }
    }

    private void AnimateToCreateProject()
    {
        DoubleAnimation highlightAnimation = new(200, 400, new Duration(TimeSpan.FromSeconds(0.2)))
        {
            EasingFunction = _easing
        };
        highlightAnimation.Completed += (_, _) =>
        {
            ThicknessAnimation animation = new(new Thickness(0), 
                new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)))
            {
                EasingFunction = _easing
            };
            browserContent.BeginAnimation(MarginProperty, animation);
        };
        highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
    }

    private void AnimateToOpenProject()
    {
        DoubleAnimation highlightAnimation = new(400, 200, new Duration(TimeSpan.FromSeconds(0.2)))
        {
            EasingFunction = _easing
        };
        highlightAnimation.Completed += (_, _) =>
        {
            ThicknessAnimation animation = new(new Thickness(-1600, 0, 0, 0), 
                new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)))
            {
                EasingFunction = _easing
            };
            browserContent.BeginAnimation(MarginProperty, animation);
        };
        highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
    }
}

