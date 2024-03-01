using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(Light))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float Damage;
    [SerializeField] private float ExplosionRadius;
    [SerializeField] private float Speed = 15f;

    [Header("VFX")]
    [SerializeField] private Transform ParticlesOnDestroy;
    //[SerializeField] private Light LightSource;
    [SerializeField] private AnimationCurve ExplosionLightCurve;
    [SerializeField] private float EplosionSpeed = 1;
    [SerializeField] private float EplosionIntensityMultiplier = 1000f;

    Rigidbody RigidBody;
    BoxCollider BoxCollider;

    private bool Hitted = false;

    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        //LightSource = GetComponent<Light>();
        BoxCollider = GetComponent<BoxCollider>();
        RigidBody.useGravity = false;
        RigidBody.freezeRotation = true;
    }

    private void Start()
    {
        RigidBody.velocity = transform.forward * Speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Hitted)
            return;
        //Physics.OverlapSphere(transform.position, ExplosionRadius);
        Hitted = true;
        BoxCollider.enabled = false;
        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
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