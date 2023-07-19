using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class controlling the menu
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField()]
    DisplayLineController displayLineController;
    [SerializeField()]
    FileBrowserController fileBrowserController;
    [SerializeField()]
    UserCodeProcessor userCodeProcessor;
    [SerializeField()]
    LanguageController langController;
    [SerializeField()]
    ConsoleController consoleController;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField brushName;
    [SerializeField()]
    TMP_InputField path;
    [SerializeField()]
    TMP_InputField pathDll;
    [SerializeField()]
    TMP_InputField userCode;
    [SerializeField()]
    TMP_InputField codeLineNums;
    [SerializeField()]
    TMP_Text error;

    [Header("Input fields")]
    [SerializeField()]
    Button SetDllPathBT;
    [SerializeField()]
    Button ApplyBT;

    [Header("Game objects")]
    [SerializeField()]
    GameObject helpPanel;
    [SerializeField()]
    GameObject consolePanel;
    [SerializeField()]
    GameObject lineObject;

    Brush currentBrush;
    LineExporter le;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.width, 600, FullScreenMode.Windowed);
        lineObject.transform.position = new Vector3(0, 0, 0);
        error.text = "";

        // generate a brush with constant width and black color
        float[] yValues = new float[1];
        yValues[0] = 1;
        currentBrush = new Brush() { Width = 0.05f, Color = Color.black, Name = "SampleBrush", TimePerIter = 5, WidthModifier = yValues };

        // display the brush
        displayLineController.GenerateExampleLine(currentBrush);
        brushName.text = currentBrush.Name;
        path.text = Application.dataPath;
        pathDll.text = userCodeProcessor.pythonPath;

        le = new LineExporter();
    }

    private void SetMessageText(string text, bool isError)
    {
        if (isError)
        {
            error.text = "<color=\"red\">" + text + "</color>";
            consoleController.AppendErrorTextToConsole(text + "\n");
        }
        else
        {
            error.text = text;
            consoleController.AppendTextToConsole(text + "\n");
        }
    }

    /// <summary>
    /// Apply button clicked
    /// - interpret user code
    /// </summary>
    public void OnApplyBTClick()
    {
        string code = userCode.text;

        if (code.Trim().Length == 0)
        {
            SetMessageText(langController.noUserCode, true);
            return;
        }
        SetMessageText(langController.codeExecuting, false);

        StartCoroutine(RunCode(code));
    }

    /// <summary>
    /// Run user code coroutine
    /// </summary>
    /// <param name="code"> Code to run </param>
    private IEnumerator RunCode(string code)
    {
        ApplyBT.interactable = false;
        
        Task r = RunBrushCode(code);

        while (!r.IsCompleted)
            yield return null;

        if (currentBrush == null)
        {
            string erMsg = userCodeProcessor.ERROR_MSG;

            MatchCollection strs = Regex.Matches(erMsg, @"line \d+");
            foreach (Match l in strs)
            {
                string resultString = Regex.Match(l.Value, @"\d+").Value;
                int val = int.Parse(resultString) - 1;
                erMsg = Regex.Replace(erMsg, l.Value, "line " + val);
            }
            SetMessageText(erMsg, true);
        }
        else
        {
            currentBrush.Name = brushName.text;
            displayLineController.GenerateExampleLine(currentBrush);
            SetMessageText(langController.codeExecuted, false);
        }

        Debug.Log("Init status: " + userCodeProcessor.GetInitStatus());
        
        SetDllPathBT.interactable = false;
        ApplyBT.interactable = true;
    }

    /// <summary>
    /// Run user code - await task returning created brush
    /// </summary>
    /// <param name="code"> Code to run </param>
    /// <returns> Task </returns>
    async Task RunBrushCode(string code)
    {
        currentBrush = await userCodeProcessor.ExecuteCode(code);
    }

    /// <summary>
    /// Fix scrollbar position
    /// </summary>
    public void FixScroll()
    {
        if (userCode.caretPosition == userCode.text.Length && userCode.verticalScrollbar.size < 1)
        {
            Debug.Log("At end");
            StartCoroutine(FixCoroutine(userCode));
        }
    }

    /// <summary>
    /// Fix scrollbar position
    /// </summary>
    public void FixScrollConsole()
    {
        Debug.Log("At end");
        StartCoroutine(FixCoroutine(consoleController.consoleText));
    }

    /// <summary>
    /// Fix scrollbar position coroutine
    /// </summary>
    IEnumerator FixCoroutine(TMP_InputField field)
    {
        Canvas.ForceUpdateCanvases();
        yield return null;

        field.verticalScrollbar.value = 1f;
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Add line numbers to the line counter text field
    /// </summary>
    public void SyncLineNumbers()
    {
        int lineCount = Regex.Matches(userCode.text, "\n").Count;
        string newTxt = "";
        for (int i = 0; i < lineCount; i++)
        {
            newTxt += (i+1) + "\n";
        }
        newTxt += lineCount+1 + "";

        codeLineNums.text = newTxt; 
        codeLineNums.verticalScrollbar.value = userCode.verticalScrollbar.value;
    }

    /// <summary>
    /// React to brush name changed
    /// </summary>
    public void OnBrushNameChanged()
    {
        // escape string - only allows a-z, A-Z 0-9, _ and a space
        string newName = Regex.Replace(brushName.text, "[^a-zA-Z0-9_ -]+", "", RegexOptions.Compiled);
        
        // set name
        if (newName.Length != 0)
            currentBrush.Name = newName;

        brushName.text = newName;
    }

    /// <summary>
    /// React to browse button pressed
    /// - open folder explorer
    /// </summary>
    public void OnBrowseBT()
    {
        string p = path.text.Trim();
        fileBrowserController.OpenFolder(p, langController.GetBrowseTitle(), UpdatePathTXT);
    }

    /// <summary>
    /// React to browse button pressed
    /// - open file explorer
    /// </summary>
    public void OnBrowseDllBT()
    {
        string p = pathDll.text.Trim();
        fileBrowserController.OpenFile(p, langController.GetBrowseTitle(), UpdatePathDllTXT);
    }

    /// <summary>
    /// Update export path 
    /// </summary>
    /// <param name="p"> New export path </param>
    public void UpdatePathTXT(string p)
    {
        path.text = p;
    }

    /// <summary>
    /// Update python dll path 
    /// </summary>
    /// <param name="p"> New python dll path </param>
    public void UpdatePathDllTXT(string p)
    {
        pathDll.text = p;
    }

    /// <summary>
    /// Set path to python dll
    /// </summary>
    public void OnSetDllPathBT()
    {
        bool r = userCodeProcessor.ResetPythonPath(pathDll.text);
        if (!r)
        {
            Debug.Log("Wrong dll");
            SetMessageText(userCodeProcessor.ERROR_MSG, true);
        } else
        {
            SetMessageText(langController.dllSet, false);
            error.text = "";
        }
    }

    /// <summary>
    /// React to export button pressed
    /// - export brush and its texture
    /// </summary>
    public void OnExportBT()
    {
        if (currentBrush == null)
            SetMessageText(langController.noBrush, true);
        else
        {
            le.ExportBrush(currentBrush, path.text.Trim());
            SetMessageText(langController.brushExported, false);
        }
    }

    /// <summary>
    /// Toggle display of help panel
    /// </summary>
    /// <param name="value"> Should the panel be displayed or not </param>
    public void ToggleHelpPanel(bool value)
    {
        helpPanel.SetActive(value);
    }

    /// <summary>
    /// Toggle console
    /// </summary>
    public void ToggleConsole()
    {
        if (!consolePanel.activeSelf)
        {
            Screen.SetResolution(Screen.width, 800, FullScreenMode.Windowed);
            consolePanel.SetActive(true);
            lineObject.transform.position = new Vector3(0, 0.13f, 0.2f);
        }
        else
        {
            Screen.SetResolution(Screen.width, 600, FullScreenMode.Windowed);
            consolePanel.SetActive(false);
            lineObject.transform.position = new Vector3(0, 0, 0);
        }

    }

    /// <summary>
    /// Load code from file
    /// </summary>
    /// <param name="p"> Path to file </param>
    public void LoadFile(string p)
    {
        if (!File.Exists(p))
        {
            // message to console
            SetMessageText(langController.GetFileError(1), true);
            return;
        }

        if (!p.EndsWith(".py"))
        {
            // message to console
            SetMessageText(langController.GetFileError(2), true);
            return;
        }

        string newCode = File.ReadAllText(p);
        userCode.text = newCode;
        SetMessageText(langController.fileLoaded, false);
    }

    /// <summary>
    /// Load python code file
    /// </summary>
    public void OnLoadFile()
    {
        string p = Application.dataPath;
        fileBrowserController.OpenFile(p, "Load file", LoadFile);
    }

    /// <summary>
    /// Save code to file
    /// </summary>
    /// <param name="p">Path to file</param>
    private void SaveFile(string p)
    {
        if (!p.EndsWith(".py"))
            p += ".py";
        File.WriteAllText(p, userCode.text);
        SetMessageText(langController.fileSaved, false);
    }

    /// <summary>
    /// Save code to .py file
    /// </summary>
    public void OnSaveFile()
    {
        string p = Application.dataPath;
        fileBrowserController.SaveFile(p, "Save as file", SaveFile);
    }


    /// <summary>
    /// Swap language - from CZ to EN and vice versa
    /// </summary>
    public void SwapLanguage()
    {
        langController.SwapLanguage();
    }
}
