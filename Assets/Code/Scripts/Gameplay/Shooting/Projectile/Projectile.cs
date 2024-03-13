using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] public float Damage = 1f;
    [SerializeField] protected float ExplosionRadius = 0f;
    [SerializeField] protected LayerMask ExplosionLayerMask;
    [SerializeField] protected float Speed = 15f;
    [SerializeField] protected Transform MeshToHide;

    [Header("VFX")]
    [SerializeField] protected ParticleSystem ParticlesOnDestroy;

    [Header("SFX")]
    [SerializeField] protected AudioClip ExplosionSFX;

#if UNITY_EDITOR
    [Header("Gizmos")]
    [SerializeField] Color ExplosionGizmoColor = Color.yellow;
#endif

    protected Rigidbody RigidBody;
    protected CapsuleCollider CapsuleCollider;
    protected AudioSource AudioSource;

    protected bool Hit = false;

    protected void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        RigidBody = GetComponent<Rigidbody>();
        RigidBody.freezeRotation = true;
        RigidBody.useGravity = false;
        ParticlesOnDestroy = GetComponent<ParticleSystem>();
    }

    protected void Start()
    {
        RigidBody.velocity = transform.forward * Speed;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        //Stop bullet
        this.RigidBody.velocity = Vector3.zero;
        //Stop collision detection
        CapsuleCollider.enabled = false;
        //Hide Bullet
        if(MeshToHide)
            MeshToHide.gameObject.SetActive(false);
        
        //Explode if needed
        if (ExplosionRadius > 0)
        {
            //Get Colliders
            Collider[] ArrayOfCollider;
            ArrayOfCollider = Physics.OverlapSphere(transform.position, ExplosionRadius, ExplosionLayerMask, QueryTriggerInteraction.Ignore);
            
            //Apply Damage
            for (int i = 0; i < ArrayOfCollider.Length; i++)
                if (ArrayOfCollider[i].gameObject.TryGetComponent<IDamageable>(out IDamageable Damageable))
                    Damageable.TakeDamage(Damage);
        }
        else //just apply damage
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable Damageable))
                Damageable.TakeDamage(Damage);
        }

        StartCoroutine(OnCollisionCoroutine());
    }

    protected virtual IEnumerator OnCollisionCoroutine()
    {
        if (ExplosionSFX != null && AudioSource!=null) //play explosion sound if present
        {
            this.AudioSource.Play();
        }
        
        if(ParticlesOnDestroy != null)
            ParticlesOnDestroy.Play();
        else
            Destroy(this.gameObject);

        yield return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        if(ExplosionRadius > 0)
        {
            Gizmos.color = ExplosionGizmoColor;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
#endif

}