using System;
using UnityEngine;
using static Mag;
using static Weapon;

public class Collectible : MonoBehaviour
{
    //trigger ui update
    public delegate void UpdateUI();
    public static event UpdateUI OnUpdateUI = () => { };

    //generic collectible delegate
    public delegate void CollectibleTaken(float value);
    public static event CollectibleTaken OnShieldTaken = (float value) => {};
    public static Action<float> OnIncreaseScore;

    //Weapon taken event
    public delegate void WeaponTaken(WeaponType type);
    public static event WeaponTaken OnWeaponTaken = (WeaponType type) => {};

    //projectile
    public delegate void ProjectileTaken(MagType magType, float value);
    public static event ProjectileTaken OnMagTaken = (MagType magType, float value) => {};

    public enum CollectibleType
    {
        IncreaseBullet,
        IncreaseShield,
        IncreaseScore,
        UnlockWeapon,
        Key
    }

    [Header("Collectible Settings")]
    [SerializeField] CollectibleType Type;
    [SerializeField] float Value;

    [Header("Projectile to increase")]
    [SerializeField] MagType ProjectileType;

    [Header("Weapon To Unlock")]
    [SerializeField] WeaponType WeaponType;

    //KEY
    [Header("Key To Unlock")]
    [Tooltip("If you see this ask Flavio, - Flavio")]
    [SerializeField] bool NOT_IMPLEMENTED_IF;

    private void OnTriggerEnter(Collider other)
    {
        //trigger if layer is player layer
        if (other.gameObject.layer != 6)
            return;

        //select wich event call
        switch(Type)
        {
            case CollectibleType.IncreaseBullet:
            {
                OnMagTaken(ProjectileType, Value);
                if(ProjectileType == MagType.Energy)
                    UI_Manager.Notify(Value+" "+ ProjectileType+" Taken");
                else
                    UI_Manager.Notify(Value + " " + ProjectileType + " Ammo Taken");
                break;
            }
            case CollectibleType.IncreaseShield:
            {
                OnShieldTaken(Value);
                UI_Manager.Notify(Value + " Shield Obtained");
                break;
            }
            //increase score
            case CollectibleType.IncreaseScore:
            {
                OnIncreaseScore(Value);
                break;
            }
            //Unlock weapon
            case CollectibleType.UnlockWeapon:
            {
                OnWeaponTaken(WeaponType);
                if(WeaponType == WeaponType.LaserCannon)
                    UI_Manager.Notify(WeaponType + " Upgraded");
                else
                    UI_Manager.Notify(WeaponType + " Unlocked");
                break;
            }
            default:
            {
                Debug.LogError("Error while selecting event on "+gameObject.name);
                break;
            }
        }
        OnUpdateUI();

        Destroy(this.gameObject);
    }
}
