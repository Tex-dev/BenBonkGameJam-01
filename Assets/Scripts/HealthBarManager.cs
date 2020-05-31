using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBarManager : MonoBehaviour
{
    private Slider      m_slider = null;

    [SerializeField]
    private Gradient    m_gradient = null;
    [SerializeField]
    private Image       m_fill = null;

    private void Start()
    {
        m_slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(int p_health)
    {
        if (m_slider == null)
            m_slider = GetComponent<Slider>();

        m_slider.maxValue = p_health;
        m_slider.value = p_health;

        m_fill.color = m_gradient.Evaluate(1.0f);
    }

    public void SetHealth(int p_health)
    {
        m_slider.value = p_health;

        m_fill.color = m_gradient.Evaluate(m_slider.normalizedValue);
    }

}
