﻿using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    [SerializeField]
    private bool m_isBounceInDamage;

    [SerializeField]
    private Transform[] m_waypoints;
    private Transform m_target;
    private int m_targetID;

    [SerializeField]
    private int m_damage;

    private bool m_isDead = false;

    private SpriteRenderer  m_sprite;
    private BoxCollider2D   m_collider;

    static private Quaternion  s_onTheBack = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);

    private void Start()
    {
        m_targetID = 0;
        m_target = m_waypoints[0];

        m_sprite = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
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
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_damage, m_isBounceInDamage);
        }
    }

    public void Death()
    {
        m_isDead = true;
        m_collider.enabled = false;
    }


}