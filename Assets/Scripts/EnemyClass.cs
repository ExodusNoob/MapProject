using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(name + " murió.");
        Destroy(gameObject);
    }

    public abstract void Attack(); // cada enemigo lo define diferente
}
