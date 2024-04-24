using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Editor;

[DataContract(IsReference = true)]
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(String propertyName) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
