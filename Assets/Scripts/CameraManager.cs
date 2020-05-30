using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_player;
    [SerializeField]
    private float m_timeOffset;
    [SerializeField]
    private Vector3 m_posOffset;

    private Vector3 m_velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_player.position + m_posOffset, ref m_velocity, m_timeOffset);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            m_posOffset.y = 0.0f;

        if (Input.GetKeyUp(KeyCode.DownArrow))
            m_posOffset.y = 2.5f;

    }
}
