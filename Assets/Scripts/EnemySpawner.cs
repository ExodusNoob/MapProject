using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemies = 1;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float wallMargin = 0.8f;

    private BoxCollider2D roomCollider;
    private bool hasSpawned = false;

    private void Awake()
    {
        roomCollider = GetComponent<BoxCollider2D>();
    }

    public void SpawnEnemies()
    {
        if (hasSpawned || roomCollider == null || enemyPrefab == null)
            return;

        int amount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = GetValidSpawnPoint();
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }

        hasSpawned = true;
    }

    private Vector3 GetValidSpawnPoint()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(roomCollider.bounds.min.x + wallMargin, roomCollider.bounds.max.x - wallMargin),
                Random.Range(roomCollider.bounds.min.y + wallMargin, roomCollider.bounds.max.y - wallMargin)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return roomCollider.bounds.center;
    }
}
