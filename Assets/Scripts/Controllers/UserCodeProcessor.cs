using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZCU.PythonExecutionLibrary;

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

    /// <summary> Last error message </summary>
    internal string ERROR_MSG;

    // Start is called before the first frame update
    void Start()
    {
        ex = new PythonExecutor();
        ex.SetPython(pythonPath);
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
            ex.SetPython(pythonPath);
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
    public Brush ExecuteCode(string code)
    {
        List<string> paramNames = new List<string>();
        Dictionary<string, object> inVariables = new Dictionary<string, object>();
        string exCode = ex.CreateCode("userCode", paramNames, inVariables, code);

        Debug.Log(exCode);

        Brush b = new Brush();
        bool res = ex.RunCode(exCode, inVariables, b);

        if (!res)
        {
            Debug.LogError(ex.ERROR_MSG);
            ERROR_MSG = ex.ERROR_MSG;
            return null;
        }

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
