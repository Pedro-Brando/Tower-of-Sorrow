using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/FuriaFogoFatuo")]
public class AttackFuriaFogoFatuo : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject FlamePrefab;            // Prefab da chama fátua
    public GameObject SummonEffectPrefab;     // Prefab do efeito de invocação

    [Header("Parâmetros do Ataque")]
    public int NumberOfFlames = 3;            // Número de chamas fátuas a serem geradas
    public float FlameSpawnRadius = 5f;       // Raio ao redor de Uldric onde as chamas serão geradas
    public float FlameSpeed = 3f;             // Velocidade de movimento das chamas
    public float FlameDuration = 5f;          // Duração de vida das chamas
    public float DelayBetweenFlames = 0.5f;   // Tempo entre a geração de cada chama

    [Header("Cooldown")]
    public float AttackCooldown = 10f;        // Tempo de espera antes de poder realizar o próximo ataque

    private Transform _uldricTransform;        // Referência ao transform de Uldric
    private Transform _playerTransform;         // Referência ao transform do jogador

    public override IEnumerator Execute()
    {
        // Obter referência ao Uldric e ao jogador
        if (_uldricTransform == null)
        {
            _uldricTransform = this.transform.parent; // Assumindo que o ScriptableObject está dentro de um GameObject filho de Uldric
        }

        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("Player não encontrado! Certifique-se de que o jogador tem a tag 'Player'.");
                yield break;
            }
        }

        // Instanciar o efeito de invocação
        if (SummonEffectPrefab != null)
        {
            Instantiate(SummonEffectPrefab, _uldricTransform.position, Quaternion.identity);
        }

        // Esperar um breve momento antes de começar a gerar as chamas
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < NumberOfFlames; i++)
        {
            // Determinar a posição de spawn da chama
            Vector3 spawnPosition = _uldricTransform.position + Random.insideUnitSphere * FlameSpawnRadius;
            spawnPosition.z = 0f; // Garantir que a posição esteja no plano correto

            // Instanciar a chama fátua
            GameObject flame = Instantiate(FlamePrefab, spawnPosition, Quaternion.identity);

            // Configurar a direção da chama em direção ao jogador
            Vector3 direction = (_playerTransform.position - spawnPosition).normalized;
            Rigidbody2D rb = flame.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * FlameSpeed;
            }

            // Destruir a chama após a duração
            Destroy(flame, FlameDuration);

            // Aguarda o tempo entre a geração de chamas
            yield return new WaitForSeconds(DelayBetweenFlames);
        }

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }
}
