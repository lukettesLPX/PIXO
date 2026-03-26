using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public int attackDamage = 25;
    public float knockbackForce = 3f;
    public float stunDuration = 0.5f;
    public CinemachineImpulseSource impulseSource;
    public AudioClip attackSound;
    public GameObject gatheringTool;
    public float resourceDamage = 25f;
    private float comboShakeIntensity = 0.2f;
    private float lastAttackTime = 0f;

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) || 
                (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame))
            {
                PerformAttack();
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    private void PerformAttack()
    {
        if (Time.time - lastAttackTime < 1.5f)
        {
            comboShakeIntensity += 0.3f;
            comboShakeIntensity = Mathf.Clamp(comboShakeIntensity, 0.2f, 1.5f);
        }
        else
        {
            comboShakeIntensity = 0.2f;
        }
        lastAttackTime = Time.time;

        if (attackSound != null && AudioManager.instance != null) AudioManager.instance.PlaySFX(attackSound);

        if (impulseSource != null) impulseSource.GenerateImpulse(comboShakeIntensity);
        if (attackPoint == null) return;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);
        foreach (Collider hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("RangeGuard"))
            {
                HealthSystem health = hit.GetComponentInParent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage);
                    Vector3 pushDirection = (hit.transform.position - transform.position).normalized;
                    pushDirection.y = 0;
                    pushDirection.Normalize();
                    Vector3 finalPush = pushDirection * knockbackForce;
                    EnemyAI enemyAI = hit.GetComponentInParent<EnemyAI>();
                    if (enemyAI != null) enemyAI.ApplyKnockback(finalPush, stunDuration);
                }
            }
            
            ResourceNode node = hit.GetComponentInParent<ResourceNode>();
            if (node != null && gatheringTool != null)
            {
                node.TakeDamage(resourceDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
