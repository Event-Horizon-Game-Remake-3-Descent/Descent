using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : Projectile
{
    [Header("Bomb Settings")]
    [SerializeField] private float LifeTime = 30.0f;
    [SerializeField] private float ActiveAfterSec = 2.0f;

    private bool CanExplode = false;

    //prevent moving
    new private void Awake()
    {
        base.CapsuleCollider = GetComponent<CapsuleCollider>();
        base.AudioSource = GetComponent<AudioSource>();
        base.CapsuleCollider.enabled = false;
    }

    new private void Start()
    {
        StartCoroutine(CoroutineOnActive());
        StartCoroutine(ExplodeAfterTineCoroutine());
    }

    private IEnumerator CoroutineOnActive()
    {
        yield return new WaitForSeconds(ActiveAfterSec);
        CanExplode = true;
        base.CapsuleCollider.enabled = true;
    }

    private IEnumerator ExplodeAfterTineCoroutine()
    {
        yield return new WaitForSeconds(LifeTime);
        Explode();
    }

    private void Explode()
    {
        base.AudioSource.Play();
        base.ParticlesOnDestroy.Play();

        //Stop collision detection
        CapsuleCollider.enabled = false;
        //Hide Bullet
        if (base.MeshToHide)
            base.MeshToHide.gameObject.SetActive(false);
        //Explode if needed
        if (ExplosionRadius > 0)
        {
            //Get Colliders
            Collider[] ArrayOfCollider;
            ArrayOfCollider = Physics.OverlapSphere(transform.position, ExplosionRadius, ExplosionLayerMask);
            //Apply Damage
            for (int i = 0; i < ArrayOfCollider.Length; i++)
                if (ArrayOfCollider[i].gameObject.TryGetComponent<IDamageable>(out IDamageable Damageable))
                    Damageable.TakeDamage(Damage);
        }
    }

    new protected void OnCollisionEnter(Collision collision){}

    private void OnTriggerEnter(Collider other)
    {
        if(CanExplode)
        {
            Explode();
        }
    }

}
