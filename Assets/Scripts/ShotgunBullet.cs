using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletLifeTime = 2f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Vector2 direction;

    public int Damage { get { return bulletDamage; } set { bulletDamage = value; } }


    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        Destroy(gameObject, bulletLifeTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
