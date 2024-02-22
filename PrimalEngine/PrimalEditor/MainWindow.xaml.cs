using System;
using System.Collections.Generic;
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
using PrimalEditor.GameProject;
using Path = System.IO.Path;

namespace PrimalEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closed += OnMainWindowClosed;
        }

        public static String PrimalPath { get; private set; }

        private void OnMainWindowLoaded(Object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            String primalPath = Environment.GetEnvironmentVariable("PRIMAL_ENGINE", EnvironmentVariableTarget.User);
            if (primalPath == null || !Directory.Exists(Path.Combine(primalPath, @"Engine\EngineAPI")))
            {
                EnginePathDialog dlg = new();
                if (dlg.ShowDialog() == true)
                {
                    PrimalPath = dlg.PrimalPath;
                    Environment.SetEnvironmentVariable("PRIMAL_ENGINE", PrimalPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                PrimalPath = primalPath;
            }
        }

        private void OpenProjectBrowserDialog()
        {
            ProjectBrowserDialog projectBrowser = new();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }

        private void OnMainWindowClosed(Object sender, EventArgs e)
        {
            Closed -= OnMainWindowClosed;
            Project.Current?.Unload();
        }
    }
}
