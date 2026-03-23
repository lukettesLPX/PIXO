using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public CinemachineImpulseSource damageImpulseSource;
    public UnityEvent onDeath;
    private bool isDead = false;


    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        currentHealth -= damageAmount;
        Debug.Log($"[HealthSystem] {gameObject.name} took {damageAmount} damage. Current health: {currentHealth}");

        if (damageImpulseSource != null)
        {
            damageImpulseSource.GenerateImpulse(1.0f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[HealthSystem] {gameObject.name} (Tag: {tag}) has died. Invoking onDeath.");
        onDeath.Invoke();
        
        if (CompareTag("Player"))
        {
            // Disable player movement/actions
            var controller = GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            // Optional: Set isKinematic on Rigidbody to stop momentum
            var rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // Disable player input/movement scripts
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && (script.GetType().Name.Contains("Input") || script.GetType().Name.Contains("Movement") || script.GetType().Name.Contains("Attack")))
                {
                    script.enabled = false;
                }
            }
        }
        else
        {
            Debug.Log($"[HealthSystem] Destroying non-player object {gameObject.name}");
            Destroy(gameObject);
        }
    }
}
