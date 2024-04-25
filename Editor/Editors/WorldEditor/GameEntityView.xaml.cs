using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Editors;

public partial class GameEntityView : UserControl
{
    public static GameEntityView Instance { get; private set; }

    private Action _undoAction;
    private String _propertyName;
    
    public GameEntityView()
    {
        InitializeComponent();
        DataContext = null;
        Instance = this;
        DataContextChanged += (_, _) =>
        {
            if (DataContext != null)
                (DataContext as MSEntity)!.PropertyChanged += (_, e) => _propertyName = e.PropertyName;
        };
    }

    private Action GetRenameAction()
    {
        MSEntity vm = DataContext as MSEntity;
        var selection = vm!.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
        return () =>
        {
            selection.ForEach(item => item.entity.Name = item.Name);
            (DataContext as MSEntity)!.Refresh();
        };
    }

    private Action GetIsEnabledAction()
    {
        MSEntity vm = DataContext as MSEntity;
        var selection = vm!.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
        return () =>
        {
            selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
            (DataContext as MSEntity)!.Refresh();
        };
    }

    private void OnNameTextBox_GainFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        _undoAction = GetRenameAction();
    }

    private void OnNameTextBox_LoseFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
        {
            Action redoAction = GetRenameAction();
            Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
            _propertyName = null;
        }

        _undoAction = null;
    }

    private void IsEnabledCheckBox_Click(Object sender, RoutedEventArgs e)
    {
        Action undoAction = GetIsEnabledAction();
        MSEntity vm = DataContext as MSEntity;
        vm!.IsEnabled = (sender as CheckBox)!.IsChecked == true;
        Action redoAction = GetIsEnabledAction();
        Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
    }
}

