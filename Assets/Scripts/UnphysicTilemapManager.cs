using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class UnphysicTilemapManager : MonoBehaviour
{
    private Tilemap m_tilemap;
    float           m_alphaDest;
    
    float           m_speed = 0.1f;
    float           m_sign;
    
    [SerializeField]
    Vector2         m_speedBorder = Vector2.up;
    [SerializeField]
    Vector2         m_alphaBorder = Vector2.up;

    private void Start()
    {
        m_tilemap = GetComponent<Tilemap>();
        m_alphaDest = Random.Range(m_alphaBorder.x, m_alphaBorder.y);
        m_speed = Random.Range(m_speedBorder.x, m_speedBorder.y);

        m_sign = Mathf.Sign(m_alphaDest - m_tilemap.color.a);
    }

    // Update is called once per frame
    void Update()
    {
        float newSign;

        m_tilemap.color = new Color(m_tilemap.color.r, m_tilemap.color.g, m_tilemap.color.b, m_tilemap.color.a + m_speed * Time.deltaTime * m_sign);

        newSign = Mathf.Sign(m_alphaDest - m_tilemap.color.a);
        if(newSign != m_sign)
        {
            m_alphaDest = Random.Range(0.0f, 1.0f);
            m_speed = Random.Range(1.0f, 5.0f);

            m_sign = Mathf.Sign(m_alphaDest - m_tilemap.color.a);
        }
    }
}
