using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Audio")]
    public AudioClip OnHit = null;

    public AudioClip OnJump = null;

    public AudioClip OnDeath = null;

    public AudioSource Source = null;

    [Header("Physic parameters")]
    [SerializeField]
    private float m_moveSpeed = 300f;

    [SerializeField]
    private float m_jumpForce = 250f;

    [SerializeField]
    private Transform m_groundCheck = null;

    [SerializeField]
    private float m_groundCheckRadius = 0.1f;

    [SerializeField]
    private LayerMask m_collisionLayer = ~0;

    private Rigidbody2D m_rigidBody = null;

    private SpriteRenderer m_spriteRenderer = null;

    private CameraManager m_cameraManager;

    private Vector2 m_velocity = Vector2.zero;

    private bool m_isJumping = false;
    private bool m_isGrounded = true;
    private bool m_canMove = true;

    private float m_horizontalMovement;

    [SerializeField]
    private int m_nbJumpMax = 0;

    [SerializeField]
    private int m_nbJump = 0;

    [SerializeField]
    private GameObject m_wings;

    private Animator m_animator = null;

    private IEnumerator m_coroutineAnim = null;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool m_IsPaused = false;

    /// <summary>
    /// Is the game done starting.
    /// </summary>
    private bool m_IsGameStarted = false;

    /// <summary>
    /// Called when play mode is activated.
    /// </summary>
    public Action OnPlay = null;

    /// <summary>
    /// Called when pause mode is activated.
    /// </summary>
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
//        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_animator = GetComponentInChildren<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();

        m_cameraManager = Camera.main.GetComponent<CameraManager>();

        m_nbJump = m_nbJumpMax;

        m_IsGameStarted = true;

        LevelLogic.Instance.OnDestionationReached += () => PlayerAnimation(true);
        GetComponent<PlayerHealth>().OnDeath += Death;

    }

    /// <summary>
    /// Update is called every frame by Unity.
    /// </summary>
    private void Update()
    {
        if (!m_IsPaused && m_canMove)
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
        if (!m_canMove)
            return;

        Vector2 targetVelocity = new Vector2(p_horizontalMovement, m_rigidBody.velocity.y);
        m_rigidBody.velocity = Vector2.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref m_velocity, 0.05f);

        if (m_isJumping && m_canMove)
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

    public void Jump(float p_force, bool mute = false)
    {
        print("coucou");
        m_rigidBody.AddForce(new Vector2(0.0f, p_force));

        if (!mute)
            Source.PlayOneShot(OnJump);
    }

    private void Death()
    {
        Source.PlayOneShot(OnDeath);
        m_wings.SetActive(true);
        m_animator.SetBool("isDead", true);

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        GetComponent<PlayerHealth>().ForceStopInvisibility();

        m_rigidBody.gravityScale = 0;
        m_rigidBody.velocity = Vector2.up;

        m_cameraManager.FollowPlayer(false);


        CoroutineManager.Delay(() => LevelManager.Instance.LoadLevel(LevelManager.CurrentLevel, false, true), 1.8f);
        CoroutineManager.Delay(DisableMovement, 1.8f);
        CoroutineManager.Delay(EnableMovement, 2.5f);
    }

    public void Respawn()
    {
        GetComponent<PlayerHealth>().Respawn();
        m_wings.SetActive(false);
        m_animator.SetBool("isDead", false);

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = true;

 //       m_rigidBody.gravityScale = 1;
 //       m_rigidBody.velocity = Vector2.zero;

        m_cameraManager.FollowPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_groundCheck.position, m_groundCheckRadius);
    }

    private void EnableMovement()
    {
        m_canMove = true;

        m_rigidBody.gravityScale = 1;
    }

    private void DisableMovement()
    {
        m_canMove = false;

        m_rigidBody.velocity = Vector2.zero;
        m_rigidBody.gravityScale = 0;
    }

    public void PlayerAnimation(bool isEndAnim)
    {
        if (m_coroutineAnim != null)
            StopCoroutine(m_coroutineAnim);

        m_coroutineAnim = SpiralAnimation(isEndAnim);
        StartCoroutine(m_coroutineAnim);

        if (isEndAnim)
            DisableMovement();
    }

    private IEnumerator SpiralAnimation(bool diminution)
    {
        // Not initialise here bacause at the loading of the level LevelLogic is not instantiate
        GameObject dest = null;
        bool isFInished = false;

        if (diminution)
        {
            dest = LevelLogic.Instance.GetEndingBlackhole();

            while (m_spriteRenderer.transform.localScale.magnitude > 0.1f)
            {
                m_spriteRenderer.transform.localScale = m_spriteRenderer.transform.localScale * 0.992f;
                m_spriteRenderer.transform.Rotate(0.0f, 0.0f, 3.0f, Space.Self);
                m_spriteRenderer.transform.position = Vector3.Lerp(m_spriteRenderer.transform.position, dest.transform.position, Time.deltaTime);
                yield return 1;
            }
        }
        else
        {
            // Boolean to know if scale if finished and finish the turn of rotation;
            bool scaleOK = false;

            dest = LevelManager.Instance.GetBeginningBlackhole();

            while (!isFInished)
            {
                // Scale animation
                if (!scaleOK)
                    m_spriteRenderer.transform.localScale = m_spriteRenderer.transform.localScale / 0.992f;

                if (m_spriteRenderer.transform.localScale.magnitude >= Vector3.one.magnitude)
                {
                    m_spriteRenderer.transform.localScale = Vector3.one;
                    scaleOK = true;
                }

                // Rotation animation
                m_spriteRenderer.transform.Rotate(0.0f, 0.0f, 3.0f, Space.Self);

                if (scaleOK && m_spriteRenderer.transform.eulerAngles.z > 355)
                {
                    m_spriteRenderer.transform.rotation = Quaternion.identity;
                    isFInished = true;
                }

                yield return 1;
            }

            // Get just now because at the loading of the level LevelLogic is not instantiate
            EnableMovement();
        }

        dest.GetComponent<BlackHoleManager>().Shrink();
    }

    public void MinimizePlayer()
    {
        m_spriteRenderer.transform.localScale = Vector3.one * 0.1f;
        DisableMovement();
    }
}