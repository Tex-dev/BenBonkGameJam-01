using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    private int                     m_maxHealth = 100;
    private int                     m_health;

    [SerializeField]
    private HealthBarManager        m_healthBar;

    private SpriteRenderer          m_graphics;
    private Rigidbody2D             m_rigidbody2D;

    private bool                    m_isInvisible = false;

    [SerializeField]
    private float                   m_invisibilityFramesDelay;

    [SerializeField]
    private float                   m_invisibilityDelay;


    void Start()
    {
        m_health = m_maxHealth;
        m_healthBar.SetMaxHealth(m_maxHealth);

        m_graphics = GetComponentInChildren<SpriteRenderer>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(20, true);

    }

    public void TakeDamage(int p_damage, bool p_isBounceEffect)
    {
        if(!m_isInvisible)
        {
            m_health -= p_damage;
            m_healthBar.SetHealth(m_health);

            m_isInvisible = true;

            StartCoroutine(InvisibilityFrames());
            StartCoroutine(HandleInvisibilityDelay());
        }
    }

    public IEnumerator InvisibilityFrames()
    {
        while(m_isInvisible)
        {
            m_graphics.color = new Color(m_graphics.color.r, m_graphics.color.g, m_graphics.color.b, 0.0f);
            yield return new WaitForSeconds(m_invisibilityFramesDelay);

            m_graphics.color = new Color(m_graphics.color.r, m_graphics.color.g, m_graphics.color.b, 1.0f);
            yield return new WaitForSeconds(m_invisibilityFramesDelay);
        }
    }

    public IEnumerator HandleInvisibilityDelay()
    {
        yield return new WaitForSeconds(m_invisibilityDelay);
        m_isInvisible = false;
    }
}
