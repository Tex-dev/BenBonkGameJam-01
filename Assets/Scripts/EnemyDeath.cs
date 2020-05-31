using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private GameObject      m_enemyGO;

    private void Start()
    {
        m_enemyGO = transform.parent.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().Jump(150);
            GetComponentInParent<EnemyManager>().Death();
        }
    }
}
