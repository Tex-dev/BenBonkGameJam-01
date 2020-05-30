using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : Singleton<Destination>
{
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
}