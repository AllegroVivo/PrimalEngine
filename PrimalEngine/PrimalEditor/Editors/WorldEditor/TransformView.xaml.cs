using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.Editors;

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

    private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector,
        Action<(Transform transform, Vector3)> forEachAction)
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

    private Action GetPositionAction() 
        => GetAction(x => (x, x.Position), x => x.transform.Position = x.Item2);
    private Action GetRotationAction() 
        => GetAction(x => (x, x.Rotation), x => x.transform.Rotation = x.Item2);
    private Action GetScaleAction() 
        => GetAction(x => (x, x.Scale), x => x.transform.Scale = x.Item2);

    private void OnPositionVectorBox_PreviewMouseLBD(Object sender, MouseButtonEventArgs e)
    {
        _propertyChanged = false;
        _undoAction = GetPositionAction();
    }

    private void OnPositionVectorBox_PreviewMouseLBU(Object sender, MouseButtonEventArgs e)
    {
        RecordActions(GetPositionAction(), "Position Changed");
    }
    
    private void OnRotationVectorBox_PreviewMouseLBD(Object sender, MouseButtonEventArgs e)
    {
        _propertyChanged = false;
        _undoAction = GetRotationAction();
    }

    private void OnRotationVectorBox_PreviewMouseLBU(Object sender, MouseButtonEventArgs e)
    {
        RecordActions(GetRotationAction(), "Rotation Changed");
    }
    
    private void OnScaleVectorBox_PreviewMouseLBD(Object sender, MouseButtonEventArgs e)
    {
        _propertyChanged = false;
        _undoAction = GetScaleAction();
    }

    private void OnScaleVectorBox_PreviewMouseLBU(Object sender, MouseButtonEventArgs e)
    {
        RecordActions(GetScaleAction(), "Scale Changed");
    }

    private void RecordActions(Action redoAction, String name)
    {
        if (_propertyChanged)
        {
            Debug.Assert(_undoAction != null);
            _propertyChanged = false;
            Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
        }
    }
    
    private void OnPositionVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_propertyChanged && _undoAction != null)
        {
            OnPositionVectorBox_PreviewMouseLBU(sender, null);
        }
    }
    
    private void OnRotationVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_propertyChanged && _undoAction != null)
        {
            OnRotationVectorBox_PreviewMouseLBU(sender, null);
        }
    }
    
    private void OnScaleVectorBox_LostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_propertyChanged && _undoAction != null)
        {
            OnScaleVectorBox_PreviewMouseLBU(sender, null);
        }
    }
}

