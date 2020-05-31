using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destination.
/// </summary>
public class Destination : MonoBehaviour
{
    /// <summary>
    /// Called by Unity on trigger enter with other.
    /// </summary>
    /// <param name="other">Collider triggering.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelLogic.Instance.OnDestinationReached();
    }
}