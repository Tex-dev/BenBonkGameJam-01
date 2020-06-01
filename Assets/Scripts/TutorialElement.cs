using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TutorialElement : MonoBehaviour
{
    TutorialManager m_tutoManager;

    private void Start()
    {
        m_tutoManager = GetComponentInParent<TutorialManager>();

        if (gameObject.name == "TutoElement_00")
            m_tutoManager.CallFunction(name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision is CapsuleCollider2D) || !collision.CompareTag("Player"))
            return;

        m_tutoManager.CallFunction(name);
        Destroy(gameObject);
    }
}
