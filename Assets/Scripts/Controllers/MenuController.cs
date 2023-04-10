using System.Collections;
using System.Text.RegularExpressions;
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

    [Header("Game objects")]
    [SerializeField()]
    GameObject helpPanel;

    Brush currentBrush;
    LineExporter le;

    // Start is called before the first frame update
    void Start()
    {
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

    /// <summary>
    /// Apply button clicked
    /// - interpret user code
    /// </summary>
    public void OnApplyBTClick()
    {
        string code = userCode.text;

        if (code.Trim().Length == 0)
        {
            error.text = langController.noUserCode;
            return;
        }

        currentBrush = userCodeProcessor.ExecuteCode(code);

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
            error.text = erMsg;
        }
        else
        {
            currentBrush.Name = brushName.text;
            displayLineController.GenerateExampleLine(currentBrush);
            error.text = "";
        }

        if (userCodeProcessor.GetInitStatus())
        {
            SetDllPathBT.interactable = false;
        }
    }

    /// <summary>
    /// Fix scrollbar position
    /// </summary>
    public void FixScroll()
    {
        if (userCode.caretPosition == userCode.text.Length && userCode.verticalScrollbar.size < 1)
        {
            Debug.Log("At end");
            StartCoroutine(FixCoroutine());
        }
    }

    /// <summary>
    /// Fix scrollbar position coroutine
    /// </summary>
    IEnumerator FixCoroutine()
    {
        Canvas.ForceUpdateCanvases();
        yield return null;

        userCode.verticalScrollbar.value = 1f;
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
            error.text = userCodeProcessor.ERROR_MSG;
        } else
        {
            error.text = "";
        }
    }

    /// <summary>
    /// React to export button pressed
    /// - export brush and its texture
    /// </summary>
    public void OnExportBT()
    {
        le.ExportBrush(currentBrush, path.text.Trim());
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
    /// Swap language - from CZ to EN and vice versa
    /// </summary>
    public void SwapLanguage()
    {
        langController.SwapLanguage();
    }
}
