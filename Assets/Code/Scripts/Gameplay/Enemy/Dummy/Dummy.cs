using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Dummy : MonoBehaviour, IDamageable
{
    [SerializeField] private float HealthPoints = 0.1f;
    public float HP { get; set; }

    private void Awake()
    {
        HP = HealthPoints;
    }

    public void TakeDamage(float Damage)
    {
        Debug.Log("Damage: "+Damage);
        Destroy(this.gameObject);
    }
}
