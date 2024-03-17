using UnityEngine;

public class Mag : MonoBehaviour
{
    public enum MagType
    {
        Energy,
        Vulkan,
        ConcussionMissile,
        HomingMissile,
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
        
        ProjectileLeft += amount;
        if(IsCapped && ProjectileLeft>MaxValue)
            ProjectileLeft = MaxValue;
    }

    private void RecoverEnergy(float amount)
    {
        if (MagMagType != MagType.Energy)
            return;

        if(ProjectileLeft < 100)
        {
            ProjectileLeft += amount;
            ProjectileLeft = Mathf.Clamp(ProjectileLeft, 0f, 100f);
        }
    }

    private void OnEnable()
    {
        AmmoCollectible.OnMagTaken += OnAmmoTaken;
        EnergyRecoverZone.OnEnergyRecover += RecoverEnergy;
    }

    private void OnDisable()
    {
        AmmoCollectible.OnMagTaken -= OnAmmoTaken;
    }

    public void ResetMag()
    {
        ProjectileLeft = InitialMagSize;
    }
}
