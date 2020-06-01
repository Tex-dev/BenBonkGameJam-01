using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenButtonsManager : MonoBehaviour
{
    [SerializeField]
    private Button m_NewGameButton = null;

    [SerializeField]
    private Button m_ContinueButton = null;

    // Start is called before the first frame update
    private void Start()
    {
        if (Directory.Exists(Application.dataPath + $"/Resources/level_0/"))
        {
            m_ContinueButton.interactable = true;

            // TODO : connect features here
            m_NewGameButton.onClick.AddListener(() => LevelManager.Instance.LoadLevel(0, true));
            m_ContinueButton.onClick.AddListener(() => LevelManager.Instance.LoadLevel(0, false));
        }
        else
        {
            m_NewGameButton.onClick.AddListener(() => LevelManager.Instance.LoadLevel(0, true));
            m_ContinueButton.interactable = false;
        }
    }
}