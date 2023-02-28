using SimpleFileBrowser;
using System;
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
    public void OpenFolder(string path, string title, UnityAction<string> evnt)
    {
        OnFileLoaded = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowLoadDialogCoroutine(path, title));
        else
            StartCoroutine(ShowLoadDialogCoroutine(null, title));
    }

    /// <summary>
    /// Opens a file dialog and waits for user input. Loaded file is then parsed and object is added to an active world space.
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

    internal void OpenFile(string path, string title, UnityAction<string> evnt)
    {
        OnFileLoaded = evnt;

        if (Directory.Exists(path))
            StartCoroutine(ShowLoadFileDialogCoroutine(path, title));
        else
            StartCoroutine(ShowLoadFileDialogCoroutine(null, title));
    }


    /// <summary>
    /// Opens a file dialog and waits for user input. Loaded file is then parsed and object is added to an active world space.
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
}
