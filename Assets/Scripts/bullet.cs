using UnityEngine;

public class bullet : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 2;
    [SerializeField] private float bulletLifeTime = 1.5f;


    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Transform target;

    public int Damage { get { return bulletDamage; } }

    public void SetTarget(Transform _target)
    {
        target = _target;
        Destroy(gameObject, bulletLifeTime);
    }
    private void FixedUpdate()
    {

        if(!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
