using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

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
        GameEntityView.Instance.DataContext = null;
        ListBox listBox = sender as ListBox;

        List<GameEntity> newSelection = listBox!.SelectedItems.Cast<GameEntity>().ToList();
        List<GameEntity> previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();
        
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

        MSGameEntity msEntity = null;
        if (newSelection.Any())
            msEntity = new MSGameEntity(newSelection);

        GameEntityView.Instance.DataContext = msEntity;
    }
}

