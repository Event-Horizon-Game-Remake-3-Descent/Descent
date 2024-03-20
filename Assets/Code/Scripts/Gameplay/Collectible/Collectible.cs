using UnityEngine;

public class Collectible : MonoBehaviour
{
    //trigger ui update
    public delegate void UpdateUI();
    public static event UpdateUI OnUpdateUI = () => { };

    public delegate void CollectiblePicked(Collectible collectible);
    public static event CollectiblePicked OnCollectiblePicked = (Collectible collectible) => { };
    
    protected void TriggerEvents()
    {
        OnUpdateUI();
    }
}
