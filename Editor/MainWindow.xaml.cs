﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Editor.GameProject;
using Path = System.IO.Path;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static String PrimalPath { get; private set; } = @"D:\PrimalEngine";
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }
        
        private void OnMainWindowLoaded(Object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void OnMainWindowClosing(Object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private void GetEnginePath()
        {
            String enginePath = Environment.GetEnvironmentVariable("PRIMAL_ENGINE_PATH", EnvironmentVariableTarget.User);
            if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI")))
            {
                EnginePathDialog dlg = new();
                if (dlg.ShowDialog() == true)
                {
                    PrimalPath = dlg.PrimalPath;
                    Environment.SetEnvironmentVariable("PRIMAL_ENGINE_PATH", PrimalPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                PrimalPath = enginePath;
            }
        }

        private void OpenProjectBrowserDialog()
        {
            ProjectBrowserDialog projectBrowser = new();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
                Application.Current.Shutdown();
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
