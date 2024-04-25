using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Editors;

public partial class ProjectLayoutView : UserControl
{
    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void OnAddGameEntityButton_Click(Object sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        Scene vm = btn!.DataContext as Scene;
        vm!.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity"});
    }

    private void OnGEListBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
        GameEntityView.Instance.DataContext = null;
        if (e.AddedItems.Count > 0)
            GameEntityView.Instance.DataContext = (sender as ListBox)!.SelectedItems[0];

        ListBox listBox = sender as ListBox;
        var newSelection = listBox!.SelectedItems.Cast<GameEntity>().ToList();
        var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();
        
        Project.UndoRedo.Add(new UndoRedoAction(
            () =>
            {
                listBox.UnselectAll();
                previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem)!.IsSelected = true);
            },
            () =>
            {
                listBox.UnselectAll();
                newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem)!.IsSelected = true);
            },
            "Selection changed"
            ));
    }
}

