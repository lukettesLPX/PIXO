using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 15f;
    public float fireCooldown = 2f;
    private int currentWaypointIndex = 0;
    private float fireTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private GameObject playerTarget;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        if (waypoints != null && waypoints.Length > 0 && navMeshAgent != null) navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void Update()
    {
        if (playerTarget == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.transform.position);
        fireTimer -= Time.deltaTime;
        if (distanceToPlayer > attackRange) Patrol();
        else Combat();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || navMeshAgent == null) return;
        navMeshAgent.isStopped = false;
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Combat()
    {
        if (navMeshAgent != null) navMeshAgent.isStopped = true;
        Vector3 targetPos = playerTarget.transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        if (fireTimer <= 0) Shoot();
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Vector3 spawnPos = firePoint.position + firePoint.forward * 0.6f;
            GameObject arrow = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);
            
            Collider[] shooterColliders = GetComponentsInChildren<Collider>();
            Collider arrowCollider = arrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                foreach (var c in shooterColliders) Physics.IgnoreCollision(arrowCollider, c);
            }

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = firePoint.forward * 20f;
            fireTimer = fireCooldown;
        }
    }
}
