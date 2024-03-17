using UnityEngine;

public class DamageZone : SpecialZone
{
    [Header("Damage Zone Settings")]
    [SerializeField] private float Damage = 0.5f;
    protected override void ApplyEffect(Collider collider)
    {
        if(collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            damageable.TakeDamage(Damage);
    }
}
