using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject m_canvas;

    [SerializeField]
    private Button[] m_button;

    [SerializeField]
    private GameObject[] m_dialog;

    [SerializeField]
    private int m_dialogID = 0;

    private void Awake()
    {
        PlayerManager.Instance.DisableMovement();
    }

    public void CallFunction(string name)
    {
        MethodInfo mi = GetType().GetMethod(name);
        mi.Invoke(this, null);
    }

    public void BeginTuto()
    {
        m_player.GetComponentInChildren<Animator>().runtimeAnimatorController = m_eightAnim[0];

        CoroutineManager.Delay(TutoContinue, 1.5f);
    }

    public void TutoContinue()
    {
        m_player.GetComponentInChildren<Animator>().runtimeAnimatorController = m_eightAnim[1];
        PlayerManager.Instance.EnableMovement();
        m_canvas.GetComponent<Canvas>().enabled = true;
    }

    public void TutoNext()
    {
        PlayerManager.Instance.EnableMovement();
        m_one.GetComponent<TutoNoOne>().GoToNextPoint();
        print("c'est reparti");
    }

    public void TutoElement_00()
    {
    }

    public void TutoElement()
    {
        PlayerManager.Instance.DisableMovement();
        m_dialog[m_dialogID].SetActive(true);
        m_dialogID++;
    }
}