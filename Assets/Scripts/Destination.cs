using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other is BoxCollider2D)
            LevelLogic.Instance.DestinationReached();
    }
}