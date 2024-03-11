using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class EnemyWeapon: MonoBehaviour
{
    [Header("Weapon Settings")]
    [Tooltip("Prefab of the projectile to shoot")]
    [SerializeField] protected Projectile ProjectileToShoot;
    [Tooltip("Delay between shots")]
    [SerializeField] protected float ShootDelay = 1f;
    [SerializeField] protected List<Transform> ShootingPoints = new List<Transform>();
    [Tooltip("If true it will shoot one bullet for each shooting point in the same frame")]
    [SerializeField] protected bool SynchronousShooting = false;
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
        ShootingPointsAmount = ShootingPoints.Count;
        AudioSource = GetComponent<AudioSource>();
    }

    public bool Shoot()
    {
        //Shoot
        if (Time.time > NextShootTime)
        {
            //Update delay
            NextShootTime = Time.time + ShootDelay;
            //Intanciate Bullet
            if (ProjectileToShoot)
            {
                //Update Shooting point
                if (SynchronousShooting)
                {
                    for (int i = 0; i < ShootingPoints.Count; ++i)
                    {
                        Instantiate(ProjectileToShoot, ShootingPoints[i].position, ShootingPoints[i].rotation);
                    }
                }
                else
                {
                    //shoot on one shooting point
                    Instantiate(ProjectileToShoot, ShootingPoints[ShootingPointIndex].position, ShootingPoints[ShootingPointIndex].rotation);
                    ShootingPointIndex++;
                    if (ShootingPointIndex > ShootingPoints.Count - 1)
                        ShootingPointIndex = 0;
                }
            }
            //Play Sound
            if (ShootSounds.Count > 0)
            {
                AudioSource.clip = ShootSounds[UnityEngine.Random.Range(0, ShootSounds.Count)];
                AudioSource.Play();
            }
            return true;
        }
        //Don't Shoot
        else
            return false;
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
