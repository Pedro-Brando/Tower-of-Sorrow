using UnityEngine;
using MoreMountains.CorgiEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab do inimigo a ser instanciado
    public float spawnInterval = 5f; // Intervalo entre spawns
    public int maxEnemies = 5; // Número máximo de inimigos ativos
    private int currentEnemies = 0;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies)
            return;

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        enemy.GetComponent<Health>().OnDeath += EnemyDied;
        currentEnemies++;
    }

    void EnemyDied()
    {
        currentEnemies--;
    }
}
