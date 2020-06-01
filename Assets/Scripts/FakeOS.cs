using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages file from outside of the game.
/// </summary>
public class FakeOS : Singleton<FakeOS>
{
    /// <summary>
    /// Pause menu game object.
    /// </summary>
    [SerializeField]
    private GameObject m_PauseMenu = null;

    /// <summary>
    /// Process of the current file viewer.
    /// </summary>
    private System.Diagnostics.Process m_CurrentFileViewProcess = null;

    /// <summary>
    /// Process of the current folder explorer.
    /// </summary>
    private System.Diagnostics.Process m_CurrentFolderViewProcess = null;

    /// <summary>
    /// Update is called once per frame by Unity.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            ClickFile();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            ClickFolder();
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            ResetFile();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePauseMenu(!PlayerManager.Instance.IsPaused);
        }
    }

    /// <summary>
    /// Toggle the pause menu.
    /// </summary>
    /// <param name="display">Should the pause menu be displayed?</param>
    public void TogglePauseMenu(bool display)
    {
        m_PauseMenu.SetActive(display);

        if (!display)
            PlayerManager.Play();
        else
            PlayerManager.Pause();
    }

    /// <summary>
    /// Called when the quit button is pressed.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Called when volume slider is moved.
    /// </summary>
    /// <param name="volume">new volume value.</param>
    public void OnVolumeChange(float volume)
    {
        AudioManager.Instance.ChangeVolumeMax(volume);
    }

    /// <summary>
    /// Open file explorer from a specified path.
    /// </summary>
    /// <param name="path">Path to open the file from.</param>
    public void OpenFile(string path)
    {
        m_CurrentFileViewProcess = System.Diagnostics.Process.Start(path);
    }

    /// <summary>
    /// Close current file viewer process.
    /// </summary>
    public void CloseFile()
    {
        try
        {
            m_CurrentFileViewProcess?.Kill();
        }
        catch
        {
        }
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

        OpenFolder(Application.dataPath + $"/Resources/");
    }

    /// <summary>
    /// Open a folder at a specified path.
    /// </summary>
    /// <param name="path">Path to open folder at.</param>
    public void OpenFolder(string path)
    {
        m_CurrentFolderViewProcess = OpenInFileBrowser.Open(path);
    }

    /// <summary>
    /// Close the current folder explorer process.
    /// </summary>
    public void CloseFolder()
    {
        try
        {
            m_CurrentFolderViewProcess?.Kill();
        }
        catch { }
    }

    /// <summary>
    /// Reset file, reloading the level from the default files.
    /// </summary>
    public void ResetFile()
    {
        LevelManager.Instance.LoadLevel(LevelManager.CurrentLevel);
    }
}