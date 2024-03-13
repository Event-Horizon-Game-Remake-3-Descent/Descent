using UnityEngine;

public class Collectible : MonoBehaviour
{
    //trigger ui update
    public delegate void UpdateUI();
    public static event UpdateUI OnUpdateUI = () => { };
}
