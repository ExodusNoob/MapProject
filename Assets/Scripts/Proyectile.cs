using UnityEngine;

public class Proyectile : MonoBehaviour
{
    public float TimeDestroy = 15;
    [SerializeField] private M4A1Movement weapon;
    private Rigidbody2D projectileRigidBody2D_;
    [SerializeField] private float speedProjectile;
    [SerializeField] private float damage;

    //PlantillaEnemy enemy;

    private void Awake()
    {
        weapon = GetComponent<M4A1Movement>();
        projectileRigidBody2D_ = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        SetData(speedProjectile, damage);
    }
    public void SetData(float speed, float dmg)
    {
        speedProjectile = speed;
        damage = dmg;

        // Lanza el proyectil hacia adelante
        LaunchProjectile(transform.right);
    }

    public void LaunchProjectile(Vector2 direction)
    {
        projectileRigidBody2D_.linearVelocity = direction * speedProjectile * Time.deltaTime;
        Destroy(gameObject, TimeDestroy);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //enemy = other.GetComponent<PlantillaEnemy>();
        //if (enemy != null)
        //{
        //    enemy.TakeDamage(damage);
        //}
    }

    private void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}
