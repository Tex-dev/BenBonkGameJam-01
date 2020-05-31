using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_player = null;
    [SerializeField]
    private float m_timeOffset = 0.2f;
    [SerializeField]
    private Vector3 m_posOffset = Vector3.zero;

    private Vector3 m_velocity = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_player.position + m_posOffset, ref m_velocity, m_timeOffset);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            m_posOffset.y -= 2.5f;

        if (Input.GetKeyUp(KeyCode.DownArrow))
            m_posOffset.y += 2.5f;

    }
}
