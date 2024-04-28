using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Editor.Utilities;

namespace Editor.GameDev;

static class VisualStudio
{
    private static EnvDTE80.DTE2 _vsInstance = null;
    private static readonly String _progID = "VisualStudio.DTE.17.0";

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
            var hResult = GetRunningObjectTable(0, out rot);
            if (hResult < 0 || rot == null)
                throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:x8}");

            rot.EnumRunning(out monikerTable);
            monikerTable.Reset();

            hResult = CreateBindCtx(0, out bindCtx);
            if (hResult < 0 || bindCtx == null)
                throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:x8}");

            IMoniker[] currentMoniker = new IMoniker[1];
            while (monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
            {
                String name = String.Empty;
                currentMoniker[0]?.GetDisplayName(bindCtx, null, out name);
                if (name.Contains(_progID))
                {
                    hResult = rot.GetObject(currentMoniker[0], out Object obj);
                    if (hResult < 0 || obj == null)
                        throw new COMException($"RunningObjectTable.GetObject() returned HRESULT: {hResult:x8}");

                    EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
                    String solutionName = dte!.Solution.FullName;
                    if (solutionName == solutionPath)
                    {
                        _vsInstance = dte;
                        break;
                    }
                }
            }

            if (_vsInstance == null)
            {
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
            Logger.Log(MessageType.Error, "Failed to open Visual Studio");
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
            if (_vsInstance != null)
            {
                if (!_vsInstance.Solution.IsOpen)
                    _vsInstance.Solution.Open(solution);
                else
                    _vsInstance.ExecuteCommand("File.SaveAll");

                foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
                {
                    if (project.UniqueName.Contains(projectName))
                    {
                        foreach (String file in files)
                            project.ProjectItems.AddFromFile(file);
                    }
                }

                String cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
                if (String.IsNullOrEmpty(cpp))
                    _vsInstance.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindTextView).Visible = true;
                
                _vsInstance.MainWindow.Activate();
                _vsInstance.MainWindow.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine("Failed to add filed to VS Project");
            return false;
        }

        return true;
    }
}