using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SmartProjectile : Projectile
{
    [Header("Smart Projectile Settings")]
    [SerializeField] float TriggerSize = 2.5f;
    [SerializeField] LayerMask Target;
    [SerializeField] float SteeringSpeed = 1f;

    private bool Triggered = false;
    private Transform TargetTransform;

    private Coroutine LookAtTargetCoroutine;

    new private void Awake()
    {
        base.Awake();
    }

    new private void Start()
    {
        //move forward
        base.Start();
    }

    private void FixedUpdate()
    {
        if(!Triggered)
        {
            Collider[] ArrayOfColliders;
            ArrayOfColliders = Physics.OverlapSphere(transform.position, TriggerSize, Target);
            //target acquired
            if(ArrayOfColliders.Length > 0 )
            {
                Triggered = true;
                TargetTransform = ArrayOfColliders[0].transform;
                //reset velocity
                base.RigidBody.velocity = Vector3.zero;
                LookAtTargetCoroutine = StartCoroutine(LookAtTarget());

            }
        }
        else
        {
            RigidBody.velocity = transform.forward * base.Speed;
        }
    }

    private IEnumerator LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(TargetTransform.position - transform.position);
        float time = 0f;

        while(time < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, time);
            time += Time.deltaTime * SteeringSpeed;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, TriggerSize);
    }
}
