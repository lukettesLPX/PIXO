using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatCompanionAI : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance = 3.5f;
    public float movementBuffer = 1.0f;
    public float rotationSpeed = 2.0f;
    public float fleeRadius = 8f;
    public float fleeDistance = 5f;
    
    private NavMeshAgent navMeshAgent;
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.stoppingDistance = followDistance;
            navMeshAgent.updateRotation = true;
            // Suavizado
            navMeshAgent.acceleration = 8f; 
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
            lastPlayerPosition = playerTarget.position;
        }
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
            
            // Solo actualizar el destino si el jugador se ha movido más de lo que indica el buffer
            if (Vector3.Distance(playerTarget.position, lastPlayerPosition) > movementBuffer)
            {
                lastPlayerPosition = playerTarget.position;
                navMeshAgent.SetDestination(playerTarget.position);
            }

            // Si estamos cerca de la distancia de parada, miramos al jugador suavemente
            float remainingDistance = navMeshAgent.remainingDistance;
            if (!navMeshAgent.pathPending && remainingDistance <= navMeshAgent.stoppingDistance)
            {
                Vector3 lookPos = playerTarget.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                    // Rotacion
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
    }
}
