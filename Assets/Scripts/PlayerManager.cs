using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed;
    [SerializeField]
    private float m_jumpForce;

    [SerializeField]
    private Transform m_groundCheck;
    [SerializeField]
    private float m_groundCheckRadius;
    [SerializeField]
    private LayerMask m_collisionLayer;

    private Rigidbody2D m_rigidBody;
    private SpriteRenderer m_spriteRenderer;
    private Vector2 m_velocity = Vector2.zero;
    private bool m_isJumping = false;
    private bool m_isGrounded = true;

    private float m_horizontalMovement;

    [SerializeField]
    private int m_nbJumpMax;
    [SerializeField]
    private int m_nbJump;

    private Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_nbJump = m_nbJumpMax;
    }

    void Update()
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

    private void FixedUpdate()
    {
        MovePlayer(m_horizontalMovement);

        if (m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_collisionLayer))
            m_nbJump = m_nbJumpMax;
    }

    void MovePlayer(float p_horizontalMovement)
    {
        Vector2 targetVelocity = new Vector2(p_horizontalMovement, m_rigidBody.velocity.y);
        m_rigidBody.velocity = Vector2.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref m_velocity, 0.05f);

        if (m_isJumping)
        {
            Jump(m_jumpForce);
            m_isJumping = false;
        }
    }

    void Flip(float p_velocity)
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
