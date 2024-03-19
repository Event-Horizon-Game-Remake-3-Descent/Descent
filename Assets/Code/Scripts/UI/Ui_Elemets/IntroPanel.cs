using System.Collections;
using TMPro;
using UnityEngine;

public class IntroPanel : MonoBehaviour
{
    [SerializeField] private RectTransform PanelRectTransform;
    [SerializeField] private TMP_Text Text;
    [SerializeField] private float TextSpeed = 1f;

    private Coroutine AnimationCoroutine = null;
    private WaitForSeconds WaitTime;
    private int TextIndex = 0;
    private bool AnimationEnded = false;

    private IEnumerator AnimateText()
    {
        AnimationEnded = false;
        while (TextIndex < Text.text.Length)
        {
            TextIndex++;
            Text.maxVisibleCharacters = TextIndex;
            yield return WaitTime;
        }
        AnimationEnded = true;
    }

    public void SkipText()
    {
        StopCoroutine(AnimationCoroutine);
        Text.maxVisibleCharacters = Text.text.Length;
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