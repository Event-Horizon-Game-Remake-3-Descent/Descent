using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float StartAfter = 3f;
    [SerializeField] CinemachineDollyCart DollyCart;
    [SerializeField] float ShowAfter = 10f;
    [SerializeField] Canvas CanvasToShow;

    float DollyInitialSpeed;

    private void Awake()
    {
        CanvasToShow.gameObject.SetActive(false);
        DollyInitialSpeed = DollyCart.m_Speed;
        DollyCart.m_Speed = 0f;
    }

    private void Start()
    {
        StartCoroutine(MoveDollyCoroutine());
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator MoveDollyCoroutine()
    {
        yield return new WaitForSeconds(StartAfter);
        DollyCart.m_Speed = DollyInitialSpeed;
    }

    private IEnumerator ShowCoroutine()
    {
        yield return new WaitForSeconds(ShowAfter);
        CanvasToShow.gameObject.SetActive(true);
    }
}
