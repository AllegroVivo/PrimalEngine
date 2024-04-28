using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.Editors;

public partial class TransformView : UserControl
{
    private Action _undoAction;
    private Boolean _propertyChanged;
    
    public TransformView()
    {
        InitializeComponent();
        Loaded += OnTransformViewLoaded;
    }

    private void OnTransformViewLoaded(Object sender, RoutedEventArgs e)
    {
        Loaded -= OnTransformViewLoaded;
        (DataContext as MSTransform)!.PropertyChanged += (_, _) => _propertyChanged = true;
    }

    private Action GetAction()
    {
        _propertyChanged = false;
        
        if (DataContext is not MSTransform vm)
        {
            _undoAction = null;
            _propertyChanged = false;
            return null;
        }

        var selection = vm.SelectedComponents.Select(transform => (transform, transform.Position)).ToList();
        return () =>
        {
            selection.ForEach(item => item.transform.Position = item.Position);
            (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
        };
    }

    private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
    {
        if (DataContext is not MSTransform vm)
        {
            _undoAction = null;
            _propertyChanged = false;
            return null;
        }

        var selection = vm.SelectedComponents.Select(selector).ToList();
        return () =>
        {
            selection.ForEach(forEachAction);
            (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
        };
    }

    private Action GetPositionAction() => GetAction(x => (x, x.Position), x => x.transform.Position = x.Item2);
    private Action GetRotationAction() => GetAction(x => (x, x.Rotation), x => x.transform.Rotation = x.Item2);
    private Action GetScaleAction() => GetAction(x => (x, x.Scale), x => x.transform.Scale = x.Item2);

    private void RecordAction(Action redoAction, String name)
    {
        if (_propertyChanged)
        {
            Debug.Assert(_undoAction != null);
            _propertyChanged = false;
            Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
        }
    }
    
    private void OnPosVectorBox_PreviewMouseLBD(Object sender, MouseButtonEventArgs e)
    {
        _propertyChanged = false;
        _undoAction = GetPositionAction();
    }
    private void OnPosVectorBox_PreviewMouseLBU(Object _, MouseButtonEventArgs __) => RecordAction(GetPositionAction(), "Position changed");
    private void OnPosVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs _)
    {
        if (_propertyChanged && _undoAction != null)
            OnPosVectorBox_PreviewMouseLBU(sender, null);
    }

    private void OnRotVectorBox_PreviewMouseLBD(Object _, MouseButtonEventArgs __)
    {
        _propertyChanged = false;
        _undoAction = GetRotationAction();
    }
    private void OnRotVectorBox_PreviewMouseLBU(Object _, MouseButtonEventArgs __) => RecordAction(GetRotationAction(), "Rotation changed");
    private void OnRotVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs _)
    {
        if (_propertyChanged && _undoAction != null)
            OnRotVectorBox_PreviewMouseLBU(sender, null);
    }

    private void OnScaleVectorBox_PreviewMouseLBD(Object _, MouseButtonEventArgs __)
    {
        _propertyChanged = false;
        _undoAction = GetScaleAction();
    }
    private void OnScaleVectorBox_PreviewMouseLBU(Object _, MouseButtonEventArgs __) => RecordAction(GetScaleAction(), "Scale changed");
    private void OnScaleVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs _)
    {
        if (_propertyChanged && _undoAction != null)
            OnScaleVectorBox_PreviewMouseLBU(sender, null);
    }
}

