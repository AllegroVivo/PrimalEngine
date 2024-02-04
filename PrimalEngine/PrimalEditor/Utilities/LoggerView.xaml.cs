using System;
using System.Windows;
using System.Windows.Controls;

namespace PrimalEditor.Utilities;

public partial class LoggerView : UserControl
{
    public LoggerView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            Logger.Log(MessageType.Info, "Information");
            Logger.Log(MessageType.Warning, "Warning");
            Logger.Log(MessageType.Error, "Error!");
        };
    }

    private void OnClearButton_Click(Object sender, RoutedEventArgs e)
    {
        Logger.Clear();
    }

    private void OnMessageFilterButton_Click(Object sender, RoutedEventArgs e)
    {
        Int32 filter = 0x00;
        if (toggleInfo.IsChecked == true)
            filter |= (Int32)MessageType.Info;
        if (toggleWarning.IsChecked == true)
            filter |= (Int32)MessageType.Warning;
        if (toggleError.IsChecked == true)
            filter |= (Int32)MessageType.Error;
        Logger.SetMessageFilter(filter);
    }
}

