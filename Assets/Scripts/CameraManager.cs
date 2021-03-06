﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_player = null;

    [SerializeField]
    private float m_timeOffset = 0.2f;

    [SerializeField]
    private Vector3 m_posOffset = Vector3.zero;

    private Rigidbody2D m_playerRB;

    private Vector3 m_velocity = Vector3.zero;

    private bool m_followPlayer = true;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Called when the scene is  loaded.
    /// </summary>
    /// <param name="scene">Loaded scene.</param>
    /// <param name="mode">Scene loading mode.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_player = PlayerManager.Instance.transform;
        m_playerRB = m_player.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_playerRB = m_player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_followPlayer)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, m_player.position + m_posOffset, ref m_velocity, m_timeOffset);

        /*
        if (Input.GetKeyDown(KeyCode.DownArrow))
            m_posOffset.y -= 2.5f;

        if (Input.GetKeyUp(KeyCode.DownArrow))
            m_posOffset.y += 2.5f;//*/

        if (m_playerRB.velocity.magnitude > 10.0f)
            m_timeOffset = Mathf.Max(0.0f, -0.02f * (m_playerRB.velocity.magnitude - 20.0f));
        else
            m_timeOffset = 0.2f;
    }

    public void FollowPlayer(bool enable = true)
    {
        m_followPlayer = enable;
    }
}