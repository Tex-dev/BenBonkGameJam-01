using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    private int m_maxHealth = 100;
    private int m_health;

    [SerializeField]
    private HealthBarManager m_healthBar = null;

    private SpriteRenderer m_graphics;
    private Rigidbody2D m_rigidbody2D;

    private bool m_isInvisible = false;

    [SerializeField]
    private float m_invisibilityFramesDelay = 0.2f;

    [SerializeField]
    private float m_invisibilityDelay = 2.0f;

    /// <summary>
    /// Called on player death.
    /// </summary>
    public Action OnDeath = null;

    /// <summary>
    /// Awake is called by Unity at initialization.
    /// </summary>
    private void Awake()
    {
        m_health = m_maxHealth;
        m_healthBar.SetMaxHealth(m_maxHealth);

        m_graphics = GetComponentInChildren<SpriteRenderer>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();

        OnDeath += () => PlayerManager.Instance.AudioSource.PlayOneShot(PlayerManager.Instance.OnDeath);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(20, true);

        if (m_health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    /// <param name="damage">Damage value to apply to the player.</param>
    /// <param name="shouldBounce">Should the player bounce off the enemy?</param>
    public void TakeDamage(int damage, bool shouldBounce)
    {
        if (!m_isInvisible)
        {
            m_health -= damage;
            m_healthBar.SetHealth(m_health);

            m_isInvisible = true;

            StartCoroutine(InvisibilityFrames());

            PlayerManager.Instance.AudioSource.PlayOneShot(PlayerManager.Instance.OnHit);
        }
    }

    /// <summary>
    /// Animate the payer invisibility frames.
    /// </summary>
    public IEnumerator InvisibilityFrames()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        m_isInvisible = true;

        while (stopwatch.ElapsedMilliseconds < m_invisibilityDelay * 1000f)
        {
            m_graphics.color = new Color(m_graphics.color.r, m_graphics.color.g, m_graphics.color.b, 0.0f);
            yield return new WaitForSeconds(m_invisibilityFramesDelay);

            m_graphics.color = new Color(m_graphics.color.r, m_graphics.color.g, m_graphics.color.b, 1.0f);
            yield return new WaitForSeconds(m_invisibilityFramesDelay);
        }

        m_isInvisible = false;
    }
}