using UnityEngine;
using UnityEngine.AI;

public class CollisionStabilizer : MonoBehaviour
{
    private void Awake()
    {
        SetupPlayer();
        SetupEnemies();
    }

    private void SetupPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            NavMeshObstacle obstacle = player.GetComponent<NavMeshObstacle>();
            if (obstacle == null) obstacle = player.AddComponent<NavMeshObstacle>();
            
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                obstacle.shape = NavMeshObstacleShape.Capsule;
                obstacle.center = cc.center;
                obstacle.radius = cc.radius;
                obstacle.height = cc.height;
                obstacle.carving = true;
            }
        }
    }

    private void SetupEnemies()
    {
        EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            EnsureCollider(enemy.gameObject);
        }

        RangedEnemyAI[] rangedEnemies = Object.FindObjectsByType<RangedEnemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in rangedEnemies)
        {
            EnsureCollider(enemy.gameObject);
        }
    }

    private void EnsureCollider(GameObject go)
    {
        CapsuleCollider col = go.GetComponent<CapsuleCollider>();
        if (col == null) col = go.AddComponent<CapsuleCollider>();
        
        NavMeshAgent agent = go.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            col.center = new Vector3(0, agent.height / 2f, 0);
            col.radius = agent.radius;
            col.height = agent.height;
        }
    }
}
