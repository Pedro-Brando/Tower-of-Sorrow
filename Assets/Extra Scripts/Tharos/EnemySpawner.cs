using UnityEngine;
using MoreMountains.CorgiEngine;
using System.Collections.Generic; // Necessário para usar List<T>

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Lista de prefabs de inimigos a serem instanciados
    public float spawnInterval = 5f; // Intervalo entre spawns
    public int maxEnemies = 5; // Número máximo de inimigos ativos
    private int currentEnemies = 0;

    public float activationDistance = 10f; // Distância para ativação
    private Transform playerTransform;
    private bool isVisible = false; // Flag para visibilidade na câmera

    void Start()
    {
        // Inicia o processo de spawning
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);

        // Encontra o jogador na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Jogador não encontrado. Certifique-se de que o objeto do jogador tem a tag 'Player'.");
        }
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies)
            return;

        // Verifica se o jogador está próximo ou se o spawner está visível na câmera
        bool shouldSpawn = false;

        // Checa a distância até o jogador
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= activationDistance)
            {
                shouldSpawn = true;
            }
        }

        // Checa se o spawner está visível na câmera
        if (isVisible)
        {
            shouldSpawn = true;
        }

        // Se nenhuma condição for atendida, não spawna inimigo
        if (!shouldSpawn)
            return;

        // Seleciona aleatoriamente um prefab da lista
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Nenhum prefab de inimigo foi atribuído ao spawner.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        // Instancia o inimigo selecionado
        GameObject enemy = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        enemy.GetComponent<Health>().OnDeath += EnemyDied;
        currentEnemies++;
    }

    void EnemyDied()
    {
        currentEnemies--;
    }

    // Método chamado quando o objeto entra na visão da câmera
    void OnBecameVisible()
    {
        isVisible = true;
    }

    // Método chamado quando o objeto sai da visão da câmera
    void OnBecameInvisible()
    {
        isVisible = false;
    }
}
