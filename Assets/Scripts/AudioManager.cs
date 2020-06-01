using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the background music audio sources.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    /// <summary>
    /// Plays the intro theme.
    /// </summary>
    [SerializeField]
    private AudioSource m_Intro = null;

    /// <summary>
    /// Plays the intro theme, paused version.
    /// </summary>
    [SerializeField]
    private AudioSource m_IntroPaused = null;

    /// <summary>
    /// Plaus the main loop.
    /// </summary>
    [SerializeField]
    private AudioSource m_ThemeLoop = null;

    /// <summary>
    /// Plays the main loop, paused version.
    /// </summary>
    [SerializeField]
    private AudioSource m_ThemeLoopPaused = null;

    /// <summary>
    /// Duration of the fade transtion.
    /// </summary>
    [SerializeField]
    private float m_FadeDuration = 1f;

    /// <summary>
    /// Range of the volume. X is min, Y is max.
    /// </summary>
    [SerializeField]
    private Vector2 m_VolumeRange = new Vector2(0.03f, 1f);

    /// <summary>
    /// Awake is called by Unity at initialization.
    /// </summary>
    private void Awake()
    {
        //DontDestroyOnLoad(this);

        // Play the main loop once the intros are done.
        m_ThemeLoop.PlayDelayed(m_Intro.clip.length);
        m_ThemeLoopPaused.PlayDelayed(m_Intro.clip.length);

        m_Intro.volume = m_VolumeRange.y;
        m_ThemeLoop.volume = m_VolumeRange.y;

        m_IntroPaused.volume = m_VolumeRange.x;
        m_ThemeLoopPaused.volume = m_VolumeRange.x;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Called when the scene is  loaded.
    /// </summary>
    /// <param name="scene">Loaded scene.</param>
    /// <param name="mode">Scene loading mode.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerManager.Instance.OnPause += PauseCallback;
        PlayerManager.Instance.OnPlay += PlayCallback;
    }

    /// <summary>
    /// Called when play mode is activated from the player manager.
    /// </summary>
    private void PlayCallback()
    {
        StartCoroutine(c_VolumePlay());
    }

    /// <summary>
    /// Animates the volume of the audio sources on play.
    /// </summary>
    private IEnumerator c_VolumePlay()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        while (stopwatch.ElapsedMilliseconds < m_FadeDuration * 1000f)
        {
            float ratio = (float)stopwatch.ElapsedMilliseconds / (m_FadeDuration * 1000f);

            m_Intro.volume = Mathf.Lerp(m_VolumeRange.x, m_VolumeRange.y, ratio);
            m_ThemeLoop.volume = Mathf.Lerp(m_VolumeRange.x, m_VolumeRange.y, ratio);

            m_IntroPaused.volume = Mathf.Lerp(m_VolumeRange.y, m_VolumeRange.x, ratio);
            m_ThemeLoopPaused.volume = Mathf.Lerp(m_VolumeRange.y, m_VolumeRange.x, ratio);
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Called when pause mode is activated from the player manager.
    /// </summary>
    private void PauseCallback()
    {
        StartCoroutine(c_VolumePause());
    }

    /// <summary>
    /// Animates the volume of the audio sources on pause.
    /// </summary>
    private IEnumerator c_VolumePause()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        while (stopwatch.ElapsedMilliseconds < m_FadeDuration * 1000f)
        {
            float ratio = (float)stopwatch.ElapsedMilliseconds / (m_FadeDuration * 1000f);

            m_IntroPaused.volume = Mathf.Lerp(m_VolumeRange.x, m_VolumeRange.y, ratio);
            m_ThemeLoopPaused.volume = Mathf.Lerp(m_VolumeRange.x, m_VolumeRange.y, ratio);

            m_Intro.volume = Mathf.Lerp(m_VolumeRange.y, m_VolumeRange.x, ratio);
            m_ThemeLoop.volume = Mathf.Lerp(m_VolumeRange.y, m_VolumeRange.x, ratio);
            yield return new WaitForEndOfFrame();
        }
    }
}