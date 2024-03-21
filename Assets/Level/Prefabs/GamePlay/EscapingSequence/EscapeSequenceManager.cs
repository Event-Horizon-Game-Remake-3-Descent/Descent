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
    [SerializeField] Transform ParticlesHolder;
    [SerializeField] Camera EscapeManagerCamera;

    private void Awake()
    {
        OnEscapeSequenceTriggered += PlayEscapeSequence;
        DollyCart.m_Speed = 0f;
        ParticlesHolder.gameObject.SetActive(false);
        EscapeManagerCamera.enabled = false;
    }

    private void OnDisable()
    {
        OnEscapeSequenceTriggered -= PlayEscapeSequence;
    }

    private void PlayEscapeSequence()
    {
        EscapeManagerCamera.enabled = true;
        DollyCart.m_Speed = DollySpeed;
        ParticlesHolder.gameObject.SetActive(true);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        while(DollyCart.m_Position < 1f)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEscapeSequenceTriggered();
    }
}
