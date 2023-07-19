using SimpleFileBrowser;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class used to control the file dialog
/// </summary>
public class FileBrowserController : MonoBehaviour
{
    /// <summary> Action performed on file loaded </summary>
    [SerializeField]
    public UnityAction<string> OnFileLoaded;

    UnityAction<string> OnFileCreated;
    /// <summary> Loaded path </summary>
    internal string loadedPath;

    /// <summary>
    /// Opens a file dialog loading folders
    /// </summary>
    /// <param name="path"> Default path </param>
    /// <param name="title"> Dialog title </param>
    /// <param name="evnt"> Action upon file loaded </param>
    public void OpenFolder(string path, string title, UnityAction<string> evnt)
    {
        OnFileLoaded = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowLoadDialogCoroutine(path, title));
        else
            StartCoroutine(ShowLoadDialogCoroutine(null, title));
    }

    /// <summary>
    /// Opens a file dialog and waits for user input. Loaded folder is then used in OnFileLoaded action
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator ShowLoadDialogCoroutine(string path, string title)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, path, null, title, "Open");

        Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success)
        {
            string folder = FileBrowser.Result[0];
            Debug.Log(FileBrowser.Result[0]);
            loadedPath = folder;
            OnFileLoaded.Invoke(loadedPath);
        }
    }

    /// <summary>
    /// Opens a file dialog loading files
    /// </summary>
    /// <param name="path"> Default path </param>
    /// <param name="title"> Title of dialog </param>
    /// <param name="evnt"> Event performed on file loaded </param>
    internal void OpenFile(string path, string title, UnityAction<string> evnt)
    {
        OnFileLoaded = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowLoadFileDialogCoroutine(path, title));
        else
            StartCoroutine(ShowLoadFileDialogCoroutine(null, title));
    }


    /// <summary>
    /// Opens a file dialog and waits for user input. Loaded file is then used in OnFileLoaded action 
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator ShowLoadFileDialogCoroutine(string path, string title)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, path, null, title, "Open");

        Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success)
        {
            string file = FileBrowser.Result[0];
            Debug.Log(FileBrowser.Result[0]);
            OnFileLoaded.Invoke(file);
        }
    }

    /// <summary>
    /// Opens a file dialog saving files
    /// </summary>
    /// <param name="path"> Default path </param>
    /// <param name="title"> Title of dialog </param>
    /// <param name="evnt"> Event performed on file loaded </param>
    internal void SaveFile(string path, string title, UnityAction<string> evnt)
    {
        OnFileCreated = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowSaveFileDialogCoroutine(path, title));
        else
            StartCoroutine(ShowSaveFileDialogCoroutine(null, title));
    }

    /// <summary>
    /// Opens a file dialog and waits for user input. Created file is then used in OnFileCreated action 
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator ShowSaveFileDialogCoroutine(string path, string title)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, path, null, title, "Save");

        Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success)
        {
            string file = FileBrowser.Result[0];
            Debug.Log(FileBrowser.Result[0]);
            OnFileCreated.Invoke(file);
        }
    }

}
