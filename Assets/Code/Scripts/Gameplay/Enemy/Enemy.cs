using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Enemy : MonoBehaviour, IDamageable, IPatrollable, ISpawnable
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
    [SerializeField] private float RotationSpeed = 1f;
    [SerializeField] private EnemyWeapon EnemyWeapon;
    [SerializeField] private EnemyState StartingState = EnemyState.Idling;
    [SerializeField] private float DamageOnCollision = 5f;

    private delegate void EnemyDelegate();
    private EnemyDelegate EnemyBehaviour = () => { };

    private Rigidbody RigidBody;
    private Coroutine TurnRoutine = null;

    private EnemyState CurrentEnemyState = EnemyState.Patrolling;

    //IDamageable
    public float HP { get; set ; }
    [SerializeField] private float EnemyHP = 10f;
    private bool IsDead = false;

    //IPatrollable variables
    [Header("Patrolling Settings")]
    //Patrolling points
    [SerializeField] private PatrollingData PatrollingData;
    public PatrollingData PatrollingPoints { get; set; }
    //Speed
    [SerializeField] private float PathSpeed = 3f;
    public float PatrollingSpeed { get; set ; }
    //Threshold
    [SerializeField] private float PathThreshold = 0.5f;
    public float PatrollingThreshold { get; set; }

    [SerializeField] private float DecelerationDistance = 1f;
    public float PatrollingDecelerationDistance { get; set; }
    //Current patrolling Index
    public int PatrollingIndex { get; set; }


    [Tooltip("Distance of Triggering")]
    [SerializeField] private float TriggerDistance = 20f;
    [Tooltip("Distance kept from player")]
    [SerializeField] private float DistanceOnceTriggered = 5f;

    private Transform TargetTransform;

    //Spawnable
    [Header("On Death Settings")]
    [SerializeField] ParticleSystem ParticlesOnDestroy;
    [SerializeField] private Collider ColliderToDisable;
    [SerializeField] private float ScoreOnDefeat = 1000f;
    [SerializeField] private List<RandomData> SpawnOnDeath;
    public List<RandomData> ObjectsToSpawn { get; set; }

    [Header("Patrolling Gizmo Setting")]
    [SerializeField] private bool DrawGizmo = true;
    [SerializeField] private Color PathColor = Color.magenta;
    [SerializeField] private Color PointColor = Color.red;
    [SerializeField] private Color PointThresholdColor = Color.yellow;
    [SerializeField] private Color PointDecelerationDistanceColor = Color.green;
    [SerializeField] private float PointsSize = 0.02f;

    private PlayerController PlayerRef;
    private float DistanceFromPlayer = float.MaxValue;

    private void Awake()
    {
        //set up IPatrollable
        PatrollingPoints = PatrollingData;
        PatrollingSpeed = PathSpeed;
        PatrollingIndex = 0;
        PatrollingThreshold = PathThreshold;
        PatrollingDecelerationDistance = DecelerationDistance;

        if(PatrollingData.PatrollingPositions.Count > 0)
            TargetTransform = PatrollingData.PatrollingPositions[0];

        //Set up IDamageable
        HP = EnemyHP;

        //Set up ISpawnable
        ObjectsToSpawn = SpawnOnDeath;

        //Other
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        CurrentEnemyState = StartingState;
        ChangeState(CurrentEnemyState);
    }

    private void FixedUpdate()
    {
        if(EnemyBehaviour != null)
            EnemyBehaviour();

        if(PlayerRef != null && !IsDead)
        {
            DistanceFromPlayer = (PlayerRef.transform.position - transform.position).magnitude;
            if (DistanceFromPlayer < TriggerDistance)
                if(CanAttack())
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
            //Patrolling behaviour
            case EnemyState.Patrolling:
            {
                TargetTransform = PatrollingData.PatrollingPositions[PatrollingIndex];
                EnemyBehaviour += Patrol;
                break;
            }
            //Attacking behaviour
            case EnemyState.Attacking:
            {
                //Set player as target
                TargetTransform = PlayerRef.transform;
                //rotate towards the player
                TurnRoutine = StartCoroutine(TurnCoroutine());
                //Attack player
                EnemyBehaviour += Attack;
                break;
            }
            
            default: { break; }
        }
        //update new state
        CurrentEnemyState = newState;
    }

    private void ResetState()
    {
        ChangeState(StartingState);
    }

    #region PATROLLING
    public void Patrol()
    {
        float targetDist = (TargetTransform.position - transform.position).magnitude;

        //check distance
        if (targetDist < PathThreshold)
        {
            //Increase Patrolling index
            PatrollingIndex++;
            if (PatrollingIndex > PatrollingData.PatrollingPositions.Count - 1)
                PatrollingIndex = 0;
            //Update transform
            TargetTransform = PatrollingData.PatrollingPositions[PatrollingIndex];
            //Turn
            TurnRoutine = StartCoroutine(TurnCoroutine());
        }
        else
        {
            if(TurnRoutine == null)
            {
                MoveToTarget(0f);
            }
        }
    }

    private void MoveToTarget(float KeepDistance)
    {
        float targetDist = (TargetTransform.position - transform.position).magnitude - KeepDistance;
        //move forward
        transform.LookAt(TargetTransform);
        float decelerationActuator = 1f;

        if (targetDist <= PatrollingDecelerationDistance)
            decelerationActuator = targetDist / PatrollingDecelerationDistance;

        RigidBody.velocity = transform.forward * PatrollingSpeed * decelerationActuator;
    }

    private IEnumerator TurnCoroutine()
    {
        Quaternion targetRotation = Quaternion.LookRotation(TargetTransform.position - transform.position);
        float time = 0f;

        while (time < 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, time);
            time += Time.deltaTime * RotationSpeed;
            yield return null;
        }
        TurnRoutine = null;
    }

    #endregion

    #region ATTACK
    private void Attack()
    {
        if (TurnRoutine == null)
        {
            MoveToTarget(DistanceOnceTriggered);
            EnemyWeapon.Shoot();
        }
    }
    private bool CanAttack()
    {
        Ray ray = new Ray(transform.position, (PlayerRef.transform.position - transform.position).normalized);
        Debug.DrawRay(transform.position, (PlayerRef.transform.position - transform.position).normalized * 50f, Color.magenta);

        if (Physics.SphereCast(ray, 1f, out RaycastHit hitInfo, TriggerDistance, ~(1 << 8 | 1 << 9), QueryTriggerInteraction.Ignore))
            return hitInfo.collider.CompareTag("Player");
        return false;
    }
    #endregion

    #region DEATH
    private void Die()
    {
        SpawnObject();

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

    public void SpawnObject()
    {
        float randomValue = UnityEngine.Random.value;
        float[] probabilityArray = new float[SpawnOnDeath.Count];
        probabilityArray[0] = SpawnOnDeath[0].Probability;

        for (int i = 1; i < probabilityArray.Length; i++)
        {
            probabilityArray[i] = probabilityArray[i-1] + SpawnOnDeath[i].Probability;
        }

        for(int i = 0; i < probabilityArray.Length; i++)
        {
            if (randomValue < probabilityArray[i])
            {
                Instantiate(SpawnOnDeath[i].ObjectToGenerate, transform.position, Quaternion.identity);
                return;
            }
        }

    }
    #endregion

    public void TakeDamage(float Damage)
    {
        if (IsDead) return;

        if (CurrentEnemyState != EnemyState.Attacking)
            ChangeState(EnemyState.Attacking);

        HP -= Damage;

        if (HP < 0)
        {
            Die();
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerReady += (PlayerController Player) => PlayerRef = Player;
        PlayerController.OnPlayerDead += ResetState;
    }

    private void OnDisable()
    {
        PlayerRef = null;
        EnemyBehaviour -= EnemyBehaviour;
        PlayerController.OnPlayerDead -= ResetState;
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
        if(!DrawGizmo)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, TriggerDistance);

        if(!Application.isPlaying) //Draw All Gizmos
        {
            if (PatrollingData.PatrollingPositions.Count > 0)
            {
                //Draw Points
                for (int i = 0; i < PatrollingData.PatrollingPositions.Count; i++)
                {
                    Gizmos.color = PointColor;
                    Gizmos.DrawSphere(PatrollingData.PatrollingPositions[i].position, PointsSize);
                }
                //Draw PathThreshold
                for (int i = 0; i < PatrollingData.PatrollingPositions.Count; i++)
                {
                    Gizmos.color = PointThresholdColor;
                    Gizmos.DrawWireSphere(PatrollingData.PatrollingPositions[i].position, PathThreshold);
                }
                //draw Decelerations
                for (int i = 0; i < PatrollingData.PatrollingPositions.Count; i++)
                {
                    Gizmos.color = PointDecelerationDistanceColor;
                    Gizmos.DrawWireSphere(PatrollingData.PatrollingPositions[i].position, DecelerationDistance);
                }

                //Initial Path
                Handles.color = Color.cyan;
                Handles.DrawDottedLine(transform.position, PatrollingData.PatrollingPositions[0].position, 10f);
            }

            //Lines
            if (PatrollingData.PatrollingPositions.Count >= 1)
            {
                Handles.color = PathColor;
                //Draw Lines
                for (int i = 1; i < PatrollingData.PatrollingPositions.Count; i++)
                {
                    Handles.DrawLine(PatrollingData.PatrollingPositions[i - 1].position, PatrollingData.PatrollingPositions[i].position);
                }
                //Draw final line
                Handles.DrawLine(PatrollingData.PatrollingPositions[0].position, PatrollingData.PatrollingPositions[PatrollingData.PatrollingPositions.Count - 1].position);
            }
        }
        else //Draw Target and path
        {
            //Point
            Gizmos.color = PointColor;
            if(TargetTransform != null)
                Gizmos.DrawSphere(TargetTransform.position, PointsSize);

            //Threshold
            Gizmos.color = PointThresholdColor;
            if (TargetTransform != null)
                Gizmos.DrawWireSphere(TargetTransform.position, PathThreshold);

            //Deceleration
            Gizmos.color = PointDecelerationDistanceColor;
            if (TargetTransform != null)
                Gizmos.DrawWireSphere(TargetTransform.position, PatrollingDecelerationDistance);

            //Line
            Handles.color = Color.cyan;
            if (TargetTransform != null)
                Handles.DrawDottedLine(transform.position, TargetTransform.position, 10f);

            //Draw Player line
            Handles.color = Color.magenta;
            Handles.DrawDottedLine(transform.position, PlayerRef.transform.position, 10f);
        }
    }
#endif
}
