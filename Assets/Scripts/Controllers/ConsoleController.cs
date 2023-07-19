using TMPro;
using UnityEngine;

public class ConsoleController : MonoBehaviour
{
    [SerializeField()]
    public TMP_InputField consoleText;

    public void AppendTextToConsole(string text)
    {
        consoleText.text += $"<color=white>{text}</color>";
    }

    public void AppendErrorTextToConsole(string text)
    {
        consoleText.text += $"<color=red>{text}</color>";
    }
}
