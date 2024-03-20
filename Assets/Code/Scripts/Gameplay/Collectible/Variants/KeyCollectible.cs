using UnityEngine;
using static KeyCollectible;

public class KeyCollectible : Collectible
{
    public delegate void KeyCollected(int KeyNumber);
    public static KeyCollected OnKeyCollected = (int keyNumber) => { };

    public delegate void KeyObtained();
    public KeyObtained OnKeyObtained = () => { };

    [Header("Key Collecible Settings")]
    [Tooltip("Name Showed In UI")]
    [SerializeField] private string KeyName = "Cazzo Palle Cazzo Palle";
    [Tooltip("Key code of the key, used in UI to show the unlock light")]
    [SerializeField][Range(0, 2)] int KeyNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnKeyCollected(KeyNumber);
        OnKeyObtained();
        UI_Manager.Notify(KeyName+" Unlocked");

        base.TriggerEvents();
        Destroy(this.gameObject);
    }
}
