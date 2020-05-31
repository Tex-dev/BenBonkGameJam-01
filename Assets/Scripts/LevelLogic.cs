using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages logic of the levels system.
/// </summary>
public class LevelLogic : Singleton<LevelLogic>
{
    /// <summary>
    /// Called on destination reach.
    /// </summary>
    public void OnDestinationReached()
    {
        PlayerManager.Pause();

        Debug.Log("Destination reached ! ");
    }
}