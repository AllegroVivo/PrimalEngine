using System;
using System.Windows;
using System.Windows.Controls;

namespace Editor.Utilities;

public partial class LoggerView : UserControl
{
    public LoggerView()
    {
        InitializeComponent();
    }

    private void OnClearLogButton_Click(Object sender, RoutedEventArgs e)
    {
        Logger.Clear();
    }

    private void OnFilterButton_Click(Object sender, RoutedEventArgs e)
    {
        Int32 filter = 0x00;
        
        if (toggleInfo.IsChecked == true)
            filter |= (Int32)MessageType.Info;
        if (toggleWarnings.IsChecked == true)
            filter |= (Int32)MessageType.Warning;
        if (toggleErrors.IsChecked == true)
            filter |= (Int32)MessageType.Error;

        Logger.SetMessageFilter(filter);
    }
}

