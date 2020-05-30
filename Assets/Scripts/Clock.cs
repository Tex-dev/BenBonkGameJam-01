using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small clock script.
/// </summary>
[RequireComponent(typeof(Text))]
public class Clock : MonoBehaviour
{
    /// <summary>
    /// Clock display text.
    /// </summary>
    [SerializeField]
    private Text m_ClockDisplay = null;

    // Update is called once per frame
    private void Update()
    {
        m_ClockDisplay.text = System.DateTime.Now.ToString("HH : mm");
    }
}