using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public void SpawnAllEnemies()
    {
        EnemySpawner[] spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);

        foreach (EnemySpawner spawner in spawners)
        {
            spawner.SpawnEnemies();
        }

        Debug.Log("Todos los enemigos fueron generados.");
    }
}
