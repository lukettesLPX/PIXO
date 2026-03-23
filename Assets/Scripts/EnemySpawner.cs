using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float checkRadius = 15f;
    public float spawnCooldown = 5f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnCooldown)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);
            int enemyCount = 0;
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Enemy")) enemyCount++;
            }
            if (enemyCount == 0 && enemyPrefab != null) Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
