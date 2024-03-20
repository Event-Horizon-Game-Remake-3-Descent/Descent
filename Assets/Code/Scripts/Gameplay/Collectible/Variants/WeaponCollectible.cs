using UnityEngine;
using static Weapon;

public class WeaponCollectible : Collectible
{
    public delegate void WeaponTaken(WeaponType type);
    public static event WeaponTaken OnWeaponTaken = (WeaponType type) => { };

    public delegate void WeaponCollectiblePickedUp(WeaponCollectible collectible);
    public static event WeaponCollectiblePickedUp OnWeaponCollectiblePickedUp = (WeaponCollectible collectible) => { };

    [Header("Weapon To Unlock")]
    [SerializeField] WeaponType WeaponType;

    private Rigidbody Rigidbody;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void ApplyForce(Vector3 dir, float force, ForceMode forceMode)
    {
        Rigidbody.AddForce(dir*force, forceMode);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnWeaponTaken(WeaponType);
        if (WeaponType == WeaponType.LaserCannon)
            UI_Manager.Notify(WeaponType + " Upgraded");
        else
            UI_Manager.Notify(WeaponType + " Unlocked");

        OnWeaponCollectiblePickedUp(this);

        base.TriggerEvents();
    }
}
