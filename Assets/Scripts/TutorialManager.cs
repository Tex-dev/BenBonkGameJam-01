using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private RuntimeAnimatorController[] m_eightAnim;

    [SerializeField]
    private GameObject m_player;

    [SerializeField]
    private GameObject m_one;

    [SerializeField]
    private Transform[] m_onePos;

    public void CallFunction(string name)
    {
        MethodInfo mi = GetType().GetMethod(name);
        mi.Invoke(this, null);
    }

    public void TutoElement_00()
    {
        print("BG");
    }

    public void TutoElement_01()
    {
        print("youhou");
    }
}
