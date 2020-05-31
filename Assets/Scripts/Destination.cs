using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelLogic.Instance.DestinationReached();
    }
}