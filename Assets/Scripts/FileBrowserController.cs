using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class FileBrowserController : MonoBehaviour
{
    public UnityAction<string> OnFileLoaded;

    internal string loadedPath;

    /// <summary>
    /// Opens a file dialog.
    /// </summary>
    public void OpenFolder(string path, UnityAction<string> evnt)
    {
        OnFileLoaded = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowLoadDialogCoroutine(path));
        else
            StartCoroutine(ShowLoadDialogCoroutine(null));
    }

    /// <summary>
    /// Opens a file dialog and waits for user input. Loaded file is then parsed and object is added to an active world space.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator ShowLoadDialogCoroutine(string path)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, path, null, "Choose export folder", "Open");

        Debug.Log(FileBrowser.Success);
        if (FileBrowser.Success)
        {
            string folder = FileBrowser.Result[0];
            Debug.Log(FileBrowser.Result[0]);
            loadedPath = folder;
            OnFileLoaded.Invoke(loadedPath);
        }
    }
}
