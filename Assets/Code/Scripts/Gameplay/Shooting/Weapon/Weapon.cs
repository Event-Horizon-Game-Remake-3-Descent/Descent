using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        DEFAULT,
        LaserCannon,
        VulcanCannon,
        SpreadfireCannon,
        ConcussionMissiles,
        HomingMissiles,
        ProximityBombs,
        FlareCannon,
    }

    [Header("Weapon Settings")]
    [Tooltip("Name showed in UI")]
    [SerializeField] protected string Name = "No Name";
    public string WeaponName { get; protected set; } = null;
    [SerializeField] protected WeaponType Type = WeaponType.DEFAULT;
    public WeaponType WType {  get; private set; }
    [Tooltip("Prefab of the projectile to shoot")]
    [SerializeField] protected Projectile ProjectileToShoot;
    [Tooltip("Delay between shots")]
    [SerializeField] protected float ShootDelay = 1f;
    [Tooltip("Mag to get the bullet count from")]
    [SerializeField] protected Mag Mag;
    [Tooltip("Mag bullet cost per projectile")]
    [SerializeField] protected float ProjectileCost = 1;
    [SerializeField] protected List<Transform> ShootingPoints = new List<Transform>();
    [Tooltip("If true it will shoot one bullet for each shooting point in the same frame")]
    [SerializeField] protected bool SynchronousShooting = false;
    [Tooltip("Rappresent the default state of the weapon")]
    [SerializeField] public bool IsUnlocked = false;
    [SerializeField] protected int ScoreIfAlreadyUnlocked = 0;
    [Space]

    [Header("UI Settings")]
    [SerializeField] protected Texture _WeaponSprite;
    public Texture WeaponSprite { get; private set; }
    [Space]

    [Header("SFX")]
    [Tooltip("List of sounds played when shooting")]
    [SerializeField] protected List<AudioClip> ShootSounds;
    [Space]

    [Header("Gizsmo Settings")]
    [SerializeField] protected Color Weapon_GizsmoColor = Color.blue;
    [SerializeField] protected float WeaponGismoSize = 0.1f;
    [SerializeField] protected Color Direction_GizsmoColor = Color.red;
    [SerializeField] protected float ShootDirectionLenght_Gizsmo = 3f;

    public Mag WeaponMag { get; private set; }
    public int ShootingPointIndex { get; protected set; } = 0;
    public int ShootingPointsAmount { get; protected set; }

    protected float NextShootTime = 0;
    protected AudioSource AudioSource;
    
    protected void Awake()
    {
        WeaponName = Name;
        WType = Type;

        if (Mag == null)
            Debug.LogError("No MAG assigned to this weapon: "+gameObject.name);

        WeaponMag = Mag;
        WeaponSprite = _WeaponSprite;
        ShootingPointsAmount = ShootingPoints.Count;
        AudioSource = GetComponent<AudioSource>();
    }

    public abstract bool Shoot();

    private void UnlockWeapon(WeaponType type)
    {
        if(type != WType)
            return;

        OnUnlock();
    }

    protected virtual void OnUnlock()
    {
        if (IsUnlocked)
            ScoreCollectible.OnIncreaseScore?.Invoke(ScoreIfAlreadyUnlocked);
        else
            IsUnlocked = true;
    }

    protected void OnEnable()
    {
        WeaponCollectible.OnWeaponTaken += UnlockWeapon;
    }

    public void ResetWeapon()
    {
        Mag.ResetMag();
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        for(int i = 0; i < ShootingPoints.Count; i++)
        {
            //Draw gun shooting direction
            Gizmos.color = Direction_GizsmoColor;
            Gizmos.DrawRay(ShootingPoints[i].position, ShootingPoints[i].forward * ShootDirectionLenght_Gizsmo);

            //draw gun position
            Gizmos.color = Weapon_GizsmoColor;
            Gizmos.DrawWireSphere(ShootingPoints[i].position, WeaponGismoSize);
        }
    }

#endif
}
