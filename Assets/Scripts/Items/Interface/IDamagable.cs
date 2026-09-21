using UnityEngine;

public interface IDamagable
{
    public GameObject GameObject { get; }

    public void TakeDamage(int damage);
}
