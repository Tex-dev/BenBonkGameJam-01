using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockStockViewer : MonoBehaviour
{
    /// <summary>
    /// Text displaying block stock.
    /// </summary>
    private Text m_BlockDisplay = null;

    /// <summary>
    /// Awake is called by Unity at initialization.
    /// </summary>
    private void Awake()
    {
        m_BlockDisplay = GetComponent<Text>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        m_BlockDisplay.text = LevelManager.Instance.NbBlockUsed.ToString("00") + " / " + LevelManager.Instance.MaxBlockPossible.ToString("99");

        if (LevelManager.Instance.NbBlockUsed > LevelManager.Instance.MaxBlockPossible)
        {
            m_BlockDisplay.color = Color.red;
        }
        else
            m_BlockDisplay.color = Color.black;
    }
}