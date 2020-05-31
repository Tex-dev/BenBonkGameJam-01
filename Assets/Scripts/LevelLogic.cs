using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogic : Singleton<LevelLogic>
{
    public void DestinationReached()
    {
        PlayerManager.Pause();

        Debug.Log("Destination reached ! ");
    }
}