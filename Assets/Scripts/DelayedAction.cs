using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// From inspector action delayer.
/// </summary>
public class DelayedAction : MonoBehaviour
{
    /// <summary>
    /// Delay before the action starts.
    /// </summary>
    [SerializeField]
    private float m_Delay = 0f;

    /// <summary>
    /// Action to delay.
    /// </summary>
    [SerializeField]
    private UnityEvent m_DelayedAction = null;

    /// <summary>
    /// Start is called by Unity after initialization.
    /// </summary>
    private void Start()
    {
        StartCoroutine(c_DelayAction());
    }

    /// <summary>
    /// Delay the action.
    /// </summary>
    private IEnumerator c_DelayAction()
    {
        yield return new WaitForSeconds(m_Delay);

        m_DelayedAction?.Invoke();
    }
}