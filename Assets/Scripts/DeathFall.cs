using UnityEngine;

public class DeathFall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            LevelManager.Instance.LoadLevel(LevelManager.CurrentLevel);
        }
    }
}
