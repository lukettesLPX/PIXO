using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 18f;
    public int damage = 15;
    public float lifetime = 4f;

    private Transform target;
    private Rigidbody rb;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;
            if (target != null)
            {
                Vector3 dir = (new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position).normalized;
                rb.linearVelocity = dir * speed;
                transform.forward = dir;
            }
            else
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    private void Update()
    {
        if (rb == null && target != null)
        {
            Vector3 dir = (new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.forward = dir;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !other.CompareTag("Player")) return;

        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.GetComponentInParent<EnemyAI>() != null || 
                 other.GetComponentInParent<RangedEnemyAI>() != null || 
                 other.CompareTag("Enemy") || 
                 other.CompareTag("RangeGuard") || 
                 other.name.Contains("Guard"))
        {
            return;
        }
        else
        {
            Debug.Log("[Projectile] Destroyed by: " + other.name + " (Tag: " + other.tag + ")");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponentInParent<EnemyAI>() != null || collision.gameObject.GetComponentInParent<RangedEnemyAI>() != null || collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        else
        {
            Debug.Log("[Projectile] Destroyed by: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");
            Destroy(gameObject);
        }
    }
}
