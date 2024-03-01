using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private string Name = "No Name";
    public string WeaponName { get; private set; } = null;

    [SerializeField] private Projectile ProjectileToShoot;
    [SerializeField] private float ShootDelay = 1f;
    [SerializeField] private Mag Mag;
    [SerializeField] private float ProjectileCost = 1;
    [SerializeField] List<Transform> ShootingPoints = new List<Transform>();
    [SerializeField] private bool SynchronousShooting = false;
    [Space]
    [Header("UI Settings")]
    [SerializeField] private Texture _WeaponSprite;
    public Texture WeaponSprite { get; private set; }
    [Header("SFX")]
    [SerializeField] private List<AudioClip> ShootSounds;
    [Space]
    [Header("Gizsmo Settings")]
    [SerializeField] private Color Weapon_GizsmoColor = Color.blue;
    [SerializeField] private float WeaponGismoSize = 0.1f;
    [SerializeField] private Color Direction_GizsmoColor = Color.red;
    [SerializeField] private float ShootDirectionLenght_Gizsmo = 3f;

    private float NextShootTime = 0;

    public Mag WeaponMag { get; private set; }
    public int ShootingPointIndex { get; private set; } = 0;

    private AudioSource AudioSource;

    private void Awake()
    {
        WeaponName = Name;
        WeaponMag = Mag;
        WeaponSprite = _WeaponSprite;
        AudioSource = GetComponent<AudioSource>();
    }

    public bool Shoot()
    {
        //Shoot
        if(Time.time > NextShootTime && Mag.ProjectileLeft > 0)
        {
            //Update delay
            NextShootTime = Time.time + ShootDelay;
            //Intanciate Bullet
            if(ProjectileToShoot)
            {
                //Update Shooting point
                if(SynchronousShooting)
                {
                    //shoot on all shooting points
                    float spawnableBulletAmount = Mag.ProjectileLeft;
                    Mag.ProjectileLeft -= (int)(ProjectileCost * ShootingPoints.Count);

                    for (int i = 0; i < ShootingPoints.Count; ++i)
                    {
                        Instantiate(ProjectileToShoot, ShootingPoints[i].position, ShootingPoints[ShootingPointIndex].rotation);
                    }
                }
                else
                {
                    //shoot on one shooting point
                    Mag.ProjectileLeft -= (int)ProjectileCost;
                    Instantiate(ProjectileToShoot, ShootingPoints[ShootingPointIndex].position, ShootingPoints[ShootingPointIndex].rotation);
                    ShootingPointIndex++;
                    if (ShootingPointIndex > ShootingPoints.Count - 1)
                        ShootingPointIndex = 0;
                }
            }
            //Play Sound
            if(ShootSounds.Count>0)
            {
                AudioSource.Stop();
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
            //Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            //Gizmos.matrix = rotationMatrix;
            Gizmos.color = Weapon_GizsmoColor;
            Gizmos.DrawWireSphere(ShootingPoints[i].position, WeaponGismoSize);
        }
    }

#endif
}
