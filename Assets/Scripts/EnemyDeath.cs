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
            print("JE ME MEEEEUUUUUUURRRRT");

            collision.gameObject.GetComponent<PlayerManager>().Jump(200);
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponentInParent<EnemyManager>().Death();
        }
    }
}
