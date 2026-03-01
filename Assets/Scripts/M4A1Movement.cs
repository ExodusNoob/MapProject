using System.Collections;
using UnityEngine;

public class M4A1Movement : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public float DistanciaDeAtk;
    public float VelocidadIdaVuelta = 0.1f; // Tiempo que tarda en ir y volver
    private bool isWeaponFacingRigh = true;

    //gun
    [SerializeField] private Proyectile projectilePrefab;
    [SerializeField] private Transform shootPosition;
    public float speedAmmo;

    //PlantillaEnemy enemy;

    private Vector3 originalLocalPosition;
    [SerializeField] private bool IsAtackingAway = false;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
    }
    private void Update()
    {
        RotateWeapon();

        if (Input.GetMouseButtonDown(0) && !IsAtackingAway)
        {
            StartCoroutine(Thrust());
        }

        if (gameObject.tag == "Gun")
        {
            FlipBasedOnMouse();

            if (Input.GetMouseButtonDown(0))
            {
                Proyectile newProjectile = Instantiate(projectilePrefab, shootPosition.position, transform.rotation);
                newProjectile.SetData(speedAmmo, damage);
            }

        }

    }
    private void RotateWeapon()
    {
        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FlipBasedOnMouse()
    {
        // Obtener la posición del mouse en el mundo
        Vector3 mousePosition_ = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Comparar posición X del mouse con la del objeto
        bool isMouseRight = mousePosition_.x > transform.position.x;

        if ((isWeaponFacingRigh && !isMouseRight) || (!isWeaponFacingRigh && isMouseRight))
        {
            isWeaponFacingRigh = !isWeaponFacingRigh;
            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;
        }
    }
    private IEnumerator Thrust()
    {
        IsAtackingAway = true;

        // Dirección hacia donde está apuntando
        Vector3 forward = transform.right * DistanciaDeAtk;

        // Movimiento hacia adelante
        float elapsed = 0f;
        while (elapsed < VelocidadIdaVuelta)
        {
            transform.localPosition = originalLocalPosition + forward * (elapsed / VelocidadIdaVuelta);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Movimiento hacia atrás
        elapsed = 0f;
        while (elapsed < VelocidadIdaVuelta)
        {
            transform.localPosition = originalLocalPosition + forward * (1 - (elapsed / VelocidadIdaVuelta));
            elapsed += Time.deltaTime;
            yield return null;
        }
        //if (enemy != null)
        //{
        //    enemy.DesableAbleCollision();
        //
        //}

        transform.localPosition = originalLocalPosition;
        IsAtackingAway = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //enemy = other.GetComponent<PlantillaEnemy>();
        //if (enemy != null)
        //{
        //    enemy.TakeDamage(damage);
        //}
    }
}
