using UnityEngine;

public class BlackHoleManager : MonoBehaviour
{
    private Animator        m_animator;
    private bool            m_beginLevel;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();

        if (m_beginLevel)
            PlayerManager.Instance.MinimizePlayer();
    }

    public void Shrink()
    {
        m_animator.SetBool("shrink", true);
    }

    public void EndShrinkAnimation()
    {
        print("End blackhole shrink");
        if (!m_beginLevel)
        {
            // Load new level
        }
        Destroy(gameObject);
    }

    public void EndGrowAnimation()
    {
        print("End blackhole grow");
        if (m_beginLevel)
        {
            PlayerManager.Instance.PlayerAnimation(false);
        }
    }

    public bool IsBeginLevel()
    {
        return m_beginLevel;
    }

    public void BeginLevel(bool beginLevel = true)
    {
        m_beginLevel = beginLevel;
    }
}
