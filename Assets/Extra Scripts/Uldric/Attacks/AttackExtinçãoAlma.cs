using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/ExtincaoAlma")]
public class AttackExtincaoAlma : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject ExtinctionEffectPrefab;     // Prefab do efeito visual de extinção

    [Header("Parâmetros do Ataque")]
    public float ExtinctionDuration = 3f;         // Duração do efeito de extinção
    public float ExtinctionRange = 10f;           // Alcance do efeito de extinção
    public float ExtinctionDamage = 50f;          // Dano causado pelo efeito de extinção
    public float TeleportDelay = 1f;              // Delay antes de teleportar o jogador
    public float AttackCooldown = 20f;            // Tempo de espera antes de poder realizar o próximo ataque

    private Transform _uldricTransform;            // Referência ao transform de Uldric
    private Transform _playerTransform;             // Referência ao transform do jogador

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

        // Instanciar o efeito de extinção
        if (ExtinctionEffectPrefab != null)
        {
            Instantiate(ExtinctionEffectPrefab, _uldricTransform.position, Quaternion.identity);
        }

        // Aguarda um breve momento antes de aplicar o efeito
        yield return new WaitForSeconds(1f);

        // Aplicar dano aos jogadores dentro do alcance
        Collider2D[] hits = Physics2D.OverlapCircleAll(_uldricTransform.position, ExtinctionRange, LayerMask.GetMask("Player"));
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Aplicar dano ao jogador
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(ExtinctionDamage);
                }

                // Opcional: Aplicar efeitos de knockback ou outros efeitos
            }
        }


        // Aguarda a duração do efeito de extinção
        yield return new WaitForSeconds(ExtinctionDuration);

        // Teleportar o jogador de volta para o mundo material
        TeleportPlayerToMaterialWorld();

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar o alcance do ataque no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_uldricTransform.position, ExtinctionRange);
    }
}
