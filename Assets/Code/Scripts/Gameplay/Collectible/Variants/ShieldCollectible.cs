using UnityEngine;

public class ShieldCollectible : Collectible
{
    //generic collectible delegate
    public delegate void ShieldTaken(float value);
    public static event ShieldTaken OnShieldTaken = (float value) => { };

    [Header("Shield Collectible Setting")]
    [SerializeField] float IncreaseShieldValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnShieldTaken(IncreaseShieldValue);

        UI_Manager.Notify(IncreaseShieldValue + " Shield Obtained");

        base.TriggerEvents();
        Destroy(this.gameObject);
    }
}
