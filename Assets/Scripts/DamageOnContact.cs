using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageCooldown = 2.0f;
    private float lastDamageTime;

    private void OnCollisionEnter(Collision collision)
    {
        TryApplyDamage(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryApplyDamage(collision.gameObject);
    }

    private void TryApplyDamage(GameObject target)
    {
        if (target.CompareTag("Player") && Time.time - lastDamageTime >= damageCooldown)
        {
            HealthSystem health = target.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
            }
        }
    }
}
