using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackHoleManager : MonoBehaviour
{
    private Animator m_animator;
    private bool m_beginLevel;

    // Start is called before the first frame update
    private void Start()
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
        if (!m_beginLevel)
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextLevel >= SceneManager.sceneCountInBuildSettings)
            {
                Application.OpenURL("https://cayugatribune.wixsite.com/welcometotheinternet");
            }
            else
                SceneManager.LoadScene(nextLevel);
        }
        Destroy(gameObject);
    }

    public void EndGrowAnimation()
    {
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