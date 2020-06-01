using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages logic of the levels system.
/// </summary>
public class LevelLogic : Singleton<LevelLogic>
{
    [SerializeField]
    private GameObject m_blackholePrefab = null;

    private GameObject m_endingBlackhole;

    public Action OnDestionationReached;

    /// <summary>
    /// Called on destination reach.
    /// </summary>
    public void DestinationReached(Transform destination)
    {
        m_endingBlackhole = Instantiate(m_blackholePrefab, destination);
        m_endingBlackhole.GetComponent<BlackHoleManager>().BeginLevel(false);

        OnDestionationReached?.Invoke();

        Debug.Log("Destination reached ! ");

    }

    public GameObject GetEndingBlackhole()
    {
        return m_endingBlackhole;
    }
}