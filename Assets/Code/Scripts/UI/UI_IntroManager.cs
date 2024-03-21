using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_IntroManager : MonoBehaviour
{
    [SerializeField] private List<IntroPanel> IntroPanels = new List<IntroPanel>();
    [SerializeField] private LoadSceneData SceneToLoadAtTheEnd;
    [SerializeField] private float DelayBetweensInput = 1f;

    private int CurrentIdex = 0;
    private float LastTimeCall = 0f;

    private void Awake()
    {
        for(int i = 1; i < IntroPanels.Count; i++)
        {
            IntroPanels[i].gameObject.SetActive(false);
        }
    }

    

    private void NextPanel()
    {
        if (Time.time < LastTimeCall + DelayBetweensInput)
            return;

        LastTimeCall = Time.time;

        if (IntroPanels[CurrentIdex].TextFinished())
        {
            IntroPanels[CurrentIdex].gameObject.SetActive(false);
            CurrentIdex++;

            if (CurrentIdex > IntroPanels.Count - 1)
            {
                SceneToLoadAtTheEnd.LoadScene();
                return;
            }

            IntroPanels[CurrentIdex].gameObject.SetActive(true);
        }
        else
            IntroPanels[CurrentIdex].SkipText();
    }

    private void OnEnable()
    {
        IntroInputManager.OnSkip += NextPanel;
    }

    private void OnDisable()
    {
        IntroInputManager.OnSkip -= NextPanel;
    }
}
