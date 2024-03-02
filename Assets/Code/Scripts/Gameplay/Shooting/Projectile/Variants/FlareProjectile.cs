using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareProjectile : Projectile
{
    [Space]
    [Space]
    [Header("Flare Parameters")]
    [SerializeField] float LifeTimeOnCollision = 10f;

    protected override IEnumerator OnCollisionCoroutine()
    {
        yield return new WaitForSeconds(LifeTimeOnCollision);
        Destroy(this.gameObject);
    }
}
