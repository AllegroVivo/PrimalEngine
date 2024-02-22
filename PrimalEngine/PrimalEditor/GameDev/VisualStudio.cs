using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameDev;

static class VisualStudio
{
    private static EnvDTE80.DTE2 _vsInstance = null;
    private static readonly String _progID = "VisualStudio.DTE.17.0";
    public static Boolean BuildSucceeded { get; private set; } = true;
    public static Boolean BuildDone { get; private set; } = true;

    [DllImport("ole32.dll")]
    private static extern Int32 CreateBindCtx(UInt32 reserved, out IBindCtx ppbc);

    [DllImport("ole32.dll")]
    private static extern Int32 GetRunningObjectTable(UInt32 reserved, out IRunningObjectTable pprot);

    public static void OpenVisualStudio(String solutionPath)
    {
        IRunningObjectTable rot = null;
        IEnumMoniker monikerTable = null;
        IBindCtx bindCtx = null;
        
        try
        {
            if (_vsInstance == null)
            {
                // Find and open visual
                Int32 hResult = GetRunningObjectTable(0, out rot);
                if (hResult < 0 || rot == null) 
                    throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:X8}");

                rot.EnumRunning(out monikerTable);
                monikerTable.Reset();

                hResult = CreateBindCtx(0, out bindCtx);
                if (hResult < 0 || bindCtx == null) 
                    throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:X8}");

                IMoniker[] currentMoniker = new IMoniker[1];
                while (monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
                {
                    String name = String.Empty;
                    currentMoniker[0]?.GetDisplayName(bindCtx, null, out name);
                    if(name.Contains(_progID))
                    {
                        hResult = rot.GetObject(currentMoniker[0], out Object obj);
                        if (hResult < 0 || obj == null) 
                            throw new COMException($"Running object table's GetObject() returned HRESULT: {hResult:X8}");

                        EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
                        String solutionName = dte!.Solution.FullName;
                        if(solutionName == solutionPath)
                        {
                            _vsInstance = dte;
                            break;
                        }
                    }
                }

                if (_vsInstance == null)
                {
                    Type visualStudioType = Type.GetTypeFromProgID(_progID, true);
                    _vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Logger.Log(MessageType.Error, "failed to open Visual Studio");
        }
        finally
        {
            if (monikerTable != null)
                Marshal.ReleaseComObject(monikerTable);
            if (rot != null)
                Marshal.ReleaseComObject(rot);
            if (bindCtx != null) 
                Marshal.ReleaseComObject(bindCtx);
        }
    }

    public static void CloseVisualStudio()
    {
        if (_vsInstance?.Solution.IsOpen == true)
        {
            _vsInstance.ExecuteCommand("File.SaveAll");
            _vsInstance.Solution.Close(true);
        }
            
        _vsInstance?.Quit();
    }

    public static Boolean AddFilesToSolution(String solution, String projectName, String[] files)
    {
        Debug.Assert(files?.Length > 0);
        OpenVisualStudio(solution);
            
        try
        {
            if(_vsInstance != null)
            {
                if (!_vsInstance.Solution.IsOpen) 
                    _vsInstance.Solution.Open(solution);
                else 
                    _vsInstance.ExecuteCommand("File.SaveAll");

                foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
                {
                    if(project.UniqueName.Contains(projectName))
                    {
                        foreach (var file in files)
                            project.ProjectItems.AddFromFile(file);
                    }
                }

                String cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
                if(!String.IsNullOrEmpty(cpp))
                    _vsInstance.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindTextView).Visible = true;
                    
                _vsInstance.MainWindow.Activate();
                _vsInstance.MainWindow.Visible = true;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine("failed to add files to Visual Studio project");
            return false;
        }
            
        return true;
    }

    public static Boolean IsDebugging()
    {
        Boolean result = false;

        for (Int32 i = 0; i < 3; i++)
        {
            try
            {
                result = _vsInstance != null &&
                         (_vsInstance.Debugger.CurrentProgram != null || _vsInstance.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgRunMode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (!result)
                    System.Threading.Thread.Sleep(1000);
            }
        }

        return result;
    }

    public static void BuildSolution(Project project, String configName, Boolean showWindow = true)
    {
        if (IsDebugging())
        {
            Logger.Log(MessageType.Error, "Visual Studio is currently running a process.");
            return;
        }

        OpenVisualStudio(project.Solution);
        BuildDone = BuildSucceeded = false;

        for (Int32 i = 0; i < 3; i++)
        {
            try
            {
                if (!_vsInstance.Solution.IsOpen)
                    _vsInstance.Solution.Open(project.Solution);

                _vsInstance.MainWindow.Visible = showWindow;

                _vsInstance.Events.BuildEvents.OnBuildProjConfigBegin += OnBuildSolutionBegin;
                _vsInstance.Events.BuildEvents.OnBuildProjConfigDone += OnBuildSolutionDone;

                try
                {
                    foreach (String pdbFile in Directory.GetFiles(Path.Combine($"{project.ProjectPath}", $@"x64\{configName}"), "*.pdb"))
                        File.Delete(pdbFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                _vsInstance.Solution.SolutionBuild.SolutionConfigurations.Item(configName).Activate();
                _vsInstance.ExecuteCommand("Build.BuildSolution");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine($"Attempt {i}: failed to build {project.Name}.");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    private static void OnBuildSolutionBegin(String project, String projectConfig, String platform, String solutionConfig)
    {
        Logger.Log(MessageType.Info, $"Building {project}, {projectConfig}, {platform}, {solutionConfig}");
    }

    private static void OnBuildSolutionDone(String project, String projectConfig, String platform, String solutionConfig, Boolean success)
    {
        if (BuildDone)
            return;
        
        if (success)
            Logger.Log(MessageType.Info, $"Building {projectConfig} configuration succeeded.");
        else
            Logger.Log(MessageType.Error, $"Building {projectConfig} configuration failed.");

        BuildDone = true;
        BuildSucceeded = success;
    }
}