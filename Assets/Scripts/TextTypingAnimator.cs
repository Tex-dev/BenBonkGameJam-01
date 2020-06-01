using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Animate a text UI.
/// </summary>
public class TextTypingAnimator : MonoBehaviour
{
    [Serializable]
    public enum TypingMode
    {
        CHARACTER,
        LINE
    }

    [Header("Animation parameters")]

    /// <summary>
    /// Typing mode.
    /// </summary>
    [SerializeField]
    private TypingMode m_TypingMode = TypingMode.CHARACTER;

    /// <summary>
    /// Speed at which a character is typed.
    /// </summary>
    [SerializeField]
    private float m_WritingSpeed = 0.25f;

    /// <summary>
    /// Should the text be disable when animation is done?
    /// </summary>
    [SerializeField]
    private bool m_DisableWhenDone = false;

    /// <summary>
    /// Should the text delete itself when done?
    /// </summary>
    [SerializeField]
    private bool m_DeleteBehaviour = false;

    /// <summary>
    /// Amount of character to delete.
    /// </summary>
    [SerializeField]
    private int m_DeleteQuantity = 0;

    /// <summary>
    /// Delay before starting character deletion.
    /// </summary>
    [SerializeField]
    private float m_DelayBeforeDeletion = 0f;

    /// <summary>
    /// Time before the text disappears for the next text.
    /// </summary>
    [SerializeField]
    private float m_TimeBeforeDisable = 0.0f;

    [Header("Timing")]
    /// <summary>
    /// Offset the start of the animation.
    /// </summary>
    [SerializeField]
    private float m_StartOffset = 0.0f;

    /// <summary>
    /// Offset the end of animation trigger.
    /// </summary>
    [SerializeField]
    private float m_EndOffset = 0.0f;

    /// <summary>
    /// Called when the animation starts.
    /// </summary>
    [SerializeField]
    private UnityEvent OnAnimationStart = null;

    /// <summary>
    /// Called when the animation is over.
    /// </summary>
    [SerializeField]
    private UnityEvent OnAnimationEnd = null;

    [Header("Audio features")]
    /// <summary>
    /// Should use typing SFX?
    /// </summary>
    [SerializeField]
    private bool m_TypingSFXEnabled = false;

    /// <summary>
    /// Audio clip for typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_TypingSFX = null;

    /// <summary>
    /// Audio clip for spacebar typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_TypingSpacebarSFX = null;

    /// <summary>
    /// Audio clip for carriage return typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_CarriageReturnSFX = null;

    /// <summary>
    /// Audio source to use for the typing SFXs.
    /// </summary>
    [SerializeField]
    private AudioSource m_TypingAudioSource = null;

    /// <summary>
    /// Should use a narrator when typing text?
    /// </summary>
    [SerializeField]
    private bool m_AudioNarratorEnabled = false;

    /// <summary>
    /// Narrator voice clip.
    /// </summary>
    [SerializeField]
    private AudioClip m_NarratorClip = null;

    /// <summary>
    /// Audio source to use for the narrator voice.
    /// </summary>
    [SerializeField]
    private AudioSource m_NarratorAudioSource = null;

    /// <summary>
    /// Text UI element.
    /// </summary>
    private Text m_Text = null;

    /// <summary>
    /// Text content to display.
    /// </summary>
    private string m_Content = "";

    /// <summary>
    /// Text content to display, line by line.
    /// </summary>
    private string[] m_ContentLines;

    /// <summary>
    /// Awake is called by Unity for initialization.
    /// </summary>
    private void Awake()
    {
        m_Text = GetComponent<Text>();

        m_TypingAudioSource = GetComponent<AudioSource>();

        m_Content = m_Text.text;

        m_ContentLines = System.Text.RegularExpressions.Regex.Split(m_Text.text, "\r\n|\r|\n");

        m_Text.text = "";
    }

    /// <summary>
    /// Start is called by Unity after initialization.
    /// </summary>
    private void Start()
    {
        StartCoroutine(Animate());
    }

    /// <summary>
    /// Animate the text.
    /// </summary>
    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(m_StartOffset);

        OnAnimationStart?.Invoke();

        int index = -1;

        int lineIndex = -1;

        float narratorSpeed;

        if (m_AudioNarratorEnabled)
        {
            narratorSpeed = (m_NarratorClip.length - m_EndOffset) / m_Content.Length;

            m_NarratorAudioSource.PlayOneShot(m_NarratorClip);
        }
        else
        {
            narratorSpeed = m_WritingSpeed;
        }

        if (m_TypingMode == TypingMode.CHARACTER)
        {
            while (m_Text.text.Length < m_Content.Length)
            {
                m_Text.text += m_Content[++index];

                if (m_TypingSFX)
                {
                    if (m_Content[index] == ' ')
                    {
                        m_TypingAudioSource.PlayOneShot(m_TypingSpacebarSFX);
                    }
                    else
                    {
                        m_TypingAudioSource.PlayOneShot(m_TypingSFX);
                    }
                }

                if (m_Content[index] != '\n' && m_Content[index] != '\r')
                    yield return new WaitForSeconds(narratorSpeed);
            }
        }
        else
        {
            while (lineIndex < m_ContentLines.Length - 1)
            {
                lineIndex++;

                m_Text.text += m_ContentLines[lineIndex] + "\n";

                if (m_ContentLines[lineIndex] != "")
                    yield return new WaitForSeconds(narratorSpeed);
            }
        }

        if (m_TypingSFXEnabled)
            m_TypingAudioSource.PlayOneShot(m_CarriageReturnSFX);

        if (m_DeleteBehaviour && m_TypingMode == TypingMode.CHARACTER)
        {
            yield return new WaitForSeconds(m_DelayBeforeDeletion);

            index = m_Text.text.Length - 1;

            int length = m_Text.text.Length;

            while (m_Text.text.Length > length - m_DeleteQuantity)
            {
                m_Content = m_Text.text;
                m_Text.text = m_Text.text.Remove(m_Text.text.Length - 1);

                if (m_TypingSFX)
                {
                    m_TypingAudioSource.PlayOneShot(m_TypingSFX);
                }

                if (m_Content[index] != '\n' && m_Content[index] != '\r')
                    yield return new WaitForSeconds(narratorSpeed);

                index--;
                if (index < 0)
                    break;
            }
        }

        if (m_DisableWhenDone)
        {
            if (m_TimeBeforeDisable != 0f)
            {
                while (m_Text.color.a > 0.0f)
                {
                    Color color = m_Text.color;
                    m_Text.color = new Color(color.r, color.g, color.b, color.a - 0.01f);

                    yield return new WaitForSeconds(m_TimeBeforeDisable / 100.0f);
                }
            }

            OnAnimationEnd?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            OnAnimationEnd?.Invoke();
        }
    }
}