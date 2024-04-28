using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Editor.GameDev;
using Editor.GameProject;

namespace Editor.Editors;

public partial class WorldEditorView : UserControl
{
    public WorldEditorView()
    {
        InitializeComponent();
        Loaded += OnWorldEditorViewLoaded;
    }

    private void OnWorldEditorViewLoaded(Object sender, RoutedEventArgs e)
    {
        Loaded -= OnWorldEditorViewLoaded;
        Focus();
    }

    private void OnNewScriptButton_Click(Object sender, RoutedEventArgs e)
    {
        new NewScriptDialog().ShowDialog();
    }
}

