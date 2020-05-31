using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField]
    private float m_moveSpeed = 300f;

    [SerializeField]
    private float m_jumpForce = 250f;

    [SerializeField]
    private Transform m_groundCheck = null;

    [SerializeField]
    private float m_groundCheckRadius = 0.1f;

    [SerializeField]
    private LayerMask m_collisionLayer;

    private Rigidbody2D m_rigidBody = null;

    private SpriteRenderer m_spriteRenderer = null;

    private Vector2 m_velocity = Vector2.zero;

    private bool m_isJumping = false;

    private bool m_isGrounded = true;

    private float m_horizontalMovement;

    [SerializeField]
    private int m_nbJumpMax = 0;

    [SerializeField]
    private int m_nbJump = 0;

    private Animator m_animator = null;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool m_IsPaused = false;

    /// <summary>
    /// Is the game done starting.
    /// </summary>
    private bool m_IsGameStarted = false;

    public Action OnPlay = null;
    public Action OnPause = null;

    // HACK : this is not that good, but could be improved for clean pause system.
    //private Vector3 m_SavedPlayerVelocity = Vector3.zero;
    //private float m_SavedPlayerAngularVelocity = 0f;

    /// <summary>
    /// Play the game.
    /// </summary>
    public static void Play()
    {
        if (!Instance.m_IsGameStarted)
        {
            return;
        }
        Instance.OnPlay?.Invoke();

        Instance.m_IsPaused = false;
        Instance.m_animator.enabled = true;

        Time.timeScale = 1f;

        // HACK : this is not that good, but could be improved for clean pause system.
        //CoroutineManager.FixedDelay(() =>
        //{
        //    Instance.m_rigidBody.isKinematic = false;
        //    Instance.m_rigidBody.AddForce(Instance.m_SavedPlayerVelocity, ForceMode2D.Force);
        //    Instance.m_rigidBody.AddTorque(Instance.m_SavedPlayerAngularVelocity, ForceMode2D.Force);
        //});
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    public static void Pause()
    {
        if (!Instance.m_IsGameStarted)
        {
            return;
        }
        Instance.OnPause?.Invoke();

        Instance.m_IsPaused = true;
        Instance.m_animator.enabled = false;

        Time.timeScale = 0f;

        // HACK : this is not that good, but could be improved for clean pause system.
        //Instance.m_SavedPlayerVelocity = Instance.m_rigidBody.velocity;
        //Instance.m_SavedPlayerAngularVelocity = Instance.m_rigidBody.angularVelocity;
        //Instance.m_rigidBody.isKinematic = true;
    }

    /// <summary>
    /// Awake is called by Unity at initialization.
    /// </summary>
    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_nbJump = m_nbJumpMax;

        m_IsGameStarted = true;
    }

    /// <summary>
    /// Update is called every frame by Unity.
    /// </summary>
    private void Update()
    {
        if (!m_IsPaused)
        {
            m_horizontalMovement = Input.GetAxis("Horizontal") * m_moveSpeed * Time.fixedDeltaTime;

            if (Input.GetButtonDown("Jump") && m_isGrounded)
            //        if(Input.GetKeyDown(KeyCode.UpArrow) && m_nbJump > 0)
            {
                m_isJumping = true;
                m_nbJump--;
            }

            Flip(m_rigidBody.velocity.x);

            m_animator.SetFloat("speed", Mathf.Abs(m_rigidBody.velocity.x));
            m_animator.SetFloat("verticalSpeed", m_rigidBody.velocity.y);
            m_animator.SetBool("isGrounded", m_isGrounded);
            m_animator.SetBool("isJumping", m_isJumping);
        }
    }

    /// <summary>
    /// Fixed update is called by Unity every fixed update time.
    /// </summary>
    private void FixedUpdate()
    {
        MovePlayer(m_horizontalMovement);

        if (m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_collisionLayer))
            m_nbJump = m_nbJumpMax;
    }

    private void MovePlayer(float p_horizontalMovement)
    {
        Vector2 targetVelocity = new Vector2(p_horizontalMovement, m_rigidBody.velocity.y);
        m_rigidBody.velocity = Vector2.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref m_velocity, 0.05f);

        if (m_isJumping)
        {
            Jump(m_jumpForce);
            m_isJumping = false;
        }
    }

    private void Flip(float p_velocity)
    {
        if (p_velocity > 0.1f)
            m_spriteRenderer.flipX = false;

        if (p_velocity < -0.1f)
            m_spriteRenderer.flipX = true;
    }

    public void Jump(float p_force)
    {
        m_rigidBody.AddForce(new Vector2(0.0f, p_force));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_groundCheck.position, m_groundCheckRadius);
    }
}