using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonExecutionLibrary;
using System.IO;

public class UserCodeProcessor : MonoBehaviour
{
    PythonExecutor ex;
    
    [SerializeField()]
    internal string pythonPath;

    internal string ERROR_MSG;

    // Start is called before the first frame update
    void Start()
    {
        ex = new PythonExecutor();
        ex.SetPython(pythonPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
}
