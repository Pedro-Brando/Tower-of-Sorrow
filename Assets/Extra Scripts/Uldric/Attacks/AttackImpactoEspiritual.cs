using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/ImpactoEspiritual")]
public class AttackImpactoEspiritual : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject ProjectilePrefab;         // Prefab do projétil de Impacto Espiritual
    public GameObject SummonEffectPrefab;       // Prefab do efeito de invocação (opcional)

    [Header("Parâmetros do Ataque")]
    public float ProjectileSpeed = 10f;         // Velocidade do projétil
    public Vector3 ProjectileSpawnOffset = Vector3.up * 5f; // Offset de spawn do projétil em relação a Uldric
    public float ImpactCooldown = 15f;           // Tempo de espera antes de poder realizar o próximo ataque

    private Transform _uldricTransform;          // Referência ao transform de Uldric
    private Transform _playerTransform;           // Referência ao transform do jogador

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

        // Aguarda um breve momento antes de lançar o projétil
        yield return new WaitForSeconds(0.5f);

        // Calcular a posição de spawn do projétil
        Vector3 spawnPosition = _uldricTransform.position + ProjectileSpawnOffset;

        // Instanciar o projétil
        GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        ImpactoEspiritualProjectile projectileBehavior = projectile.GetComponent<ImpactoEspiritualProjectile>();
        if (projectileBehavior != null)
        {
            // Direção fixa para o centro da plataforma
            Vector3 direction = (Vector3.zero - spawnPosition).normalized;
            projectileBehavior.Initialize(direction, ProjectileSpeed);
        }

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(ImpactCooldown);
    }
}
