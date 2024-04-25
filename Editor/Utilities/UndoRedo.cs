﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Editor.Utilities;

public interface IUndoRedo
{
    String Name { get; }
    
    void Undo();
    void Redo();
}

public class UndoRedoAction : IUndoRedo
{
    private Action _undoAction;
    private Action _redoAction;

    public String Name { get; }

    public UndoRedoAction(String name)
    {
        Name = name;
    }

    public UndoRedoAction(Action undo, Action redo, String name)
        : this(name)
    {
        Debug.Assert(undo != null && redo != null);
        _undoAction = undo;
        _redoAction = redo;
    }

    public UndoRedoAction(String property, Object instance, Object undoValue, Object redoValue, String name)
        : this(
            () => instance.GetType().GetProperty(property)!.SetValue(instance, undoValue),
            () => instance.GetType().GetProperty(property)!.SetValue(instance, redoValue),
            name
        )
    { }

    public void Undo() => _undoAction();

    public void Redo() => _redoAction();
}

public class UndoRedo
{
    private Boolean _enableAdd = true;
    
    private readonly ObservableCollection<IUndoRedo> _redoList = new();
    private readonly ObservableCollection<IUndoRedo> _undoList = new();
    
    public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
    public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

    public UndoRedo()
    {
        RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
        UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
    }

    public void Reset()
    {
        _redoList.Clear();
        _undoList.Clear();
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
        if (_enableAdd)
        {
            _undoList.Add(cmd);
            _redoList.Clear();
        }
    }
}