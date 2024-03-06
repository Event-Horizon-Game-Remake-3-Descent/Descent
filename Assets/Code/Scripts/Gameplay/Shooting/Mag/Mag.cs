using UnityEngine;

public class Mag : MonoBehaviour
{
    public enum MagType
    {
        Energy,
        Vulkan,
        Spread,
        Concussion_Rocket,
        HeatSeeking_Rocket,
        PlasmaCannon,
        Bomb,
    };

    [Header("Mag Settings")]
    [SerializeField] public float InitialMagSize = 0f;
    [SerializeField] public MagType MagMagType;
    [SerializeField] public bool IsCapped = false;
    [SerializeField] private float MaxValue = 200f;

    [HideInInspector] public float ProjectileLeft;

    private void Awake()
    {
        ProjectileLeft = InitialMagSize;
    }

    private void OnAmmoTaken(MagType type, float amount)
    {
        if (type != MagMagType)
            return;
        Debug.Log("Ammo Taken: " + type + "\nThis Mag Type: " + MagMagType);
        
        ProjectileLeft += amount;
        if(IsCapped && ProjectileLeft>MaxValue)
            ProjectileLeft = MaxValue;
    }

    private void OnEnable()
    {
        Collectible.OnMagTaken += OnAmmoTaken;
    }

    private void OnDisable()
    {
        Collectible.OnMagTaken -= OnAmmoTaken;
    }

}
