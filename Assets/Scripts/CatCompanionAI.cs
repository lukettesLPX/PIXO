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
    private Animator animator;
    private Vector3 lastPlayerPosition;
    private CharacterController playerController;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        
        if (navMeshAgent != null)
        {
            navMeshAgent.stoppingDistance = followDistance;
            navMeshAgent.updateRotation = true;
            navMeshAgent.acceleration = 8f; 
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
            lastPlayerPosition = playerTarget.position;
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (playerTarget == null || navMeshAgent == null) return;
        
        // Update Animator
        float currentSpeed = navMeshAgent.velocity.magnitude;
        
        // Check if player is nearly still
        bool isPlayerMoving = false;
        if (playerController != null)
        {
            isPlayerMoving = new Vector3(playerController.velocity.x, 0, playerController.velocity.z).magnitude > 0.1f;
        }

        // If player is still and we are close enough, force Idle animation
        if (!isPlayerMoving && Vector3.Distance(transform.position, playerTarget.position) <= followDistance + 0.2f)
        {
            currentSpeed = 0f;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }

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
