﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameDev;

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
                                                        : primal::script::entity_script{{entity}} 
                                                        {{
                                                        }}
                                                        
                                                    void begin_play() override;
                                                    void update(float dt) override;
                                                    
                                                private:
                                                
                                                }};
                                            
                                            }}
                                            """;

    private static readonly String _namespace = GetNamespaceFromProjectName();

    private static String GetNamespaceFromProjectName()
    {
        String projectName = Project.Current.Name;
        projectName = projectName.Replace(' ', '_');
        return projectName;
    }

    public NewScriptDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
        scriptPath.Text = @"GameCode\";
    }

    private Boolean Validate()
    {
        String name = scriptName.Text.Trim();
        String path = scriptPath.Text.Trim();
        
        Boolean isValid = false;
        String errorMsg = String.Empty;
        
        if (String.IsNullOrEmpty(name))
            errorMsg = "Type in a script name.";
        else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(Char.IsWhiteSpace))
            errorMsg = "Invalid character(s) used in script name.";
        else if (String.IsNullOrEmpty(path))
            errorMsg = "Select a valid script folder";
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            errorMsg = "Invalid character(s) used in script path.";
        else if (!Path.GetFullPath(Path.Combine(Project.Current.ProjectPath, path)).Contains(Path.Combine(Project.Current.ProjectPath, @"GameCode\")))
            errorMsg = "Script must be added to (a sub-folder of) GameCode.";
        else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.ProjectPath, path), $"{name}.cpp"))) ||
                 File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.ProjectPath, path), $"{name}.h"))))
            errorMsg = $"script {name} already exists in this folder.";
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
        
        busyAnimation.Opacity = 0;
        busyAnimation.Visibility = Visibility.Visible;
        DoubleAnimation fadeIn = new(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
        busyAnimation.BeginAnimation(OpacityProperty, fadeIn);
        
        IsEnabled = false;

        try
        {
            String name = scriptName.Text.Trim();
            String path = Path.GetFullPath(Path.Combine(Project.Current.ProjectPath, scriptPath.Text.Trim()));
            String solution = Project.Current.Solution;
            String projectName = Project.Current.Name;

            await Task.Run(() => CreateScript(name, path, solution, projectName));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Logger.Log(MessageType.Error, $"Failed to create script: {scriptName.Text}");
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
        {
            sw.Write(_cppCode, name, _namespace);
        }
        
        using (StreamWriter sw = File.CreateText(h))
        {
            sw.Write(_hCode, name, _namespace);
        }

        String[] files = { cpp, h };

        for (Int32 i = 0; i < 3; i++)
        {
            if (!VisualStudio.AddFilesToSolution(solution, projectName, files))
                Thread.Sleep(1000);
            else
                break;
        }
    }
}

