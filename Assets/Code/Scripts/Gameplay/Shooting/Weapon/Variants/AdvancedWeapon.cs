using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedWeapon : Weapon
{
    [Header("Advanced weapon Settings")]
    [Tooltip("Projectiles used by weapon level")]
    [SerializeField] private List<Projectile> OtherProjectiles = new List<Projectile>();

    private Projectile CurrentProjectile;

    private int WeaponLevel = 0;

    new private void Awake()
    {
        base.Awake();
        CurrentProjectile = base.ProjectileToShoot;
    }

    public override bool Shoot()
    {
        //Shoot
        if (Time.time > base.NextShootTime && base.Mag.ProjectileLeft > 0)
        {
            //Update delay
            base.NextShootTime = Time.time + ShootDelay;
            //Intanciate Bullet
            if (base.ProjectileToShoot)
            {
                //Update Shooting point
                if (base.SynchronousShooting)
                {
                    //shoot on all shooting points
                    float spawnableBulletAmount = base.Mag.ProjectileLeft;
                    base.Mag.ProjectileLeft -= base.ProjectileCost * base.ShootingPoints.Count;

                    for (int i = 0; i < base.ShootingPoints.Count; ++i)
                    {
                        Instantiate(base.ProjectileToShoot, base.ShootingPoints[i].position, base.ShootingPoints[i].rotation);
                    }
                }
                else
                {
                    //shoot on one shooting point
                    base.Mag.ProjectileLeft -= (int)base.ProjectileCost;
                    Instantiate(base.ProjectileToShoot, base.ShootingPoints[ShootingPointIndex].position, base.ShootingPoints[ShootingPointIndex].rotation);
                    base.ShootingPointIndex++;
                    if (base.ShootingPointIndex > base.ShootingPoints.Count - 1)
                        base.ShootingPointIndex = 0;
                }
            }
            //Play Sound
            if (base.ShootSounds.Count > 0)
            {
                base.AudioSource.Stop();
                base.AudioSource.clip = ShootSounds[UnityEngine.Random.Range(0, ShootSounds.Count)];
                base.AudioSource.Play();
            }
            return true;
        }
        //Don't Shoot
        else
            return false;
    }

    private void IncreaseWeaponLevel()
    {
        WeaponLevel++;
        base.ProjectileToShoot = OtherProjectiles[WeaponLevel];
    }
}
