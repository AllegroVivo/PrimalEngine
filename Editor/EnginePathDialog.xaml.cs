using System;
using System.IO;
using System.Windows;

namespace Editor;

public partial class EnginePathDialog : Window
{
    public String PrimalPath { get; private set; }
    
    public EnginePathDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
    }

    private void OnOKButton_Click(Object sender, RoutedEventArgs e)
    {
        String path = pathTextBox.Text.Trim();
        
        messageTextBlock.Text = String.Empty;
        if (String.IsNullOrEmpty(path))
            messageTextBlock.Text = "Invalid path";
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            messageTextBlock.Text = "Invalid characters used in path";
        else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI\")))
            messageTextBlock.Text = "Unable to find engine at specified location";

        if (String.IsNullOrEmpty(messageTextBlock.Text))
        {
            if (!Path.EndsInDirectorySeparator(path))
                path += @"\";

            PrimalPath = path;
            DialogResult = true;
            Close();
        }
    }
}

