using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1;

    [SerializeField]
    private bool m_isBounceInDamage = false;

    [SerializeField]
    private Transform[] m_waypoints = null;

    private Transform m_target;
    private int m_targetID;

    [SerializeField]
    private int m_damage = 10;

    [SerializeField]
    private Vector2 m_BounceVelocity = new Vector2(2000f, 80f);

    private bool m_isDead = false;

    private SpriteRenderer m_sprite;
    private Collider2D m_collider;

    static private Quaternion s_onTheBack = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);

    private void Start()
    {
        m_targetID = 0;
        m_target = m_waypoints[0];

        m_sprite = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (m_isDead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, s_onTheBack, 10.0f * Time.deltaTime);
        }
        else
        {
            Vector3 dir = m_target.position - transform.position;
            transform.Translate(dir.normalized * m_speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, m_target.position) < 0.3f)
            {
                m_targetID = (m_targetID + 1) % m_waypoints.Length;
                m_target = m_waypoints[m_targetID];

                m_sprite.flipX = !m_sprite.flipX;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_damage, m_isBounceInDamage))
                return;

            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            Vector2 direction = playerRigidbody.position - new Vector2(transform.position.x, transform.position.y);

            if (Mathf.Abs(Vector2.Angle(direction, Vector2.left)) < 90f)
                playerRigidbody.AddForce(new Vector2(-1 * m_BounceVelocity.x, m_BounceVelocity.y));
            else
                playerRigidbody.AddForce(new Vector2(m_BounceVelocity.x, m_BounceVelocity.y));
        }
    }

    public void Death()
    {
        m_isDead = true;
        m_collider.enabled = false;
    }
}