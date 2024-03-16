using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Boss : MonoBehaviour, IDamageable
{
    public delegate void EnemyDead();
    public EnemyDead OnEnemyDead = () => { };

    private enum EnemyState
    {
        Idling,
        Patrolling,
        Attacking,
    }

    //Enemy generic setting
    [Header("Enemy Settings")]
    [SerializeField] private TrackingEnemyWeapon EnemyWeapon;
    [SerializeField] ParticleSystem ParticlesOnDestroy;
    [SerializeField] private Collider ColliderToDisable;
    [SerializeField] private float ScoreOnDefeat = 1000f;
    [SerializeField] private EnemyState StartingState = EnemyState.Idling;
    [SerializeField] private float DamageOnCollision = 5f;

    private delegate void EnemyDelegate();
    private EnemyDelegate EnemyBehaviour = () => { };

    private Rigidbody RigidBody;

    //IDamageable
    public float HP { get; set; }
    [SerializeField] float EnemyHP = 10f;

    private bool IsDead = false;

    //Spawnable Index
    public List<RandomData> ObjectsToSpawn { get; set; }
    public float SpawnProbability { get; set; }

    [Tooltip("Distance of Triggering")]
    [SerializeField] private float TriggerDistance = 20f;

    private Transform TargetTransform;

    [Header("Patrolling Gizmo Setting")]
    [SerializeField] private bool DrawGizmo = true;

    private EnemyState CurrentEnemyState;
    private PlayerController PlayerRef;
    private float DistanceFromPlayer = float.MaxValue;

    private void Awake()
    {
        //Set up IDamageable
        HP = EnemyHP;

        //Other
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ChangeState(StartingState);
    }

    private void FixedUpdate()
    {
        if (EnemyBehaviour != null)
            EnemyBehaviour();

        if (PlayerRef != null && !IsDead)
        {
            DistanceFromPlayer = (PlayerRef.transform.position - transform.position).magnitude;
            if (DistanceFromPlayer < TriggerDistance)
                if (CanAttack())
                    ChangeState(EnemyState.Attacking);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        //empty enemy behaviour
        EnemyBehaviour -= EnemyBehaviour;

        switch (newState)
        {
            //Idling behaviour
            case EnemyState.Idling:
                {
                    break;
                }
            //Attacking behaviour
            case EnemyState.Attacking:
                {
                    //Set player as target
                    TargetTransform = PlayerRef.transform;
                    //Attack player
                    EnemyBehaviour += Attack;
                    break;
                }

            default: { break; }
        }
        //update new state
        CurrentEnemyState = newState;
    }

    private void Attack()
    {
        EnemyWeapon.Shoot();
    }

    private bool CanAttack()
    {
        Ray ray = new Ray(transform.position, (PlayerRef.transform.position - transform.position).normalized);
        Debug.DrawRay(transform.position, (PlayerRef.transform.position - transform.position).normalized * 50f, Color.magenta);

        if (Physics.SphereCast(ray, 1f, out RaycastHit hitInfo, TriggerDistance, ~(1 << 8 | 1 << 9), QueryTriggerInteraction.Ignore))
            return hitInfo.collider.CompareTag("Player");
        return false;
    }

    private void Die()
    {
        IsDead = true;
        if (!ParticlesOnDestroy)
        {
            Destroy(RigidBody);
        }
        else
        {
            ColliderToDisable.enabled = false;
            RigidBody.velocity = Vector3.zero;
            ParticlesOnDestroy.Play();
        }
        EnemyBehaviour -= EnemyBehaviour;
        OnEnemyDead();
        ScoreCollectible.OnIncreaseScore?.Invoke(ScoreOnDefeat);
    }

    public void TakeDamage(float Damage)
    {
        if (IsDead) return;

        if (CurrentEnemyState != EnemyState.Attacking)
            ChangeState(EnemyState.Attacking);

        HP -= Damage;

        if (HP < 0)
            Die();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerReady += (PlayerController Player) => 
        {
            PlayerRef = Player;
            EnemyWeapon.Target = Player.transform;
            EnemyWeapon.MaxRaycastDistance = TriggerDistance;
        };
    }

    private void OnDisable()
    {
        PlayerRef = null;
        EnemyBehaviour -= EnemyBehaviour;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage(DamageOnCollision);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!DrawGizmo)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, TriggerDistance);

        //Line
        Handles.color = Color.cyan;
        if (TargetTransform != null)
            Handles.DrawDottedLine(transform.position, TargetTransform.position, 10f);

        if (!PlayerRef)
            return;
        //Draw Player line
        Handles.color = Color.magenta;
        Handles.DrawDottedLine(transform.position, PlayerRef.transform.position, 10f);
    }
#endif
}
