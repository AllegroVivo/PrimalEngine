using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace PrimalEditor.Utilities;

public interface IUndoRedo
{
    public String Name { get; }

    public void Undo();
    public void Redo();
}

public class UndoRedoAction : IUndoRedo
{
    private Action _undoAction;
    private Action _redoAction;
    
    public String Name { get; }
    
    public void Undo() => _undoAction();
    public void Redo() => _redoAction();

    private UndoRedoAction(String name)
    {
        Name = name;
    }

    public UndoRedoAction(Action undoAction, Action redoAction, String name)
        : this(name)
    {
        Debug.Assert(undoAction != null && redoAction != null);
        _undoAction = undoAction;
        _redoAction = redoAction;
    }

    public UndoRedoAction(String property, Object instance, Object undoValue, Object redoValue, String name) :
        this(
            ()=> instance.GetType().GetProperty(property).SetValue(instance, undoValue),
            ()=>instance.GetType().GetProperty(property).SetValue(instance, redoValue),
            name)
    {
        
    }
}

public class UndoRedo
{
    private Boolean _enableAdd = true;
    
    private ObservableCollection<IUndoRedo> _undoList = new();
    private ObservableCollection<IUndoRedo> _redoList = new();
    
    public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }
    public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }

    public UndoRedo()
    {
        UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
    }
    
    public void Reset()
    {
        _undoList.Clear();
        _redoList.Clear();
    }

    public void Undo()
    {
        if (_undoList.Any())
        {
            IUndoRedo cmd = _undoList.Last();
            _undoList.RemoveAt(_undoList.Count - 1);
            _enableAdd = false;
            cmd.Undo();
            _enableAdd = true;
            _redoList.Insert(0, cmd);
        }
    }

    public void Redo()
    {
        if (_redoList.Any())
        {
            IUndoRedo cmd = _redoList.First();
            _redoList.RemoveAt(0);
            _enableAdd = false;
            cmd.Redo();
            _enableAdd = true;
            _undoList.Add(cmd);
        }
    }

    public void Add(IUndoRedo cmd)
    {
        if(_enableAdd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
        }
    }
}