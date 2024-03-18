using TMPro;
using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VictoryScreenManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float StartAfter = 3f;
    [SerializeField] CinemachineDollyCart DollyCart;
    [SerializeField] float ShowAfter = 10f;
    [SerializeField] Canvas CanvasToShow;
    [SerializeField] TMP_Text Score_Text;
    [SerializeField] string SceneToLoad = "none";

    private float DollyInitialSpeed;
    private float Score = 0; 

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
        if (PlayerPrefs.HasKey("PlayerScore"))
            Score = PlayerPrefs.GetFloat("PlayerScore");
        Score_Text.text = ("Score: ") +Mathf.RoundToInt(Score).ToString();
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

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
