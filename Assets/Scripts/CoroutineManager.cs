using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages small coroutines.
/// </summary>
public class CoroutineManager : Singleton<CoroutineManager>
{
    /// <summary>
    /// Delay a given action.
    /// </summary>
    /// <param name="actionToDelay">Action to delay.</param>
    /// <param name="delay">Delay duration.</param>
    public static void Delay(Action actionToDelay, float delay)
    {
        Instance.StartCoroutine(Instance.c_LaunchAfter(actionToDelay, delay));
    }

    /// <summary>
    /// Cortouien for a delayed action.
    /// </summary>
    /// <param name="actionToDelay">Action to delay.</param>
    /// <param name="delay">Delay duration.</param>
    private IEnumerator c_LaunchAfter(Action actionToDelay, float delay)
    {
        yield return new WaitForSeconds(delay);

        actionToDelay?.Invoke();
    }
}