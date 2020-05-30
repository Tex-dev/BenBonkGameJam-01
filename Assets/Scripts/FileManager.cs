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
        int currentLevel = LevelManager.CurrentLevel;

        OpenFile(Application.dataPath + $"/Resources/level_{currentLevel}/level_{currentLevel}_blocks.ini");
    }

    /// <summary>
    /// On "Folder" click.
    /// </summary>
    public void ClickFolder()
    {
        int currentLevel = LevelManager.CurrentLevel;

        OpenFolder(Application.dataPath + $"/Resources/level_{currentLevel}/");
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