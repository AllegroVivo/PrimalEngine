using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.Editors;

public partial class GameEntityView : UserControl
{
    private Action _undoAction;
    private String _propertyName;
    
    public static GameEntityView Instance { get; private set; }
    
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

    private void OnNameTextBox_GotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        _propertyName = String.Empty;
        _undoAction = GetRenameAction();
    }

    private void OnNameTextBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
        {
            Action redoAction = GetRenameAction();
            Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
            _propertyName = null;
        }

        _undoAction = null;
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

    private void OnIsEnabledCheckBox_Click(Object sender, RoutedEventArgs e)
    {
        Action undoAction = GetIsEnabledAction();
        MSEntity vm = DataContext as MSEntity;
        Boolean enable = (sender as CheckBox)!.IsChecked == true;
        vm!.IsEnabled = enable;
        Action redoAction = GetIsEnabledAction();
        Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, enable ? "Enable" : "Disable" + " game entity"));
    }
}

