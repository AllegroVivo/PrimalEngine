using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace Editor.Utilities;

public enum MessageType
{
    Info = 0x01,
    Warning = 0x02,
    Error = 0x04
}

public class LogMessage
{
    public DateTime Time { get; }
    public MessageType MessageType { get; }
    public String Message { get; }
    public String File { get; }
    public String Caller { get; }
    public Int32 Line { get; }

    public String MetaData => $"{File}: {Caller} ({Line})";

    public LogMessage(MessageType type, String msg, String file, String caller, Int32 line)
    {
        Time = DateTime.Now;
        MessageType = type;
        Message = msg;
        File = file;
        Caller = caller;
        Line = line;
    }
}

public static class Logger
{
    private static Int32 _messageFilter = (Int32)(MessageType.Info | MessageType.Warning | MessageType.Error);
    
    private static readonly ObservableCollection<LogMessage> _messages = new();
    public static ReadOnlyObservableCollection<LogMessage> Messages { get; } = new(_messages);
    
    public static CollectionViewSource FilteredMessages { get; } = new() { Source = Messages };

    public static async void Log(MessageType type, String msg, [CallerFilePath] String file = "", 
        [CallerMemberName] String caller= "", [CallerLineNumber] Int32 line= 0)
    {
        await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            _messages.Add(new LogMessage(type, msg, file, caller, line));
        }));
    }

    public static async void Clear()
    {
        await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            _messages.Clear();
        }));
    }

    public static void SetMessageFilter(Int32 mask)
    {
        _messageFilter = mask;
        FilteredMessages.View.Refresh();
    }

    static Logger()
    {
        FilteredMessages.Filter += (_, e) =>
        {
            Int32 type = (Int32)(e.Item as LogMessage)!.MessageType;
            e.Accepted = (type & _messageFilter) != 0;
        };
    }
}