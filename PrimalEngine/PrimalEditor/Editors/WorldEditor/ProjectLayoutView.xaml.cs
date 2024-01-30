using System;
using System.Windows;
using System.Windows.Controls;
using PrimalEditor.Components;
using PrimalEditor.GameProject;

namespace PrimalEditor.Editors;

public partial class ProjectLayoutView : UserControl
{
    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void AddGameEntityButton_Click(Object sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        Scene vm = btn!.DataContext as Scene;
        vm!.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
    }

    private void OnGameEntitiesListBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
        Object entity = (sender as ListBox)!.SelectedItems[0];
        GameEntityView.Instance.DataContext = entity;
    }
}

