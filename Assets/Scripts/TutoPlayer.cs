using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPlayer : MonoBehaviour
{
    [SerializeField]
    private TutorialManager m_tutoManager = null;

    public void EndStandUp()
    {
        m_tutoManager.TutoContinue();
    }
}