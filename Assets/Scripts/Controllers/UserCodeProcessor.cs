using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZCU.PythonExecutionLibrary;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for execution of user code
/// </summary>
public class UserCodeProcessor : MonoBehaviour
{
    /// <summary> Executor of python code </summary>
    PythonExecutor ex;
    
    /// <summary> Path to python.dll </summary>
    [SerializeField()]
    internal string pythonPath;
    [SerializeField()]
    ConsoleController consoleController;

    /// <summary> Last error message </summary>
    internal string ERROR_MSG;

    // Start is called before the first frame update
    void Start()
    {
        ex = new PythonExecutor();

        Debug.Log(pythonPath);

        // if in config wasnt valid dll path set default built in path
        bool res = ResetPythonPath(pythonPath);
        if (!res)
            pythonPath = Directory.GetCurrentDirectory() + "/Resources/python37.dll"; 

        ex.SetPython(pythonPath);
        Debug.Log(pythonPath);
    }

    /// <summary>
    /// Set new path to python.dll
    /// </summary>
    /// <param name="newpath"> New path to python.dll </param>
    /// <returns> True if successful, false if not </returns>
    public bool ResetPythonPath(string newpath)
    {
        if (File.Exists(newpath) && newpath.EndsWith(".dll")) {
            pythonPath = newpath;
            // setting python path can fail if there was a previous attempt to run code with different dll
            try
            {
                ex.SetPython(pythonPath);
            } catch (Exception e)
            {
                ERROR_MSG = e.Message;
                return false;
            }
            return true;
        }
        else
        {
            ERROR_MSG = "Invalid python dll path";
            return false;
        }
    }

    /// <summary>
    /// Create a function from user code and execute the created code
    /// </summary>
    /// <param name="code"> User code </param>
    /// <returns> Created brush </returns>
    public async Task<Brush> ExecuteCode(string code)
    {
        var stdoutWriter = new DispatchedTextWriter(consoleController, false);
        var stderrWriter = new DispatchedTextWriter(consoleController, true);

        List<string> paramNames = new List<string>();
        Dictionary<string, object> inVariables = new Dictionary<string, object>();
        string exCode = ex.CreateCode("userCode", paramNames, new List<string>(inVariables.Keys), code);

        Debug.Log(exCode);

        Brush b = await Task<Brush>.Run(() =>
        {
            Brush b = new Brush();

            try
            {
                // if python not yet initialized - initialize
                // if this fails, user needs to restart the whole app to reinit
                if (!ex.initializedOnce)
                {
                    ex.SetPython(pythonPath);
                    ex.Initialize();
                }
                ex.RunCode(exCode, inVariables, b, stdoutWriter, stderrWriter);
                Debug.Log("Code executed");

            }
            catch (Python.Runtime.PythonException e)
            {
                Debug.LogError("Python exception: " + e.Message);
                ERROR_MSG = e.Message; // pythonExecutor.ERROR_MSG;
                return null;
            }
            catch (Exception e)
            {
                // python runtime incorrectly set
                if (e.Message.Contains("Runtime.PythonDLL was not set or does not point to a supported Python runtime DLL"))
                {
                    Debug.LogError("Exception: " + e.Message);
                    ERROR_MSG = e.Message + "\nRestart of application might be needed.";
                }
                else
                {
                    Debug.LogError("Exception: " + e.Message);
                    ERROR_MSG = e.Message;
                }
                return null;
            }

            return b;
        });

        if (b != null)
            b.SetTexture();
        return b;
    }

    /// <summary>
    /// Was python already initialized
    /// </summary>
    /// <returns> True if initialized, false if not </returns>
    public bool GetInitStatus()
    {
        return ex.initializedOnce;
    }
}
