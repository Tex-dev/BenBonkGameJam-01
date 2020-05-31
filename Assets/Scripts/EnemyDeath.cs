using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    /// <summary>
    /// Called by Unity when a collision is triggered.
    /// </summary>
    /// <param name="collision">Collision info.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Mathf.Abs(Vector2.Angle(collision.GetComponent<Rigidbody2D>().velocity, Vector2.down)) > 80f)
                return;

            GetComponent<AudioSource>().Play();

            collision.gameObject.GetComponent<PlayerManager>().Jump(200, true);
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponentInParent<EnemyManager>().Death();
        }
    }
}