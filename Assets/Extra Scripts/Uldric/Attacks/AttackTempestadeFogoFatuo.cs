using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/TempestadeFogoFatuo")]
public class AttackTempestadeFogoFatuo : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject FlamePrefab;            // Prefab da chama fátua
    public GameObject SummonEffectPrefab;     // Prefab do efeito de invocação
    public GameObject SpreadEffectPrefab;     // Prefab do efeito de propagação das chamas

    [Header("Parâmetros do Ataque")]
    public int NumberOfFlames = 5;            // Número de chamas fátuas a serem geradas
    public float FlameSpawnRadius = 6f;       // Raio ao redor de Uldric onde as chamas serão geradas
    public float FlameSpreadSpeed = 2f;       // Velocidade de propagação das chamas
    public float FlameDuration = 7f;          // Duração de vida das chamas
    public float DamagePerSecond = 10f;       // Dano causado por segundo pelas chamas
    public float DelayBetweenFlames = 0.7f;   // Tempo entre a geração de cada chama

    [Header("Cooldown")]
    public float AttackCooldown = 15f;        // Tempo de espera antes de poder realizar o próximo ataque

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

            // Configurar a direção da propagação das chamas
            Vector3 direction = (_playerTransform.position - spawnPosition).normalized;
            FlameBehavior flameBehavior = flame.GetComponent<FlameBehavior>();
            if (flameBehavior != null)
            {
                flameBehavior.Initialize(direction, FlameSpreadSpeed, DamagePerSecond);
            }

            // Instanciar o efeito de propagação
            if (SpreadEffectPrefab != null)
            {
                Instantiate(SpreadEffectPrefab, spawnPosition, Quaternion.identity);
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
