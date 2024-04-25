﻿using System;
using System.Windows.Input;

namespace Editor;

class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;
    
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public Boolean CanExecute(Object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
    public void Execute(Object parameter) => _execute((T)parameter);
}
