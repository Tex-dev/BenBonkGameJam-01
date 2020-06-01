using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPlayer : MonoBehaviour
{
    [SerializeField]
    private TutorialManager m_tutoManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndStandUp()
    {
        m_tutoManager.TutoContinue();
    }
}
