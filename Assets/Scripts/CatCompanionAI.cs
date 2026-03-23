using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatCompanionAI : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance = 2.5f;
    public float fleeRadius = 8f;
    public float fleeDistance = 5f;
    public LayerMask enemyLayer;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.stoppingDistance = followDistance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    private void Update()
    {
        if (playerTarget == null || navMeshAgent == null) return;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fleeRadius);
        bool isFleeing = false;
        Transform nearestEnemy = null;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                isFleeing = true;
                nearestEnemy = hitCollider.transform;
                break;
            }
        }

        if (isFleeing && nearestEnemy != null)
        {
            navMeshAgent.stoppingDistance = 0f;
            Vector3 fleeDirection = (transform.position - nearestEnemy.position).normalized;
            Vector3 fleeTarget = transform.position + (fleeDirection * fleeDistance);
            navMeshAgent.SetDestination(fleeTarget);
        }
        else
        {
            navMeshAgent.stoppingDistance = followDistance;
            float distance = Vector3.Distance(transform.position, playerTarget.position);
            if (distance > followDistance)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(playerTarget.position);
            }
            else
            {
                navMeshAgent.isStopped = true;
                transform.LookAt(new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z));
            }
        }
    }
}
