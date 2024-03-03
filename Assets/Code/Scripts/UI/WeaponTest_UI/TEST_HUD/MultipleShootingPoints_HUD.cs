using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleShootingPoints_HUD : MonoBehaviour
{
    [SerializeField] private RawImage RightHUD;
    [SerializeField] private RawImage LeftHUD;
    [SerializeField] private Color HighlightedColor;
    [SerializeField] private Color UnhighlightedColor;

    private void Awake()
    {
        UpdateHud(0);
    }

    public void UpdateHud(float shootingPoint)
    {
        if(shootingPoint%2 == 0)
        {
            RightHUD.color = HighlightedColor;
            LeftHUD.color = UnhighlightedColor;
        }
        else
        {
            RightHUD.color = UnhighlightedColor;
            LeftHUD.color = HighlightedColor;
        }
    }
}
