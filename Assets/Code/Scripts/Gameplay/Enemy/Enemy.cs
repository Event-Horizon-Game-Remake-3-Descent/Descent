using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IPatrollable
{
    //Enemy generic setting
    [Header("Enemy Settings")]
    [SerializeField] private float RotationSpeed = 1f;
    [SerializeField] private Weapon EnemyWeapon;
    [SerializeField] ParticleSystem ParticlesOnDestroy;

    private delegate void EnemyState();
    private EnemyState EnemyBehaviour = () => { };

    private Rigidbody RigidBody;
    [SerializeField] private SphereCollider SphereColliderToRemove;
    private Coroutine TurnRoutine = null;

    //IDamageable
    public float HP { get; set ; }
    [SerializeField] float EnemyHP = 10f;

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
    //Current Index
    public int PatrollingIndex { get; set; }

    private Transform TargetTransform;

    [Header("Patrolling Gizmo Setting")]
    [SerializeField] private bool DrawGizmo = true;
    [SerializeField] private Color PathColor = Color.magenta;
    [SerializeField] private Color PointColor = Color.red;
    [SerializeField] private Color PointThresholdColor = Color.yellow;
    [SerializeField] private Color PointDecelerationDistanceColor = Color.green;
    [SerializeField] private float PointsSize = 0.02f;

    private void Awake()
    {
        //set up IPatrollable
        PatrollingPoints = PatrollingData;
        PatrollingSpeed = PathSpeed;
        PatrollingIndex = 0;
        PatrollingThreshold = PathThreshold;
        PatrollingDecelerationDistance = DecelerationDistance;

        TargetTransform = PatrollingData.PatrollingPositions[PatrollingIndex];

        //Set up IDamageable
        HP = EnemyHP;

        //Other
        RigidBody = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        TurnRoutine = StartCoroutine(TurnCoroutine());
        EnemyBehaviour += Patrol;
    }

    private void FixedUpdate()
    {
        if(EnemyBehaviour != null)
            EnemyBehaviour();
    }

    public void TakeDamage(float Damage)
    {
        if (IsDead) return;
        HP -= Damage;

        if (HP < 0)
        {
            IsDead = true;
            if (!ParticlesOnDestroy)
                Destroy(RigidBody);
            else
            {
                SphereColliderToRemove.enabled = false;
                RigidBody.velocity = Vector3.zero;
                ParticlesOnDestroy.Play();
            }
            EnemyBehaviour -= EnemyBehaviour;
        }
    }

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
                MoveToTarget();
            }
        }
    }

    private void MoveToTarget()
    {
        float targetDist = (TargetTransform.position - transform.position).magnitude;
        //move forward
        transform.LookAt(TargetTransform);
        float decelerationActuator = 1f;

        if (targetDist <= PatrollingDecelerationDistance)
            decelerationActuator = targetDist / PatrollingDecelerationDistance;

        RigidBody.velocity = transform.forward * PatrollingSpeed * decelerationActuator;
    }

    private void Attack()
    {
        if (TurnRoutine == null)
            MoveToTarget();
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

    private void OnTriggerEnter(Collider other)
    {
        //attak player
        EnemyBehaviour -= Patrol;
        TurnRoutine = StartCoroutine(TurnCoroutine());
        TargetTransform = other.transform;
        EnemyBehaviour += Attack;
    }

    private void OnTriggerExit(Collider other)
    {
        //back to patrolling
        EnemyBehaviour -= Attack;
        TargetTransform = PatrollingData.PatrollingPositions[PatrollingIndex];
        EnemyBehaviour += Patrol;
    }

    private void OnDisable()
    {
        EnemyBehaviour -= EnemyBehaviour;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!DrawGizmo)
            return;

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
            if (PatrollingData.PatrollingPositions.Count >= 2)
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
            Gizmos.DrawSphere(TargetTransform.position, PointsSize);

            //Threshold
            Gizmos.color = PointThresholdColor;
            Gizmos.DrawWireSphere(TargetTransform.position, PathThreshold);

            //Deceleraion
            Gizmos.color = PointDecelerationDistanceColor;
            Gizmos.DrawWireSphere(TargetTransform.position, PatrollingDecelerationDistance);

            //Line
            Handles.color = Color.cyan;
            Handles.DrawDottedLine(transform.position, TargetTransform.position, 10f);
        }
    }
#endif
}
