using System.Collections;
using UnityEngine;

public class HitScanProjectile : Projectile
{
    [Header("Hitscan Property")]
    [SerializeField] private float HitscanRayDistance = 150f;
    [SerializeField] private float RayThichness = 0.25f;

    private RaycastHit RaycastHit;

    private void Awake()
    {

    }

    new void Start()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.SphereCast(ray, RayThichness, out RaycastHit, HitscanRayDistance, ~(1<<8 | 1<<9)))
        {
            transform.position = RaycastHit.point;
            transform.rotation = Quaternion.Euler(RaycastHit.normal);
            base.ParticlesOnDestroy.Play();
        }
    }

    new private void OnCollisionEnter(Collision collision)
    {

    }

    new protected virtual IEnumerator OnCollisionCoroutine()
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