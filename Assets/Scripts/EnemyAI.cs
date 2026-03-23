using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private float stunTimer = 0f;
    public float attackRange = 1.8f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (agent != null)
        {
            agent.stoppingDistance = attackRange + 0.5f;
            agent.autoBraking = true;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (agent != null) agent.isStopped = true;
            return;
        }

        if (playerTransform == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange + 0.5f)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            if (attackTimer <= 0f)
            {
                HealthSystem playerHealth = playerTransform.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    attackTimer = attackCooldown;
                    if (impulseSource != null) impulseSource.GenerateImpulse(1.0f);
                }
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
    }

    public void ApplyKnockback(Vector3 pushVector, float stunDuration)
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) agent.Move(pushVector);
        stunTimer = stunDuration;
    }
}
