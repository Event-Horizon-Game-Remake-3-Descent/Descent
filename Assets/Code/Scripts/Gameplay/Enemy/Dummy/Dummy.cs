using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Dummy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Hitted By: " + collision.gameObject.name + "\nDamage: "+collision.gameObject.GetComponent<Projectile>().Damage);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Hitted By: " + other.gameObject.name + "\nDamage: "+ other.GetComponent<Projectile>().Damage);
            Destroy(this.gameObject);
        }
    }
}
