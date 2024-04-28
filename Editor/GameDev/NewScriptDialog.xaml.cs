using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.GameDev;

public partial class NewScriptDialog : Window
{
    private static readonly String _cppCode = """
                                              #include "{0}.h"

                                              namespace {1}
                                              {{
                                                  REGISTER_SCRIPT({0});
                                                  void {0}::begin_play()
                                                  {{
                                                      
                                                  }}
                                                  
                                                  void {0}::update(float dt)
                                                  {{
                                                  
                                                  }}
                                              }}
                                              """;
    private static readonly String _hCode = """
                                            #pragma once

                                            namespace {1}
                                            {{
                                                class {0} : public primal::script::entity_script
                                                {{
                                                public:
                                                    constexpr explicit {0}(primal::game_entity::entity entity)
                                                        : primal::script::entity_script{{ entity }} {{ }}
                                            
                                                    void begin_play() override;
                                                    void update(float dt) override;
                                                    
                                                private:
                                                    
                                                }};
                                            }}
                                            """;
    
    private static readonly String _namespace = GetNamespaceFromProjectName();
    
    public NewScriptDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
        scriptPath.Text = @"GameCode\";
    }
    
    private Boolean Validate()
    {
        Boolean isValid = false;
        
        String name = scriptName.Text.Trim();
        String path = scriptPath.Text.Trim();
        String errorMsg = String.Empty;

        if (String.IsNullOrEmpty(name))
            errorMsg = "You must provide a script name";
        else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(Char.IsWhiteSpace))
            errorMsg = "Invalid character(s) in script name";
        else if (String.IsNullOrEmpty(path))
            errorMsg = "You must provide a script path";
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            errorMsg = "Invalid character(s) in script path";
        else if (!Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"GameCode\")))
            errorMsg = "Script must be added to [a sub-folder of] GameCode";
        else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) ||
                 File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
            errorMsg = $"Script {name} already exists in this directory";
        else
            isValid = true;

        messageTextBlock.Foreground = !isValid ? FindResource("Editor.RedBrush") as Brush : FindResource("Editor.FontBrush") as Brush;
        messageTextBlock.Text = errorMsg;

        return isValid;
    }

    private void OnScriptNameTextBox_TextChanged(Object sender, TextChangedEventArgs e)
    {
        if (!Validate())
            return;

        String name = scriptName.Text.Trim();
        messageTextBlock.Text = $"{name}.h and {name}.cpp will be added to {Project.Current.Name}";
    }

    private void OnScriptPathTextBox_TextChanged(Object sender, TextChangedEventArgs e)
    {
        Validate();
    }

    private async void OnCreateButton_Click(Object sender, RoutedEventArgs e)
    {
        if (!Validate())
            return;

        IsEnabled = false;
        busyAnimation.Opacity = 0;
        busyAnimation.Visibility = Visibility.Visible;
        DoubleAnimation fadeIn = new(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
        busyAnimation.BeginAnimation(OpacityProperty, fadeIn);

        try
        {
            String name = scriptName.Text.Trim();
            String path = Path.GetFullPath(Path.Combine(Project.Current.Path, scriptPath.Text.Trim()));
            String solution = Project.Current.Solution;
            String projectName = Project.Current.Name;

            await Task.Run(() => CreateScript(name, path, solution, projectName));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Logger.Log(MessageType.Error, $"$Failed to create script {scriptName.Text}");
        }
        finally
        {
            DoubleAnimation fadeOut = new(1, 0, new Duration(TimeSpan.FromMilliseconds(200)));
            fadeOut.Completed += (_, _) =>
            {
                busyAnimation.Opacity = 0;
                busyAnimation.Visibility = Visibility.Hidden;
                Close();
            };
            busyAnimation.BeginAnimation(OpacityProperty, fadeOut);
        }
    }

    private void CreateScript(String name, String path, String solution, String projectName)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        String cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
        String h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

        using (StreamWriter sw = File.CreateText(cpp))
            sw.Write(_cppCode, name, _namespace);
        
        using (StreamWriter sw = File.CreateText(h))
            sw.Write(_hCode, name, _namespace);

        String[] files = { cpp, h };

        for (Int32 i = 0; i < 3; i++)
        {
            if (!VisualStudio.AddFilesToSolution(solution, projectName, files))
                Thread.Sleep(1000);
            else
                break;
        }
    }
    
    private static String GetNamespaceFromProjectName()
    {
        String projectName = Project.Current.Name;
        return projectName.Replace(' ', '_');
    }
}

