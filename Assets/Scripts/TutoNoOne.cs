using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoNoOne : MonoBehaviour
{
    [SerializeField]
    Transform[] m_waypoints;

    [SerializeField]
    float m_speed;

    int m_currentWaypoint = 0;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_waypoints[m_currentWaypoint].position, m_speed * Time.deltaTime);
    }

    public void GoToNextPoint()
    {
        m_currentWaypoint++;

        if (m_currentWaypoint >= m_waypoints.Length)
            m_currentWaypoint = m_waypoints.Length - 1;
    }
}
