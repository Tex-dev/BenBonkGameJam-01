using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenButtonsManager : MonoBehaviour
{
    /// <summary>
    /// New game button.
    /// </summary>
    [SerializeField]
    private Button m_NewGameButton = null;

    /// <summary>
    /// Continue button.
    /// </summary>
    [SerializeField]
    private Button m_ContinueButton = null;

    /// <summary>
    /// Is continue available.
    /// </summary>
    private bool m_ContinueAvailable => Directory.Exists(Application.dataPath + "/Resources/level_0/");

    /// <summary>
    /// Start is called by Unity after initialization.
    /// </summary>
    private void Start()
    {
        m_NewGameButton.onClick.AddListener(ResetGame);
        m_ContinueButton.onClick.AddListener(ContinueGame);

        if (m_ContinueAvailable)
        {
            m_ContinueButton.gameObject.SetActive(true);
        }
        else
        {
            m_ContinueButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Resets the game.
    /// </summary>
    private void ResetGame()
    {
        if (m_ContinueAvailable)
            Directory.Delete(Application.dataPath + "/Resources/", true);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Resume the game to the last level load.
    /// </summary>
    private void ContinueGame()
    {
        SceneManager.LoadScene(1);
        SceneManager.LoadScene(Directory.GetDirectories(Application.dataPath + "/Resources/").Length);
    }
}