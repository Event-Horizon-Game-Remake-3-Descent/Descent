using UnityEngine;
using static Weapon;

public class WeaponCollectible : Collectible
{
    public delegate void WeaponTaken(WeaponType type);
    public static event WeaponTaken OnWeaponTaken = (WeaponType type) => { };

    [Header("Weapon To Unlock")]
    [SerializeField] WeaponType WeaponType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnWeaponTaken(WeaponType);
        if (WeaponType == WeaponType.LaserCannon)
            UI_Manager.Notify(WeaponType + " Upgraded");
        else
            UI_Manager.Notify(WeaponType + " Unlocked");

        Destroy(this.gameObject);
    }
}
