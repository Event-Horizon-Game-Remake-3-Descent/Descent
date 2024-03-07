using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] public float Damage = 1f;
    [SerializeField] protected float ExplosionRadius = 0f;
    [SerializeField] protected float Speed = 15f;

    [Header("VFX")]
    [SerializeField] protected ParticleSystem ParticlesOnDestroy;
    //[SerializeField] private Light LightSource;
    //[SerializeField] private AnimationCurve ExplosionLightCurve;
    //[SerializeField] private float EplosionSpeed = 1;
    //[SerializeField] private float EplosionIntensityMultiplier = 1000f;

    protected Rigidbody RigidBody;
    protected BoxCollider BoxCollider;

    protected bool Hit = false;

    protected void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        RigidBody.useGravity = false;
        RigidBody.freezeRotation = true;
    }

    protected void Start()
    {
        RigidBody.velocity = transform.forward * Speed;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        //Physics.OverlapSphere(transform.position, ExplosionRadius);
        this.RigidBody.velocity = Vector3.zero;
        Hit = true;
        BoxCollider.enabled = false;
        StartCoroutine(OnCollisionCoroutine());
    }

    protected virtual IEnumerator OnCollisionCoroutine()
    {
        //float progress = 0f;
        //while(progress < 1f)
        //{
        //    float curveValue = ExplosionLightCurve.Evaluate(progress) * EplosionIntensityMultiplier;
        //    LightSource.intensity = curveValue;
        //    LightSource.range = curveValue;
        //    progress += Time.deltaTime * EplosionSpeed;
        //    yield return null;
        //}
        Destroy(this.gameObject);
        yield return null;
    }
}