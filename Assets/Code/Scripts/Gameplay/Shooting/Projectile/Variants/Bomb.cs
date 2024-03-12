using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : Projectile, IDamageable
{
    [Header("Bomb Settings")]
    [SerializeField] private float LifeTime = 30.0f;
    [SerializeField] private float ActiveAfterSec = 2.0f;

    [Header("Bomb Blinking Light")]
    [SerializeField] private Light LightSource;
    [SerializeField] private float Frequency;

    private bool LightState = false;
    private bool CanExplode = false;

    public float HP { get; set; } = 0.1f;


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
        InvokeRepeating("BlinkLight", ActiveAfterSec, 1 / Frequency);
    }

    private void BlinkLight()
    {
        LightState = !LightState;
        LightSource.enabled = LightState;
    }

    private IEnumerator CoroutineOnActive()
    {
        yield return new WaitForSeconds(ActiveAfterSec);
        CanExplode = true;
        base.CapsuleCollider.enabled = true;
    }

    private IEnumerator ExplodeAfterTimeCoroutine()
    {
        yield return new WaitForSeconds(LifeTime);
        if(CanExplode)
            Explode();
    }

    private void Explode()
    {
        CanExplode = false;

        base.AudioSource.Play();
        base.ParticlesOnDestroy.Play();
        LightSource.intensity = 0f;

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

    public void TakeDamage(float Damage)
    {
        HP -= Damage;
        if (CanExplode && HP<=0f)
        {
            Explode();
        }
    }
}
