using UnityEngine;
using static Mag;

public class AmmoCollectible : Collectible
{
    public delegate void ProjectileTaken(MagType magType, float value);
    public static event ProjectileTaken OnMagTaken = (MagType magType, float value) => { };

    [Header("Ammo Collectible Settings")]
    [SerializeField] MagType ProjectileType;
    [SerializeField] float IncreaseAmmoValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnMagTaken(ProjectileType, IncreaseAmmoValue);
        if (ProjectileType == MagType.Energy)
            UI_Manager.Notify(IncreaseAmmoValue + " " + ProjectileType + " Taken");
        else
            UI_Manager.Notify(IncreaseAmmoValue + " " + ProjectileType + " Ammo Taken");
        
        base.TriggerEvents();
        Destroy(this.gameObject);
    }
}
