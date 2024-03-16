using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeSequenceManager : MonoBehaviour
{
    public delegate void EscapeSequenceTriggered();
    public static event EscapeSequenceTriggered OnEscapeSequenceTriggered = () => { };
    
    [Header("Escape Sequence Settings")]
    [SerializeField] CinemachineDollyCart DollyCart;
    [SerializeField] float DollySpeed = 0.25f;
    [SerializeField] string SceneToLoad = "none";

    private void Awake()
    {
        OnEscapeSequenceTriggered += PlayEscapeSequence;
        DollyCart.m_Speed = 0f;
    }

    private void PlayEscapeSequence()
    {
        DollyCart.m_Speed = DollySpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEscapeSequenceTriggered();
    }
}
