using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used to switch between czech and english languages
/// </summary>
public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    TMP_Text brushNMTXT;
    [SerializeField()]
    TMP_Text editBrushTXT;
    [SerializeField()]
    TMP_Text previewTXT;
    [SerializeField()]
    TMP_Text exportBrushTXT;
    [SerializeField()]
    TMP_Text helpTitleTXT;
    [SerializeField()]
    TMP_Text helpTXT;
    [SerializeField()]
    TMP_Text setDllTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button applyBT;
    [SerializeField()]
    Button exportBT;
    [SerializeField()]
    Button browseBT;
    [SerializeField()]
    Button browseDllBT;
    [SerializeField()]
    Button langBT;
    [SerializeField()]
    Button setDllBT;

    [Header("Input fields")]
    [SerializeField()]
    TMP_InputField brushName;
    [SerializeField()]
    TMP_InputField userCode;
    [SerializeField()]
    TMP_InputField path;
    [SerializeField()]
    TMP_InputField dllPath;

    [Header("Language")]
    internal string lang;

    [Header("Strings")]
    internal string noUserCode = "No user code written";

    internal string browseTitleEN = "Choose export folder";
    internal string browseTitleCZ = "Vybrat složku";

    string inputPromptCZ = "Napiš text...";
    string inputPromptEN = "Enter text...";

    string brushNMCZ = "Jméno štetce:";
    string brushNMEN = "Brush name:";

    string editBrushCZ = "Editovat štetec:";
    string editBrushEN = "Edit brush:";

    string previewCZ = "Náhled:";
    string previewEN = "Preview:";

    string exportbrushCZ = "Exportovat:";
    string exportbrushEN = "Export:";

    string setDllCZ = "Nastavit python.dll:";
    string setDllEN = "Set python.dll:";

    string applyCZ = "Provést";
    string applyEN = "Apply";

    string browseCZ = "Prohlížet...";
    string browseEN = "Browse...";

    string exportCZ = "Exportovat";
    string exportEN = "Export";

    string helpTCZ = "Specifinace Python kódu";
    string helpTEN = "Python Code Requirements";

    string helpCZ =
@"Uživatelský kód musi vracet python list
    [color, width, widthModifiers, time, texSize, pixels]
- color - list promìnných typu float z intervalu <0,1> ve tvaru [r, g, b]
- width - promìnná typu int z intervalu <1, 100>
- widthModifier - list promìnných typu float z intervalu <0, 1>
- time - promìnná typu int
- texSize - list promìnných typu int ve tvaru [šíøka, výška]
- pixels - list barev ([r, g, b] nebo [r, g, b, a]) délky šíøka*výška, povolené hodnoty 'a' jsou 0 nebo 1

texSize a pixels jsou volitelné návratové hodnoty
widthModifier = upravují velikost štìtce v urèitém bodì v èase
time = jak dlouho trvá pøi tažení štìtcem jeden prùchod listem widthModifier a texturou
pixels = pixely textury øádku po øádce";
    string helpEN = 
@"User code must return a python list
    [color, width, widthModifier, time, texSize, pixels]
- color - list of floats in interval<0, 1> [r, g, b]
- width - int in interval <1, 100>
- widthModifier - list of floats in interval <0, 1>
- time - int
- texSize - list of ints [width, height]
- pixels - list of colors ([r, g, b]/[r, g, b, a]) of length width* height, valid values of 'a' is 0 or 1

texSize and pixels are optional
widthModifier = modifies the width of line in given keyframe
time = how long will one loop through widthModifier and texture take
pixels = assembled into texture row by row";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels();
    }

    /// <summary>
    /// Get title for a browse window
    /// </summary>
    /// <returns> Title for a browse window </returns>
    internal string GetBrowseTitle()
    {
        if (lang == "CZ")
            return browseTitleCZ;
        return browseTitleEN;
    }

    /// <summary>
    /// Swap languages between CZ and EN
    /// </summary>
    public void SwapLanguage()
    {
        if (lang == "CZ")
            lang = "EN";
        else if (lang == "EN")
            lang = "CZ";

        SetLabels();
    }

    /// <summary>
    /// Set labels to czech or english texts
    /// </summary>
    private void SetLabels()
    {
        if (lang == "CZ")
        {
            brushNMTXT.text = brushNMCZ;
            editBrushTXT.text = editBrushCZ;
            previewTXT.text = previewCZ;
            exportBrushTXT.text = exportbrushCZ;
            helpTitleTXT.text = helpTCZ;
            helpTXT.text = helpCZ;
            setDllTXT.text = setDllCZ;

            applyBT.GetComponentInChildren<TMP_Text>().text = applyCZ;
            exportBT.GetComponentInChildren<TMP_Text>().text = exportCZ;
            browseBT.GetComponentInChildren<TMP_Text>().text = browseCZ;
            browseDllBT.GetComponentInChildren<TMP_Text>().text = browseCZ;
            setDllBT.GetComponentInChildren<TMP_Text>().text = applyCZ;
            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;

            brushName.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            userCode.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            path.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;
            dllPath.placeholder.GetComponent<TMP_Text>().text = inputPromptCZ;

            noUserCode = "Žádný kód k vykonání";
        }

        else if (langCZ == "EN")
        {
            brushNMTXT.text = brushNMEN;
            editBrushTXT.text = editBrushEN;
            previewTXT.text = previewEN;
            exportBrushTXT.text = exportbrushEN;
            helpTitleTXT.text = helpTEN;
            helpTXT.text = helpEN;
            setDllTXT.text = setDllEN;

            applyBT.GetComponentInChildren<TMP_Text>().text = applyEN;
            exportBT.GetComponentInChildren<TMP_Text>().text = exportEN;
            browseBT.GetComponentInChildren<TMP_Text>().text = browseEN;
            browseDllBT.GetComponentInChildren<TMP_Text>().text = browseEN;
            setDllBT.GetComponentInChildren<TMP_Text>().text = applyEN;
            langBT.GetComponentInChildren<TMP_Text>().text = langEN;

            userCode.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;

            brushName.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            userCode.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            path.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;
            dllPath.placeholder.GetComponent<TMP_Text>().text = inputPromptEN;

            noUserCode = "No user code to execute";
        }
    }
}
