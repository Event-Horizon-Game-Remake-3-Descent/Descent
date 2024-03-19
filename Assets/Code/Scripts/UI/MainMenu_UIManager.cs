using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_UIManager : MonoBehaviour
{
    [SerializeField] private List<LoadSceneData> PossiblesScenes;
    [SerializeField] private RectTransform SettingsPanel;

    public void LoadScene(int index)
    {
        PossiblesScenes[index].LoadScene();
    }

    public void OpenSettingsScreen()
    {
        SettingsPanel.gameObject.SetActive(true);
    }

    public void CloseSettingsScreen()
    {
        SettingsPanel.gameObject.SetActive(false);
    }
}
