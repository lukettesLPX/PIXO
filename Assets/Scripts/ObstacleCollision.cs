using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    public float collisionDamageCoefficient = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health == null) return;
        if (collision.gameObject.CompareTag("Player")) health.TakeDamage((int)(10 * collisionDamageCoefficient));
        else if (collision.gameObject.CompareTag("Enemy")) health.TakeDamage((int)(5 * collisionDamageCoefficient));
    }
}
