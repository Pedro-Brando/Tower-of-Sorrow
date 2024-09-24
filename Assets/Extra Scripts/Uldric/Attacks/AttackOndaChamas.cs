using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/OndaChamas")]
public class AttackOndaChamas : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject FireballPrefab;          // Prefab da bola de fogo
    public GameObject SummonEffectPrefab;      // Prefab do efeito de invocação (opcional)
    public GameObject FlightAnimationPrefab;   // Prefab da animação de voo de Uldric (opcional)

    [Header("Parâmetros do Ataque")]
    public int NumberOfFireballs = 10;         // Número de bolas de fogo a serem lançadas
    public float FireballSpeed = 8f;           // Velocidade das bolas de fogo
    public float FireballSpawnRadius = 2f;     // Raio ao redor de Uldric para spawn das bolas
    public float LaunchDelay = 0.1f;           // Delay entre o lançamento de cada bola
    public Vector3 LaunchDirection = Vector3.right; // Direção inicial do lançamento

    [Header("Padrão de Lançamento")]
    public PatternType LaunchPattern = PatternType.Spread; // Tipo de padrão de lançamento
    public float SpreadAngle = 45f;             // Ângulo de spread para padrão de dispersão
    public float SpiralSpeed = 20f;             // Velocidade de rotação para padrão espiral

    [Header("Cooldown")]
    public float AttackCooldown = 15f;          // Tempo de espera antes de poder realizar o próximo ataque

    private Transform _uldricTransform;         // Referência ao transform de Uldric
    private Transform _playerTransform;          // Referência ao transform do jogador

    // Enum para definir tipos de padrões de lançamento
    public enum PatternType
    {
        Spread,
        Spiral,
        Circular
    }

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

        // Opcional: Instanciar o efeito de invocação
        if (SummonEffectPrefab != null)
        {
            Instantiate(SummonEffectPrefab, _uldricTransform.position, Quaternion.identity);
        }

        // Opcional: Instanciar a animação de voo de Uldric
        if (FlightAnimationPrefab != null)
        {
            Instantiate(FlightAnimationPrefab, _uldricTransform.position, _uldricTransform.rotation, _uldricTransform);
        }

        // Aguarda um breve momento antes de iniciar o lançamento
        yield return new WaitForSeconds(1f);

        // Lançar as bolas de fogo conforme o padrão definido
        for (int i = 0; i < NumberOfFireballs; i++)
        {
            Vector3 spawnPosition = _uldricTransform.position + Random.insideUnitSphere * FireballSpawnRadius;
            spawnPosition.z = 0f; // Garantir que a posição esteja no plano correto

            GameObject fireball = Instantiate(FireballPrefab, spawnPosition, Quaternion.identity);
            FireballBehavior fireballBehavior = fireball.GetComponent<FireballBehavior>();
            if (fireballBehavior != null)
            {
                Vector3 direction = CalculateLaunchDirection(i);
                fireballBehavior.Initialize(direction.normalized, FireballSpeed);
            }

            // Aguarda o delay entre lançamentos
            yield return new WaitForSeconds(LaunchDelay);
        }

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }

    // Método para calcular a direção de lançamento com base no padrão
    private Vector3 CalculateLaunchDirection(int index)
    {
        switch (LaunchPattern)
        {
            case PatternType.Spread:
                float angle = SpreadAngle / NumberOfFireballs;
                return Quaternion.Euler(0, 0, angle * index) * LaunchDirection;

            case PatternType.Spiral:
                return Quaternion.Euler(0, 0, SpiralSpeed * index * Time.deltaTime) * LaunchDirection;

            case PatternType.Circular:
                float circularAngle = 360f / NumberOfFireballs;
                return Quaternion.Euler(0, 0, circularAngle * index) * LaunchDirection;

            default:
                return LaunchDirection;
        }
    }
}
