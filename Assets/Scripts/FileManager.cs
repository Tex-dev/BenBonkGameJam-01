using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages file from outside of the game.
/// </summary>
public class FileManager : MonoBehaviour
{
    /// <summary>
    /// Open file explorer from a specified path.
    /// </summary>
    /// <param name="path">Path to open the file from.</param>
    public void OpenFile(string path)
    {
        System.Diagnostics.Process.Start(path);
    }

    /// <summary>
    /// On "File" click.
    /// </summary>
    public void ClickFile()
    {
        // TODO : should open current map file.
        OpenFile(Application.dataPath + "/Levels/coucou.ini");
    }

    /// <summary>
    /// On "Folder" click.
    /// </summary>
    public void ClickFolder()
    {
        // TODO : should open current map folder.
        OpenFolder(Application.dataPath + "/Levels/");
    }

    /// <summary>
    /// Open a folder at a specified path.
    /// </summary>
    /// <param name="path">Path to open folder at.</param>
    public void OpenFolder(string path)
    {
        OpenInFileBrowser.Open(path);
    }

    /// <summary>
    /// Refresh map view from the file.
    /// </summary>
    public void RefreshFileView()
    {
    }

    /// <summary>
    /// Reset file, reloading the level from the default files.
    /// </summary>
    public void ResetFile()
    {
    }
}