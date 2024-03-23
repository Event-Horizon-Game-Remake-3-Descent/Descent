using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroPanel : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> TextList = null;
    [SerializeField] private float TextSpeed = 1f;
    [SerializeField] private int StartChar = 0;
    [SerializeField] private int StopChar = -1;

    private Coroutine AnimationCoroutine = null;
    private WaitForSeconds WaitTime;
    private int TextIndex = 0;
    private bool AnimationEnded = false;

    private void Awake()
    {
        if (TextList.Count > 0)
        {
            for (int i = 0; i < TextList.Count; i++)
                TextList[i].maxVisibleCharacters = 0;
            if (StopChar < 0)
                StopChar = TextList[0].text.Length;
        }
    }

    private IEnumerator AnimateText()
    {
        if (TextList == null)
        {
            AnimationEnded = true;
            yield break;
        }

        AnimationEnded = false;
        for(int i = 0; i < TextList.Count; i++)
        {
            TextIndex = StartChar;
            while (TextIndex < StopChar)
            {
                TextIndex++;
                TextList[i].maxVisibleCharacters = TextIndex;
                yield return WaitTime;
            }
        }
        AnimationEnded = true;
    }

    public void SkipText()
    {
        StopCoroutine(AnimationCoroutine);

        if (TextList != null)
            for (int i = 0; i < TextList.Count; i++)
                TextList[i].maxVisibleCharacters = TextList[i].text.Length;

        AnimationEnded = true;
    }

    public bool TextFinished()
    { 
        return AnimationEnded;
    }

    private void OnEnable()
    {
        WaitTime = new WaitForSeconds(1/TextSpeed);
        AnimationCoroutine = StartCoroutine(AnimateText());
    }
}