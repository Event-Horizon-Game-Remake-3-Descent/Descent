using UnityEditor;

public interface IDamageable
{
    float HP { set; get; }

    public void TakeDamage(float damage);
}